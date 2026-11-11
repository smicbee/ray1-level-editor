using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BinarySerializer;
using BinarySerializer.Ray1;
using BinarySerializer.Ray1.PC;

namespace Ray1LevelEditor
{
    public struct Cell { public ushort Tile; public byte Type; public byte Render; public byte PadA, PadB; }

    /// <summary>One level object ("event") together with its command script and link group.</summary>
    public class EObj
    {
        public ObjData D;
        public Command[] Cmds = new Command[0];
        public ushort[] Labels = new ushort[0];
        public int Link; // 0 = not linked, otherwise group id

        public EObj Clone()
        {
            var o = new EObj { Link = Link, Labels = (ushort[])Labels.Clone() };
            var mc = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
            var d = (ObjData)mc.Invoke(D, null);
            d.BlockTypes = D.BlockTypes == null ? null : (BlockType[])D.BlockTypes.Clone();
            d.UnknownBytes = D.UnknownBytes == null ? null : (byte[])D.UnknownBytes.Clone();
            d.CommandContexts = D.CommandContexts?.Select(c => new CommandContext { ReturnOffset = c.ReturnOffset, Count = c.Count }).ToArray();
            d.TypeZDC = D.TypeZDC == null ? null : new ZDCReference { Index = D.TypeZDC.Index, Count = D.TypeZDC.Count };
            o.D = d;
            o.Cmds = Cmds.Select(c => new Command { CommandType = c.CommandType, Arguments = (byte[])c.Arguments.Clone() }).ToArray();
            return o;
        }
    }

    public class Snapshot
    {
        public int W, H; public Cell[] Cells; public List<EObj> Objs; public byte[][] Pal;
        public byte Fond, Fond2, Last; public int ScrollDiff;
    }

    public class Sheet { public int X, Y, W, H; public byte[] Bgra; }

    public static class Names
    {
        public static readonly string[] World = { "JUNGLE", "MUSIC", "MOUNTAIN", "IMAGE", "CAVE", "CAKE" };
        public static readonly string[] WorldDe = { "Dream Forest", "Band Land", "Blue Mountains", "Picture City", "Caves of Skops", "Candy Chateau" };

        public static int ExpectedArgs(CommandType t, int firstArg)
        {
            switch (t)
            {
                case CommandType.GO_DOLOOP: case CommandType.GO_RETURN: return 0;
                case CommandType.GO_TEST: return firstArg <= 4 ? 2 : 1;
                case CommandType.GO_SPEED: return 3;
                case CommandType.GO_X: case CommandType.GO_Y: return 2;
                default: return 1;
            }
        }
    }

    public class LevelDoc
    {
        public string Root; public int World, LevelNo;
        public string Rel => $"PCMAP/{Names.World[World - 1]}/RAY{LevelNo}.LEV";
        Context ctx;
        public LevelFile Lev; public AllfixFile Fix; public WorldFile Wld;
        public int W, H; public Cell[] Cells; public List<EObj> Objs = new List<EObj>();
        public byte[][] Pal; // 3 palettes x 256*3, values 0..63
        public byte Fond, Fond2, Last; public int ScrollDiff;
        public Design[] Des; public States[] Eta;
        public int TileCount;
        Dictionary<long, TileSetBlock> blockByOff = new Dictionary<long, TileSetBlock>();
        HashSet<long> transOff = new HashSet<long>();
        Dictionary<ushort, byte[]> tileCache = new Dictionary<ushort, byte[]>();
        Dictionary<int, byte[]> desImg = new Dictionary<int, byte[]>();
        Dictionary<(int, int), Sheet> sprCache = new Dictionary<(int, int), Sheet>();
        Dictionary<EObj, (int, int, int, int, int, int)> visKey = new Dictionary<EObj, (int, int, int, int, int, int)>();

        public static LevelDoc Open(string root, int world, int level)
        {
            var d = new LevelDoc { Root = root, World = world, LevelNo = level };
            d.ctx = new Context(root);
            d.ctx.AddSettings(new Ray1Settings(Ray1EngineVersion.PC, (World)world, level, Ray1PCVersion.PC_1_21));
            string wldRel = $"PCMAP/RAY{world}.WLD";
            d.ctx.AddFile(new LinearFile(d.ctx, d.Rel));
            d.ctx.AddFile(new LinearFile(d.ctx, "PCMAP/ALLFIX.DAT"));
            d.ctx.AddFile(new LinearFile(d.ctx, wldRel));
            d.Lev = FileFactory.Read<LevelFile>(d.ctx, d.Rel);
            d.Fix = FileFactory.Read<AllfixFile>(d.ctx, "PCMAP/ALLFIX.DAT");
            d.Wld = FileFactory.Read<WorldFile>(d.ctx, wldRel);
            d.Des = new Design[] { null }.Concat(d.Fix.DesItems).Concat(d.Wld.DesItems).ToArray();
            d.Eta = d.Fix.Eta.Concat(d.Wld.Eta).ToArray();
            var mi = d.Lev.MapInfo;
            d.W = mi.Width; d.H = mi.Height;
            d.Cells = mi.Blocks.Select(b => new Cell { Tile = b.TileIndex, Type = (byte)b.BlockType, Render = (byte)b.RenderMode, PadA = b.PadA, PadB = b.PadB }).ToArray();
            d.Pal = mi.Palettes.Select(p => p.SelectMany(c => new[] { (byte)(c.R & 0x3F), (byte)(c.G & 0x3F), (byte)(c.B & 0x3F) }).ToArray()).ToArray();
            d.Last = mi.LastPlan1Palette; d.Fond = d.Lev.FondIndex; d.Fond2 = d.Lev.ScrollDiffFondIndex; d.ScrollDiff = d.Lev.ScrollDiffSprites;
            var lo = d.Lev.ObjData;
            for (int i = 0; i < lo.ObjectsCount; i++)
                d.Objs.Add(new EObj { D = lo.Objects[i], Cmds = lo.ObjCommands[i].Commands?.Commands ?? new Command[0], Labels = lo.ObjCommands[i].LabelOffsetTable ?? new ushort[0] });
            InitLinks(d.Objs, lo.ObjectLinkingTable);
            var ts = d.Lev.TileSetNormal; var mb = ts.MapBlocks; d.TileCount = (int)ts.TotalBlocksCount;
            foreach (var b in mb.OpaqueBlocks) d.blockByOff[b.Offset.FileOffset - mb.Offset.FileOffset] = b;
            foreach (var b in mb.TransparentBlocks) { long o = b.Offset.FileOffset - mb.Offset.FileOffset; d.blockByOff[o] = b; d.transOff.Add(o); }
            return d;
        }

        public static void InitLinks(List<EObj> objs, ushort[] t)
        {
            int cur = 1;
            for (int i = 0; i < objs.Count && i < t.Length; i++)
            {
                if (t[i] == i) { objs[i].Link = 0; continue; }
                if (objs[i].Link != 0) continue;
                int next = t[i]; objs[i].Link = cur; int prev = i;
                while (next != i && next != prev && next < objs.Count) { prev = next; objs[next].Link = cur; next = t[next]; }
                cur++;
            }
        }

        public static ushort[] BuildLinks(List<EObj> objs)
        {
            var t = new ushort[objs.Count];
            for (int i = 0; i < t.Length; i++) t[i] = (ushort)i;
            foreach (var g in objs.Select((o, i) => (o, i)).Where(x => x.o.Link != 0).GroupBy(x => x.o.Link))
            {
                var idx = g.Select(x => x.i).ToList();
                for (int j = 0; j < idx.Count; j++) t[idx[j]] = (ushort)idx[(j + 1) % idx.Count];
            }
            return t;
        }

        // ---------- snapshots ----------
        public Snapshot Snap() => new Snapshot { W = W, H = H, Cells = (Cell[])Cells.Clone(), Objs = Objs.Select(o => o.Clone()).ToList(), Pal = Pal.Select(p => (byte[])p.Clone()).ToArray(), Fond = Fond, Fond2 = Fond2, Last = Last, ScrollDiff = ScrollDiff };
        public void Restore(Snapshot s)
        {
            W = s.W; H = s.H; Cells = (Cell[])s.Cells.Clone(); Objs = s.Objs.Select(o => o.Clone()).ToList(); Pal = s.Pal.Select(p => (byte[])p.Clone()).ToArray();
            Fond = s.Fond; Fond2 = s.Fond2; Last = s.Last; ScrollDiff = s.ScrollDiff; ClearCaches();
        }
        public void ClearCaches() { tileCache.Clear(); sprCache.Clear(); }

        public void Resize(int nw, int nh)
        {
            nw = Math.Max(1, Math.Min(nw, 1000)); nh = Math.Max(1, Math.Min(nh, 1000));
            var n = new Cell[nw * nh];
            for (int y = 0; y < nh; y++) for (int x = 0; x < nw; x++)
                    n[y * nw + x] = (x < W && y < H) ? Cells[y * W + x] : new Cell { Tile = 0, Type = 0, Render = 1 };
            Cells = n; W = nw; H = nh;
        }

        // ---------- tiles ----------
        public bool IsTransparentTile(ushort idx)
        {
            var t = Lev.TileSetNormal.BlocksOffsetTable;
            return idx < t.Length && transOff.Contains(t[idx]);
        }
        public byte RenderModeFor(ushort tile) => tile == 0 ? (byte)1 : IsTransparentTile(tile) ? (byte)2 : (byte)0;

        public byte[] TileBgra(ushort idx)
        {
            if (tileCache.TryGetValue(idx, out var c)) return c;
            var res = new byte[16 * 16 * 4];
            var tab = Lev.TileSetNormal.BlocksOffsetTable;
            if (idx < tab.Length && blockByOff.TryGetValue(tab[idx], out var blk))
            {
                bool tr = transOff.Contains(tab[idx]);
                for (int i = 0; i < 256; i++)
                {
                    int ci = 255 - blk.ImgData[i];
                    bool vis = !tr || (blk.Alpha != null && blk.Alpha[i] != 0 && ci != 0);
                    if (!tr) vis = true;
                    SetPx(res, i * 4, ci, vis);
                }
            }
            tileCache[idx] = res; return res;
        }

        void SetPx(byte[] b, int o, int ci, bool vis)
        {
            var p = Pal[0];
            b[o] = (byte)(p[ci * 3 + 2] * 255 / 63); b[o + 1] = (byte)(p[ci * 3 + 1] * 255 / 63); b[o + 2] = (byte)(p[ci * 3] * 255 / 63); b[o + 3] = vis ? (byte)255 : (byte)0;
        }

        // ---------- sprites / objects ----------
        byte[] DesImage(int d)
        {
            if (desImg.TryGetValue(d, out var b)) return b;
            var des = Des[d]; b = des.IsAnimatedSprite ? Design.ProcessImageData(des.ImageData) : des.ImageData; desImg[d] = b; return b;
        }

        public Sheet Sprite(int d, int s, bool flip)
        {
            if (d <= 0 || d >= Des.Length) return null;
            var key = (d, s * 2 + (flip ? 1 : 0));
            if (sprCache.TryGetValue(key, out var sh)) return sh;
            var spr = Des[d].Sprites; Sheet r = null;
            if (s >= 0 && s < spr.Length && !spr[s].IsDummySprite())
            {
                var sp = spr[s]; int w = sp.Width, h = sp.Height; var img = DesImage(d); r = new Sheet { W = w, H = h, Bgra = new byte[w * h * 4] };
                for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
                    {
                        int si = sp.ImageBufferOffset + y * w + x; int ci = si < img.Length ? img[si] : 0;
                        int dx = flip ? w - 1 - x : x;
                        SetPx(r.Bgra, (y * w + dx) * 4, ci, ci != 0);
                    }
            }
            sprCache[key] = r; return r;
        }

        /// <summary>Layers of the current (first frame) animation of the object's state, positions relative to object origin.</summary>
        public List<Sheet> Visual(EObj o) => VisualAt(o.D, o.D.MainEtat, o.D.SubEtat, o.D.FlipX, 0);

        public ObjState StateOf(ObjData d, int main, int sub)
        {
            int ei = (int)d.PCPacked_ETAIndex;
            if (ei < 0 || ei >= Eta.Length) return null;
            var st = Eta[ei].ObjectStates;
            if (main < 0 || main >= st.Length || sub < 0 || sub >= st[main].Length) return null;
            return st[main][sub];
        }

        /// <summary>Animation frame count of the given state (0 if unknown).</summary>
        public int FrameCount(ObjData d, int main, int sub)
        {
            try
            {
                var s = StateOf(d, main, sub); int di = (int)d.PCPacked_AnimationsIndex; if (s == null || di <= 0 || di >= Des.Length) return 0;
                var anims = Des[di].Animations; return s.AnimationIndex < anims.Length ? anims[s.AnimationIndex].FramesCount : 0;
            }
            catch { return 0; }
        }

        public List<Sheet> VisualAt(ObjData d, int main, int sub, bool flipX, int frame)
        {
            var list = new List<Sheet>();
            try
            {
                int di = (int)d.PCPacked_AnimationsIndex; int si = (int)d.PCPacked_SpritesIndex;
                var s0 = StateOf(d, main, sub); if (s0 == null || di <= 0 || di >= Des.Length) return list;
                int ai = s0.AnimationIndex;
                var anims = Des[di].Animations; if (ai >= anims.Length) return list;
                var an = anims[ai]; int lc = an.LayersCount;
                int fc = Math.Max(1, (int)an.FramesCount); frame = ((frame % fc) + fc) % fc;
                int fw = (an.Frames != null && an.Frames.Length > frame) ? an.Frames[frame].Width : 0;
                for (int i = 0; i < lc && frame * lc + i < an.Layers.Length; i++)
                {
                    var l = an.Layers[frame * lc + i];
                    bool flip = l.FlipX ^ flipX;
                    var sh = Sprite(si, l.SpriteIndex, flip); if (sh == null) continue;
                    int x = l.XPosition; if (flipX) x = fw - l.XPosition - sh.W;
                    list.Add(new Sheet { X = x, Y = l.YPosition, W = sh.W, H = sh.H, Bgra = sh.Bgra });
                }
            }
            catch { }
            return list;
        }

        // ---------- background images (VIGNET.DAT PCX files referenced by the world file) ----------
        public class Pcx { public int W, H, Index; public byte[] Idx; public byte[] Pal; }
        public string BgError; FileArchive vig; readonly Dictionary<int, Pcx> pcxCache = new Dictionary<int, Pcx>();

        /// <summary>Index into VIGNET.DAT for the given background index (LEV FondIndex / ScrollDiffFondIndex index the world's PcxFileIndexes table), or -1.</summary>
        public int BgFile(int fondIdx) => Wld.PcxFileIndexes != null && fondIdx >= 0 && fondIdx < Wld.PcxFileIndexes.Length ? Wld.PcxFileIndexes[fondIdx] : -1;
        public int BgCount => Wld.PcxFileIndexes?.Length ?? 0;

        public Pcx Background(int fondIdx)
        {
            int fi = BgFile(fondIdx); if (fi < 0) return null;
            if (pcxCache.TryGetValue(fi, out var c)) return c;
            Pcx r = null;
            try
            {
                if (vig == null)
                {
                    ctx.AddFile(new LinearFile(ctx, "VIGNET.DAT"));
                    vig = FileFactory.Read<FileArchive>(ctx, "VIGNET.DAT");
                }
                r = DecodePcx(vig.ReadFileBytes(ctx, fi)); if (r != null) r.Index = fi;
            }
            catch (Exception ex) { BgError = ex.GetType().Name + ": " + ex.Message; }
            pcxCache[fi] = r; return r;
        }

        public static Pcx DecodePcx(byte[] b)
        {
            if (b == null || b.Length < 900 || b[0] != 0x0A || b[2] != 1 || b[3] != 8) return null;
            int w = (b[8] | b[9] << 8) - (b[4] | b[5] << 8) + 1, h = (b[10] | b[11] << 8) - (b[6] | b[7] << 8) + 1, bpl = b[66] | b[67] << 8;
            var idx = new byte[w * h]; int p = 128; var line = new byte[bpl];
            for (int y = 0; y < h; y++)
            {
                int x = 0;
                while (x < bpl && p < b.Length)
                {
                    byte v = b[p++]; int run = 1;
                    if ((v & 0xC0) == 0xC0) { run = v & 0x3F; v = b[p++]; }
                    for (int i = 0; i < run && x < bpl; i++) line[x++] = v;
                }
                Array.Copy(line, 0, idx, y * w, w);
            }
            var pal = new byte[768];
            if (b.Length >= 769 && b[b.Length - 769] == 0x0C) Array.Copy(b, b.Length - 768, pal, 0, 768);
            int max = 0; foreach (var v in pal) if (v > max) max = v;
            if (max < 64) for (int i = 0; i < 768; i++) pal[i] = (byte)(pal[i] * 255 / 63); // 6-bit VGA palette
            return new Pcx { W = w, H = h, Idx = idx, Pal = pal };
        }

        public static byte[] PcxBgra(Pcx p, bool index0Transparent)
        {
            var o = new byte[p.W * p.H * 4];
            for (int i = 0; i < p.Idx.Length; i++)
            {
                int ci = p.Idx[i]; o[i * 4] = p.Pal[ci * 3 + 2]; o[i * 4 + 1] = p.Pal[ci * 3 + 1]; o[i * 4 + 2] = p.Pal[ci * 3]; o[i * 4 + 3] = (byte)(index0Transparent && ci == 0 ? 0 : 255);
            }
            return o;
        }

        public (int x, int y, int w, int h) Bounds(EObj o)
        {
            var v = Visual(o);
            if (v.Count == 0) return (o.D.XPosition, o.D.YPosition, 16, 16);
            int x0 = v.Min(s => s.X), y0 = v.Min(s => s.Y), x1 = v.Max(s => s.X + s.W), y1 = v.Max(s => s.Y + s.H);
            return (o.D.XPosition + x0, o.D.YPosition + y0, x1 - x0, y1 - y0);
        }

        // ---------- rendering ----------
        static readonly Dictionary<int, (byte, byte, byte)> collColors = new Dictionary<int, (byte, byte, byte)>
        {
            {1,(0,255,255)},{2,(255,140,0)},{3,(255,140,0)},{4,(255,170,60)},{5,(255,170,60)},{6,(255,170,60)},{7,(255,170,60)},{8,(255,0,0)},{9,(255,0,255)},
            {10,(0,80,255)},{11,(0,255,0)},{12,(255,255,0)},{13,(120,180,255)},{14,(200,200,200)},{15,(70,70,70)},{16,(150,90,30)},
            {18,(160,255,255)},{19,(160,255,255)},{20,(160,255,255)},{21,(160,255,255)},{22,(160,255,255)},{23,(160,255,255)},{24,(220,30,30)},{25,(160,0,200)},{30,(0,170,170)}
        };
        public static (byte, byte, byte) CollColor(int t) => collColors.TryGetValue(t, out var c) ? c : ((byte)255, (byte)255, (byte)255);

        public bool TransparentEmpty; // when a background image is shown: empty tile pixels become transparent

        public void RenderCell(byte[] buf, int stride, int cx, int cy, bool coll)
        {
            var c = Cells[cy * W + cx]; var tile = TileBgra(c.Tile);
            for (int y = 0; y < 16; y++) for (int x = 0; x < 16; x++)
                {
                    int s = (y * 16 + x) * 4; int d = ((cy * 16 + y) * stride) + (cx * 16 + x) * 4;
                    bool vis = tile[s + 3] != 0 && !(TransparentEmpty && c.Tile == 0);
                    if (vis) { buf[d] = tile[s]; buf[d + 1] = tile[s + 1]; buf[d + 2] = tile[s + 2]; buf[d + 3] = 255; }
                    else if (TransparentEmpty) { buf[d] = 0; buf[d + 1] = 0; buf[d + 2] = 0; buf[d + 3] = 0; }
                    else { buf[d] = 0x50; buf[d + 1] = 0x30; buf[d + 2] = 0x20; buf[d + 3] = 255; }
                    if (coll && c.Type != 0)
                    {
                        var (r, g, b) = CollColor(c.Type); bool edge = x == 0 || y == 0 || x == 15 || y == 15; int a = edge ? 200 : 110;
                        if (!vis && TransparentEmpty) { buf[d] = b; buf[d + 1] = g; buf[d + 2] = r; buf[d + 3] = (byte)a; }
                        else { buf[d] = (byte)((buf[d] * (255 - a) + b * a) / 255); buf[d + 1] = (byte)((buf[d + 1] * (255 - a) + g * a) / 255); buf[d + 2] = (byte)((buf[d + 2] * (255 - a) + r * a) / 255); }
                    }
                }
        }

        public byte[] RenderMap(bool coll, bool objs, out int pw, out int ph)
        {
            pw = W * 16; ph = H * 16; int stride = pw * 4; var buf = new byte[stride * ph];
            for (int y = 0; y < H; y++) for (int x = 0; x < W; x++) RenderCell(buf, stride, x, y, coll);
            if (objs) foreach (var o in Objs) DrawObj(buf, pw, ph, o);
            return buf;
        }

        public void DrawObj(byte[] buf, int pw, int ph, EObj o)
        {
            foreach (var l in Visual(o))
                for (int y = 0; y < l.H; y++) for (int x = 0; x < l.W; x++)
                    {
                        int px = o.D.XPosition + l.X + x, py = o.D.YPosition + l.Y + y; if (px < 0 || py < 0 || px >= pw || py >= ph) continue;
                        int s = (y * l.W + x) * 4; if (l.Bgra[s + 3] == 0) continue; int d = (py * pw + px) * 4;
                        buf[d] = l.Bgra[s]; buf[d + 1] = l.Bgra[s + 1]; buf[d + 2] = l.Bgra[s + 2]; buf[d + 3] = 255;
                    }
        }

        // ---------- saving ----------
        public static string ValidateCommands(Command[] cmds)
        {
            for (int i = 0; i < cmds.Length; i++)
            {
                var c = cmds[i]; int first = c.Arguments.Length > 0 ? c.Arguments[0] : 0;
                if (c.Arguments.Length != Names.ExpectedArgs(c.CommandType, first)) return $"Command {i + 1} ({c.CommandType}) needs {Names.ExpectedArgs(c.CommandType, first)} argument(s)";
            }
            return null;
        }

        /// <summary>Writes the level back. Serialises to a temp file in the same folder first, validates by re-reading, then replaces the target.</summary>
        public void Save()
        {
            var mi = Lev.MapInfo;
            mi.Width = (ushort)W; mi.Height = (ushort)H;
            var oldBlocks = mi.Blocks;
            mi.Blocks = Cells.Select(c => new Block { TileIndex = c.Tile, BlockType = (BlockType)c.Type, RenderMode = (Block.BlockRenderMode)c.Render, PadA = c.PadA, PadB = c.PadB }).ToArray();
            for (int p = 0; p < mi.Palettes.Length && p < Pal.Length; p++)
                for (int i = 0; i < 256; i++) { var c = mi.Palettes[p][i]; c.R = Pal[p][i * 3]; c.G = Pal[p][i * 3 + 1]; c.B = Pal[p][i * 3 + 2]; }
            mi.LastPlan1Palette = Last; Lev.FondIndex = Fond; Lev.ScrollDiffFondIndex = Fond2; Lev.ScrollDiffSprites = ScrollDiff;
            var lo = Lev.ObjData;
            lo.ObjectsCount = (ushort)Objs.Count;
            lo.Objects = Objs.Select(o => o.D).ToArray();
            lo.ObjectLinkingTable = BuildLinks(Objs);
            lo.ObjCommands = Objs.Select(o => new ObjCommandsData { Commands = new ObjCommands { Commands = o.Cmds }, LabelOffsetTable = o.Labels }).ToArray();
            foreach (var o in Objs) { var e = ValidateCommands(o.Cmds); if (e != null) throw new InvalidDataException(e); }

            string tmpRel = Rel.Substring(0, Rel.Length - 4) + ".TMP";
            string tmpAbs = Path.Combine(Root, tmpRel.Replace('/', Path.DirectorySeparatorChar));
            string dstAbs = Path.Combine(Root, Rel.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(tmpAbs)) File.Delete(tmpAbs);
            ctx.AddFile(new LinearFile(ctx, tmpRel));
            FileFactory.Write<LevelFile>(ctx, tmpRel, Lev);
            ctx.RemoveFile(tmpRel);
            // validate by re-reading with a fresh context
            using (var c2 = new Context(Root))
            {
                c2.AddSettings(new Ray1Settings(Ray1EngineVersion.PC, (World)World, LevelNo, Ray1PCVersion.PC_1_21));
                c2.AddFile(new LinearFile(c2, tmpRel));
                var chk = FileFactory.Read<LevelFile>(c2, tmpRel);
                if (chk.MapInfo.Width != W || chk.MapInfo.Height != H || chk.ObjData.ObjectsCount != Objs.Count) throw new InvalidDataException("Validation after writing failed");
            }
            File.Copy(tmpAbs, dstAbs, true);
            File.Delete(tmpAbs);
        }
    }
}
