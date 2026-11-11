using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Ray1;

namespace Ray1LevelEditor
{
    /// <summary>Script text <-> compiled commands. Text uses label ids (GO_GOTO 3), the label offset table is recomputed with ObjCommandCompiler.</summary>
    public static class ScriptText
    {
        static CommandType Sym(CommandType t) => t switch
        {
            CommandType.RESERVED_GO_GOTO => CommandType.GO_GOTO, CommandType.RESERVED_GO_GOSUB => CommandType.GO_GOSUB,
            CommandType.RESERVED_GO_GOTOT => CommandType.GO_BRANCHTRUE, CommandType.RESERVED_GO_GOTOF => CommandType.GO_BRANCHFALSE,
            CommandType.RESERVED_GO_SKIP => CommandType.GO_SKIP, CommandType.RESERVED_GO_SKIPT => CommandType.GO_SKIPTRUE,
            CommandType.RESERVED_GO_SKIPF => CommandType.GO_SKIPFALSE, _ => t
        };
        static bool IsJump(CommandType t) => t is CommandType.GO_GOTO or CommandType.GO_GOSUB or CommandType.GO_BRANCHTRUE or CommandType.GO_BRANCHFALSE;
        static bool IsSkip(CommandType t) => t is CommandType.GO_SKIP or CommandType.GO_SKIPTRUE or CommandType.GO_SKIPFALSE;
        public static bool IsReserved(CommandType t) => Sym(t) != t;

        public static int[] Starts(Command[] cmds) { var r = new int[cmds.Length]; int p = 0; for (int i = 0; i < cmds.Length; i++) { r[i] = p; p += cmds[i].Length; } return r; }

        /// <summary>Decompiles the stored (compiled) commands to the symbolic text, 1 command per line (line i = command i).</summary>
        public static string ToText(Command[] cmds, ushort[] labels)
        {
            var starts = Starts(cmds); var lines = new string[cmds.Length]; var sym = new List<Command>();
            var rawSkip = new Dictionary<int, int>();
            for (int k = 0; k < cmds.Length; k++)
            {
                var c = cmds[k]; var st = Sym(c.CommandType);
                if (st == c.CommandType) { sym.Add(new Command { CommandType = st, Arguments = (byte[])c.Arguments.Clone() }); continue; }
                int off = c.Arguments.Length > 0 && c.Arguments[0] < labels.Length ? labels[c.Arguments[0]] : -1;
                if (IsJump(st))
                {
                    int t = Array.IndexOf(starts, off - 1);
                    if (t >= 0 && cmds[t].CommandType == CommandType.GO_LABEL) { sym.Add(new Command { CommandType = st, Arguments = new[] { cmds[t].Arguments[0] } }); continue; }
                    rawSkip[k] = off; sym.Add(new Command { CommandType = st == CommandType.GO_GOTO ? CommandType.GO_SKIP : st, Arguments = new byte[] { 0 } }); // unresolved: shown raw
                    lines[k] = $"{st} @{off}"; continue;
                }
                sym.Add(new Command { CommandType = st, Arguments = new byte[] { 255 } }); rawSkip[k] = off; // resolve below
            }
            for (int k = 0; k < cmds.Length; k++)
            {
                if (!IsReserved(cmds[k].CommandType) || !IsSkip(Sym(cmds[k].CommandType))) continue;
                int want = rawSkip[k]; int idx = 0; for (int j = 0; j < k; j++) if (IsReserved(cmds[j].CommandType)) idx++;
                bool found = false;
                for (int n = 0; n < 64 && !found; n++)
                {
                    sym[k].Arguments[0] = (byte)n;
                    try { var cc = ObjCommandCompiler.Compile(new ObjCommands { Commands = ToCompilable(sym, rawSkip, k, n).ToArray() }); if (idx < cc.LabelOffsets.Length && cc.LabelOffsets[idx] == want) found = true; } catch { }
                }
                if (found) lines[k] = $"{sym[k].CommandType} {sym[k].Arguments[0]}"; else lines[k] = $"{sym[k].CommandType} @{want}";
            }
            for (int k = 0; k < cmds.Length; k++)
                lines[k] ??= sym[k].CommandType + (sym[k].Arguments.Length > 0 ? " " + string.Join(" ", sym[k].Arguments) : "");
            return string.Join("\n", lines);
        }

        // builds a compilable list: unresolved skips/jumps (still in rawSkip) get a harmless placeholder GO_SKIP 0
        static List<Command> ToCompilable(List<Command> sym, Dictionary<int, int> raw, int exceptK, int n)
        {
            var l = new List<Command>();
            for (int i = 0; i < sym.Count; i++)
            {
                var c = sym[i];
                if (i != exceptK && raw.ContainsKey(i) && c.Arguments.Length == 1 && (c.Arguments[0] == 255 || IsSkip(c.CommandType) || IsJump(c.CommandType)) && c.Arguments[0] == 255) l.Add(new Command { CommandType = c.CommandType, Arguments = new byte[] { 0 } });
                else l.Add(c);
            }
            return l;
        }

        public class ParseResult { public Command[] Cmds; public ushort[] Labels; public List<string> Errors = new List<string>(); public List<int> LineOfCmd = new List<int>(); }

        /// <summary>Parses + validates + compiles. Errors contain line numbers (1-based).</summary>
        public static ParseResult Parse(string text)
        {
            var r = new ParseResult(); var sym = new List<Command>(); var rawOff = new Dictionary<int, int>();
            var lines = text.Replace("\r", "").Split('\n');
            for (int ln = 0; ln < lines.Length; ln++)
            {
                var raw = lines[ln].Trim(); if (raw.Length == 0 || raw.StartsWith("#")) continue;
                var parts = raw.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (!Enum.TryParse(parts[0], out CommandType ct) || !Enum.IsDefined(typeof(CommandType), ct)) { r.Errors.Add($"Line {ln + 1}: unknown command '{parts[0]}'"); continue; }
                if (IsReserved(ct)) { r.Errors.Add($"Line {ln + 1}: {ct} is the compiled form - please use {Sym(ct)} with a label number"); continue; }
                if (ct == CommandType.INVALID_CMD_DEMO) { r.Errors.Add($"Line {ln + 1}: {ct} not allowed"); continue; }
                if (parts.Length == 2 && parts[1].StartsWith("@") && (IsJump(ct) || IsSkip(ct)))
                {
                    if (!int.TryParse(parts[1].Substring(1), out int ro) || ro < 0 || ro > 65535) { r.Errors.Add($"Line {ln + 1}: invalid raw offset '{parts[1]}'"); continue; }
                    rawOff[sym.Count] = ro; sym.Add(new Command { CommandType = ct, Arguments = new byte[] { 0 } }); r.LineOfCmd.Add(ln); continue;
                }
                var args = new List<byte>(); bool bad = false;
                foreach (var a in parts.Skip(1)) { if (!byte.TryParse(a, out byte b)) { r.Errors.Add($"Line {ln + 1}: argument '{a}' is not a number 0..255"); bad = true; break; } args.Add(b); }
                if (bad) continue;
                int first = args.Count > 0 ? args[0] : 0; int exp = Names.ExpectedArgs(ct, first);
                if (args.Count != exp) { r.Errors.Add($"Line {ln + 1}: {ct} needs {exp} argument(s), found {args.Count}"); continue; }
                sym.Add(new Command { CommandType = ct, Arguments = args.ToArray() }); r.LineOfCmd.Add(ln);
            }
            var defined = new HashSet<byte>(sym.Where(c => c.CommandType == CommandType.GO_LABEL).Select(c => c.Arguments[0]));
            for (int i = 0; i < sym.Count; i++)
                if (IsJump(sym[i].CommandType) && !rawOff.ContainsKey(i) && !defined.Contains(sym[i].Arguments[0])) r.Errors.Add($"Line {r.LineOfCmd[i] + 1}: label {sym[i].Arguments[0]} not defined (GO_LABEL {sym[i].Arguments[0]} missing)");
            if (r.Errors.Count > 0) return r;
            try
            {
                var comp = ObjCommandCompiler.Compile(new ObjCommands { Commands = sym.ToArray() });
                var labels = comp.LabelOffsets.ToArray(); int idx = 0;
                for (int i = 0; i < sym.Count; i++) if (IsJump(sym[i].CommandType) || IsSkip(sym[i].CommandType)) { if (rawOff.TryGetValue(i, out int ro)) labels[idx] = (ushort)ro; idx++; }
                r.Cmds = comp.Commands.Commands; r.Labels = labels;
            }
            catch (Exception ex) { r.Errors.Add("Compilation failed: " + ex.Message); }
            return r;
        }
    }

    public class SimSeg
    {
        public int T0, T1, Cmd, Main, Sub, AnimSpeed; public double X0, Y0, VX, VY; public bool Flip; public string Desc;
        public double XAt(double t) => X0 + VX * (Math.Min(t, T1) - T0);
        public double YAt(double t) => Y0 + VY * (Math.Min(t, T1) - T0);
    }

    public class SimResult
    {
        public List<SimSeg> Segs = new List<SimSeg>(); public int TotalTicks; public bool Truncated; public string Note;
        public SimSeg At(double t) { for (int i = Segs.Count - 1; i >= 0; i--) if (t >= Segs[i].T0) return Segs[i]; return Segs.Count > 0 ? Segs[0] : null; }
    }

    /// <summary>Approximate interpreter. Does NOT modify level data. 1 tick = 1 game frame; speeds from ETA state (RightSpeed/LeftSpeed, px per tick).</summary>
    public static class Sim
    {
        /// <summary>ASSUMPTION: ETA/GO_SPEED speeds are in 1/16 px per tick (Rayman engine fixed-point); durations are ticks. Not verified against the engine source.</summary>
        public const double SpeedUnit = 16.0;
        public static SimResult Run(LevelDoc doc, EObj o, bool testValue, int maxTicks = 1500, int maxExec = 40000)
        {
            var res = new SimResult(); var cmds = o.Cmds; var d = o.D;
            double x = d.XPosition, y = d.YPosition; int main = d.MainEtat, sub = d.SubEtat; bool flip = d.FlipX; bool test = testValue;
            double vx = 0, vy = 0; int t = 0;
            if (cmds.Length == 0) { res.Note = "Object has no script."; return res; }
            var starts = ScriptText.Starts(cmds);
            int Label(int lab) { for (int i = 0; i < cmds.Length; i++) if (cmds[i].CommandType == CommandType.GO_LABEL && cmds[i].Arguments[0] == lab) return i; return -1; }
            int JumpTarget(Command c)
            {
                var ct = c.CommandType;
                if (ct is CommandType.GO_GOTO or CommandType.GO_GOSUB or CommandType.GO_BRANCHTRUE or CommandType.GO_BRANCHFALSE) return Label(c.Arguments[0]);
                int ix = c.Arguments[0]; if (ix >= o.Labels.Length) return -1; int off = o.Labels[ix];
                bool isSkip = ct is CommandType.RESERVED_GO_SKIP or CommandType.RESERVED_GO_SKIPT or CommandType.RESERVED_GO_SKIPF;
                int tg = Array.IndexOf(starts, isSkip ? off + 1 : off - 1);
                if (tg < 0 && isSkip) for (int i = 0; i < starts.Length; i++) if (starts[i] > off) { tg = i; break; }
                return tg;
            }
            var calls = new Stack<int>(); var loops = new Stack<(int start, int left)>();
            int pc = 0, exec = 0;
            void Timed(int cmd, int dur, double nvx, double nvy, string desc)
            {
                dur = Math.Max(1, dur); var st = doc.StateOf(d, main, sub);
                res.Segs.Add(new SimSeg { T0 = t, T1 = t + dur, Cmd = cmd, Main = main, Sub = sub, X0 = x, Y0 = y, VX = nvx, VY = nvy, Flip = flip, Desc = desc, AnimSpeed = st != null ? Math.Max(1, st.AnimationSpeed & 0x0F) : 4 });
                x += nvx * dur; y += nvy * dur; t += dur;
            }
            while (t < maxTicks && exec++ < maxExec && pc < cmds.Length)
            {
                var c = cmds[pc]; int a0 = c.Arguments.Length > 0 ? c.Arguments[0] : 0; int cur = pc; pc++;
                var ct = c.CommandType;
                switch (ct)
                {
                    case CommandType.GO_LEFT: case CommandType.GO_RIGHT:
                        {
                            flip = ct == CommandType.GO_RIGHT; if (main == 0) { main = 1; sub = 0; }
                            var st = doc.StateOf(d, main, sub); double sp = st == null ? 0 : (flip ? st.RightSpeed : st.LeftSpeed) / SpeedUnit;
                            if (sp == 0 && vx != 0) sp = vx; Timed(cur, a0, sp, vy, $"{ct} {a0}"); break;
                        }
                    case CommandType.GO_WAIT: if (main == 1) { main = 0; sub = 0; } Timed(cur, a0, vx, vy, $"WAIT {a0}"); break;
                    case CommandType.GO_NOP: Timed(cur, a0, vx, vy, $"NOP {a0}"); break;
                    case CommandType.GO_UP: Timed(cur, a0, 0, -2, $"UP {a0}"); break;
                    case CommandType.GO_DOWN: Timed(cur, a0, 0, 2, $"DOWN {a0}"); break;
                    case CommandType.GO_SPEED: vx = (sbyte)c.Arguments[1] / SpeedUnit; vy = (sbyte)c.Arguments[2] / SpeedUnit; Timed(cur, a0, vx, vy, $"SPEED {a0} {(sbyte)c.Arguments[1]} {(sbyte)c.Arguments[2]}"); break;
                    case CommandType.GO_STATE: main = a0; sub = 0; Timed(cur, 0, 0, 0, $"STATE {a0}"); res.Segs[^1].T1 = res.Segs[^1].T0; break;
                    case CommandType.GO_SUBSTATE: sub = a0; Timed(cur, 0, 0, 0, $"SUBSTATE {a0}"); res.Segs[^1].T1 = res.Segs[^1].T0; break;
                    case CommandType.GO_WAITSTATE:
                        {
                            int fc = Math.Max(1, doc.FrameCount(d, main, sub)); var st = doc.StateOf(d, main, sub); int sp = st != null ? Math.Max(1, st.AnimationSpeed & 0x0F) : 4;
                            Timed(cur, fc * sp * Math.Max(1, a0), 0, 0, $"WAITSTATE {a0}"); break;
                        }
                    case CommandType.GO_X: x = c.Arguments[0] * 100 + c.Arguments[1]; break;
                    case CommandType.GO_Y: y = c.Arguments[0] * 100 + c.Arguments[1]; break;
                    case CommandType.GO_TEST: test = testValue; break;
                    case CommandType.GO_SETTEST: test = a0 == 1; break;
                    case CommandType.GO_PREPARELOOP: loops.Push((pc, a0)); break;
                    case CommandType.GO_DOLOOP:
                        if (loops.Count > 0) { var l = loops.Pop(); if (l.left - 1 > 0) { loops.Push((l.start, l.left - 1)); pc = l.start; } }
                        break;
                    case CommandType.GO_RETURN: if (calls.Count > 0) pc = calls.Pop(); else { res.Note = "RETURN without GOSUB - end."; pc = cmds.Length; } break;
                    case CommandType.GO_GOTO: case CommandType.RESERVED_GO_GOTO: case CommandType.RESERVED_GO_SKIP:
                    case CommandType.GO_GOSUB: case CommandType.RESERVED_GO_GOSUB:
                    case CommandType.GO_BRANCHTRUE: case CommandType.RESERVED_GO_GOTOT: case CommandType.RESERVED_GO_SKIPT:
                    case CommandType.GO_BRANCHFALSE: case CommandType.RESERVED_GO_GOTOF: case CommandType.RESERVED_GO_SKIPF:
                        {
                            bool needT = ct is CommandType.GO_BRANCHTRUE or CommandType.RESERVED_GO_GOTOT or CommandType.RESERVED_GO_SKIPT;
                            bool needF = ct is CommandType.GO_BRANCHFALSE or CommandType.RESERVED_GO_GOTOF or CommandType.RESERVED_GO_SKIPF;
                            if ((needT && !test) || (needF && test)) break;
                            int tg = JumpTarget(c); if (tg < 0) { res.Note = $"Jump target of command {cur + 1} not found."; pc = cmds.Length; break; }
                            if (ct is CommandType.GO_GOSUB or CommandType.RESERVED_GO_GOSUB) calls.Push(pc);
                            pc = tg; break;
                        }
                    case CommandType.GO_SKIP: case CommandType.GO_SKIPTRUE: case CommandType.GO_SKIPFALSE:
                        if ((ct == CommandType.GO_SKIPTRUE && !test) || (ct == CommandType.GO_SKIPFALSE && test)) break; pc += a0; break;
                    case CommandType.INVALID_CMD: case CommandType.INVALID_CMD_DEMO: pc = 0; if (res.Segs.Count == 0 || t == res.Segs[^1].T1 && exec > 2000) { res.Note = "Script consumes no time."; pc = cmds.Length; } break;
                    default: break; // LABEL, ADD
                }
                if (res.Segs.Count == 0 && exec > 200) { res.Note = "No time-consuming commands reachable."; break; }
            }
            res.TotalTicks = t; res.Truncated = t >= maxTicks || exec >= maxExec;
            if (res.Segs.Count > 0 && res.Segs[^1].T1 > res.TotalTicks) res.TotalTicks = res.Segs[^1].T1;
            return res;
        }
    }
}
