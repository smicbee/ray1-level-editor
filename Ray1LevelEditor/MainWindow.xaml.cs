using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BinarySerializer.Ray1;

namespace Ray1LevelEditor
{
    public partial class MainWindow : Window
    {
        // Workspace = TEST COPY of the game. Originals in "C:\GOG Games\Rayman Forever" are never written.
        public static string DefaultRoot = @"C:\GOG Games\Rayman Forever - Editor Test\Rayman";
        string root; LevelDoc doc; bool dirty;
        double zoom = 1; WriteableBitmap mapBmp; int mapPw, mapPh;
        readonly Stack<Snapshot> undo = new Stack<Snapshot>(), redo = new Stack<Snapshot>();
        ushort curTile = 1; EObj sel; bool dragging; int dragDX, dragDY; bool paintActive; bool loading;
        readonly HashSet<EObj> multi = new HashSet<EObj>();

        static readonly string[] ObjProps = { "Type", "XPosition", "YPosition", "InitialXPosition", "InitialYPosition", "PCPacked_SpritesIndex", "PCPacked_AnimationsIndex", "PCPacked_ImageBufferIndex", "PCPacked_ETAIndex",
            "MainEtat", "SubEtat", "InitialMainEtat", "InitialSubEtat", "HitPoints", "InitialHitPoints", "HitSprite", "OffsetBX", "OffsetBY", "OffsetHY", "FollowSprite", "DisplayPrio", "Timer", "DetectZone", "DetectZoneFlag",
            "Command", "ChangeAnimationMode", "ActiveFlag", "SpeedX", "SpeedY", "Scale", "GravityValue1", "GravityValue2", "AnimationIndex", "AnimationFrame", "FollowX", "FollowY", "RaymanDistance",
            "FlipX", "ReadCommands", "IsLinked", "IsActive", "IsAlive", "FollowEnabled", "CommandTest", "UnknownFlag1", "UnknownFlag2", "UnknownFlag3", "UnknownFlag4", "UnknownFlag5",
            "SpritesCount", "AnimationsCount", "CommandsCount", "CommandOffset", "EDU_ExtHitPoints", "Short_3A", "Short_4A", "Short_50", "Short_52", "IFramesTimer", "TestBlockIndex", "ActiveTimer", "Byte_7D", "InitFlag", "CommandContextDepth", "Id", "ScreenXPosition", "ScreenYPosition" };

        public MainWindow()
        {
            InitializeComponent();
            root = Environment.GetEnvironmentVariable("R1ED_ROOT") ?? DefaultRoot;
            cbWorld.ItemsSource = Names.World.Select((n, i) => $"{i + 1} {n}").ToList(); cbWorld.SelectedIndex = 0;
            cbWorld.SelectionChanged += (s, e) => FillLevels();
                        cbType.ItemsSource = Enum.GetValues(typeof(BlockType)).Cast<BlockType>().Select(b => $"{(int)b} {b}").ToList(); cbType.SelectedIndex = 15;
            FillLevels();
            Loaded += (s, e) =>
            {
                var a = Environment.GetCommandLineArgs();
                if (a.Length >= 3) { cbWorld.SelectedIndex = int.Parse(a[1]) - 1; FillLevels(); cbLevel.SelectedItem = int.Parse(a[2]); }
                OnOpen(null, null);
                TestHooks();
            };
            KeyDown += (s, e) =>
            {
                if (e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control) OnUndo(null, null);
                else if (e.Key == Key.Y && Keyboard.Modifiers == ModifierKeys.Control) OnRedo(null, null);
                else if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control) OnSave(null, null);
                else if (e.Key == Key.Delete && !(Keyboard.FocusedElement is TextBox)) OnDel(null, null);
                else if (Keyboard.Modifiers == ModifierKeys.Control && (e.Key == Key.OemPlus || e.Key == Key.Add)) { OnZoomIn(null, null); e.Handled = true; }
                else if (Keyboard.Modifiers == ModifierKeys.Control && (e.Key == Key.OemMinus || e.Key == Key.Subtract)) { OnZoomOut(null, null); e.Handled = true; }
                else if (Keyboard.Modifiers == ModifierKeys.Control && (e.Key == Key.D0 || e.Key == Key.NumPad0)) { OnZoom100(null, null); e.Handled = true; }
                else if (Keyboard.Modifiers == ModifierKeys.Control && (e.Key == Key.D9 || e.Key == Key.NumPad9)) { OnZoomFit(null, null); e.Handled = true; }
            };
            PreviewKeyDown += (s, e) => { if (e.Key == Key.Space && !(Keyboard.FocusedElement is TextBox) && !spaceDown) { spaceDown = true; sv.Cursor = Cursors.Hand; e.Handled = true; } };
            PreviewKeyUp += (s, e) => { if (e.Key == Key.Space) { spaceDown = false; if (!panning) sv.Cursor = null; } };
            sv.PreviewMouseWheel += OnSvWheel;
            sv.PreviewMouseDown += OnSvDown; sv.PreviewMouseMove += OnSvMove; sv.PreviewMouseUp += OnSvUp;
            pvTimer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) }; pvTimer.Tick += OnPvTick;
            imgMap.MouseLeftButtonDown += MapDown; imgMap.MouseMove += MapMove; imgMap.MouseLeftButtonUp += MapUp; imgMap.MouseRightButtonDown += MapRight;
            host.MouseLeftButtonDown += MapDown; host.MouseMove += MapMove; host.MouseLeftButtonUp += MapUp;
            Closing += (s, e) => { if (dirty && MessageBox.Show("Discard unsaved changes?", "Exit", MessageBoxButton.YesNo) != MessageBoxResult.Yes) e.Cancel = true; };
        }

        // Optional automation hooks for screenshot verification (env vars; no effect when unset)
        void TestHooks()
        {
            string E(string n) => Environment.GetEnvironmentVariable(n);
            if (E("R1ED_BGMODE") != null) cbBgMode.SelectedIndex = int.Parse(E("R1ED_BGMODE"));
            if (E("R1ED_FOND") != null) { doc.Fond = byte.Parse(E("R1ED_FOND")); Redraw(); }
            if (E("R1ED_COLL") == "1") chColl.IsChecked = true;
            if (E("R1ED_ZOOM") != null) SetZoom(double.Parse(E("R1ED_ZOOM"), System.Globalization.CultureInfo.InvariantCulture) / 100);
            if (E("R1ED_SEL") != null) { int i = int.Parse(E("R1ED_SEL")); sel = doc.Objs[i]; ShowSel(); DrawOverlay(); }
            if (E("R1ED_SCROLL") != null) { var p = E("R1ED_SCROLL").Split(','); sv.UpdateLayout(); sv.ScrollToHorizontalOffset(double.Parse(p[0]) * zoom); sv.ScrollToVerticalOffset(double.Parse(p[1]) * zoom); }
            if (E("R1ED_PVT") != null) { pvT = double.Parse(E("R1ED_PVT")); PvUpdateAnim(); }
            if (E("R1ED_FIT") == "1") OnZoomFit(null, null);
        }

        void FillLevels()
        {
            int w = cbWorld.SelectedIndex + 1; string dir = System.IO.Path.Combine(root, "PCMAP", Names.World[w - 1]);
            if (!Directory.Exists(dir)) { cbLevel.ItemsSource = null; return; }
            cbLevel.ItemsSource = Directory.GetFiles(dir, "RAY*.LEV").Select(f => int.Parse(System.IO.Path.GetFileNameWithoutExtension(f).Substring(3))).OrderBy(x => x).ToList(); cbLevel.SelectedIndex = 0;
        }

        void Status(string s) => status.Text = s;

        void OnOpen(object s, RoutedEventArgs e)
        {
            if (cbLevel.SelectedItem == null) { Status("No level found in " + root); return; }
            if (dirty && MessageBox.Show("Discard unsaved changes?", "Open", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            try
            {
                doc = LevelDoc.Open(root, cbWorld.SelectedIndex + 1, (int)cbLevel.SelectedItem);
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString(), "Loading failed"); return; }
            undo.Clear(); redo.Clear(); dirty = false; sel = null; multi.Clear();
            BuildTileSheet(); BuildLevelPanel(); BuildPal(); BuildTemplates(); Redraw(); ShowSel();
            Title = $"Rayman 1 Level Editor – {doc.Rel}"; Status($"{doc.W}x{doc.H} tiles, {doc.Objs.Count} objects, {doc.TileCount} tiles in set. Working copy: {root}");
        }

        void OnSave(object s, RoutedEventArgs e)
        {
            if (doc == null) return;
            if (tbCmds.IsKeyboardFocused) OnCmds(null, null);
            try { doc.Save(); dirty = false; Status("Saved: " + System.IO.Path.Combine(root, doc.Rel)); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Saving failed"); }
        }

        void OnTest(object s, RoutedEventArgs e)
        {
            if (doc == null) return;
            OnSave(null, null);
            string gameDir = System.IO.Path.GetDirectoryName(root);
            string dbox = System.IO.Path.Combine(gameDir, "DOSBOX", "DOSBox.exe");
            if (!File.Exists(dbox)) { MessageBox.Show("DOSBox not found: " + dbox); return; }
            // Start Rayman directly with the level-select cheat-less approach: just boot the test copy's game menu.
            var psi = new ProcessStartInfo(dbox, "-conf \"..\\dosboxRayman.conf\" -conf \"..\\dosboxRayman_single.conf\" -noconsole") { WorkingDirectory = System.IO.Path.Combine(gameDir, "DOSBOX"), UseShellExecute = false };
            psi.EnvironmentVariables["SDL_VIDEODRIVER"] = "windib";
            Process.Start(psi);
            Status($"DOSBox (test copy) started. Select level {doc.Names()} in the game.");
        }

        // ---------- undo ----------
        void Push() { undo.Push(doc.Snap()); redo.Clear(); dirty = true; }
        void OnUndo(object s, RoutedEventArgs e) { if (undo.Count == 0) return; redo.Push(doc.Snap()); doc.Restore(undo.Pop()); sel = null; multi.Clear(); Redraw(); ShowSel(); BuildLevelPanel(); BuildPal(); }
        void OnRedo(object s, RoutedEventArgs e) { if (redo.Count == 0) return; undo.Push(doc.Snap()); doc.Restore(redo.Pop()); sel = null; multi.Clear(); Redraw(); ShowSel(); BuildLevelPanel(); BuildPal(); }

        // ---------- map drawing ----------
        void OnView(object s, RoutedEventArgs e) { if (doc != null) Redraw(); }
        // ---------- zoom / pan ----------
        static readonly double[] ZoomSteps = { 0.25, 0.33, 0.5, 0.67, 0.75, 1, 1.5, 2, 3, 4, 5, 6, 8 };
        bool zoomUi, spaceDown, panning; Point panStart; double panH, panV;
        void OnZoomIn(object s, RoutedEventArgs e) => SetZoom(ZoomSteps.FirstOrDefault(z => z > zoom + 1e-6) is double n && n > 0 ? n : 8);
        void OnZoomOut(object s, RoutedEventArgs e) => SetZoom(ZoomSteps.LastOrDefault(z => z < zoom - 1e-6) is double n && n > 0 ? n : 0.25);
        void OnZoom100(object s, RoutedEventArgs e) => SetZoom(1);
        void OnZoomFit(object s, RoutedEventArgs e)
        {
            if (mapBmp == null) return;
            SetZoom(Math.Min((sv.ViewportWidth - 2) / mapPw, (sv.ViewportHeight - 2) / mapPh)); sv.ScrollToHorizontalOffset(0); sv.ScrollToVerticalOffset(0);
        }
        void OnZoomSlider(object s, RoutedPropertyChangedEventArgs<double> e) { if (!zoomUi && tbZoom != null && IsLoaded) SetZoom(slZoom.Value / 100.0); }
        void OnSvWheel(object s, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control) return; e.Handled = true;
            double n = e.Delta > 0 ? (ZoomSteps.FirstOrDefault(z => z > zoom + 1e-6) is double a && a > 0 ? a : 8) : (ZoomSteps.LastOrDefault(z => z < zoom - 1e-6) is double b && b > 0 ? b : 0.25);
            SetZoom(n, e.GetPosition(sv));
        }
        /// <summary>Zoom 0.25..8; keeps the map point under <paramref name="anchor"/> (viewport coordinates) fixed.</summary>
        void SetZoom(double z, Point? anchor = null)
        {
            z = Math.Max(0.25, Math.Min(8, z));
            zoomUi = true; slZoom.Value = z * 100; tbZoom.Text = Math.Round(z * 100) + "%"; zoomUi = false;
            if (Math.Abs(z - zoom) < 1e-9) return;
            var a = anchor ?? new Point(sv.ViewportWidth / 2, sv.ViewportHeight / 2);
            double mx = (sv.HorizontalOffset + a.X) / zoom, my = (sv.VerticalOffset + a.Y) / zoom;
            zoom = z; if (doc == null || mapBmp == null) return;
            ApplyZoom(); sv.UpdateLayout();
            sv.ScrollToHorizontalOffset(mx * zoom - a.X); sv.ScrollToVerticalOffset(my * zoom - a.Y);
        }
        void OnSvDown(object s, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle || (e.ChangedButton == MouseButton.Left && spaceDown))
            { panning = true; panStart = e.GetPosition(sv); panH = sv.HorizontalOffset; panV = sv.VerticalOffset; sv.CaptureMouse(); sv.Cursor = Cursors.Hand; e.Handled = true; }
        }
        void OnSvMove(object s, MouseEventArgs e)
        {
            if (!panning) return; var p = e.GetPosition(sv);
            sv.ScrollToHorizontalOffset(panH - (p.X - panStart.X)); sv.ScrollToVerticalOffset(panV - (p.Y - panStart.Y)); e.Handled = true;
        }
        void OnSvUp(object s, MouseButtonEventArgs e) { if (!panning) return; panning = false; sv.ReleaseMouseCapture(); sv.Cursor = spaceDown ? Cursors.Hand : null; e.Handled = true; }

        // ---------- level background (VIGNET.DAT PCX; Fond = base image, ScrollDiff Fond = parallax layer with index 0 transparent) ----------
        BitmapSource bgBmp; int bgW, bgH; ImageBrush bgBrush;
        void OnBgToggle(object s, RoutedEventArgs e) { if (doc != null && IsLoaded) Redraw(); }
        void OnBgMode(object s, SelectionChangedEventArgs e) { if (doc != null) ApplyBg(); }
        void OnSvScroll(object s, ScrollChangedEventArgs e) { if (cbBgMode != null && cbBgMode.SelectedIndex > 0) UpdateBgCam(); }

        void ComputeBg()
        {
            bgBmp = null; bgBrush = null; doc.TransparentEmpty = false;
            if (chBg.IsChecked != true) return;
            var b1 = doc.Background(doc.Fond); var b2 = chBg2.IsChecked == true ? doc.Background(doc.Fond2) : null;
            if (b1 == null && b2 == null) { Status("Background image not loaded" + (doc.BgError != null ? ": " + doc.BgError : "")); return; }
            bgW = Math.Max(b1?.W ?? 0, b2?.W ?? 0); bgH = Math.Max(b1?.H ?? 0, b2?.H ?? 0); var buf = new byte[bgW * bgH * 4];
            void Blit(LevelDoc.Pcx p, bool trans)
            {
                if (p == null) return; var src = LevelDoc.PcxBgra(p, trans);
                for (int y = 0; y < p.H; y++) for (int x = 0; x < p.W; x++) { int si = (y * p.W + x) * 4, di = (y * bgW + x) * 4; if (src[si + 3] == 0) continue; Array.Copy(src, si, buf, di, 4); }
            }
            Blit(b1, false); Blit(b2, true);
            var bmp = new WriteableBitmap(bgW, bgH, 96, 96, PixelFormats.Bgra32, null); bmp.WritePixels(new Int32Rect(0, 0, bgW, bgH), buf, bgW * 4, 0); bmp.Freeze(); bgBmp = bmp;
            bgBrush = new ImageBrush(bmp) { TileMode = TileMode.Tile, ViewportUnits = BrushMappingMode.Absolute, Stretch = Stretch.Fill, AlignmentX = AlignmentX.Left, AlignmentY = AlignmentY.Top };
            RenderOptions.SetBitmapScalingMode(bgBrush, BitmapScalingMode.NearestNeighbor);
            doc.TransparentEmpty = true;
        }

        void ApplyBg()
        {
            bgLvl.Visibility = bgCam.Visibility = Visibility.Collapsed; if (bgBrush == null || mapBmp == null) return;
            if (cbBgMode.SelectedIndex == 0)
            {
                bgLvl.Width = mapPw * zoom; bgLvl.Height = mapPh * zoom; bgBrush.Viewport = new Rect(0, 0, bgW * zoom, bgH * zoom); bgLvl.Fill = bgBrush; bgLvl.Visibility = Visibility.Visible;
            }
            else { bgCam.Fill = bgBrush; bgCam.Visibility = Visibility.Visible; UpdateBgCam(); }
        }

        void UpdateBgCam()
        {
            if (bgBrush == null || cbBgMode.SelectedIndex == 0) return;
            double f = cbBgMode.SelectedIndex == 2 ? 0.25 : 0; double w = bgW * zoom, h = bgH * zoom;
            double ox = -((sv.HorizontalOffset * f) % w), oy = -((sv.VerticalOffset * f) % h);
            bgBrush.Viewport = new Rect(ox, oy, w, h);
        }

        void Redraw()
        {
            if (doc == null) return;
            ComputeBg();
            var buf = doc.RenderMap(chColl.IsChecked == true, chObj.IsChecked == true, out mapPw, out mapPh);
            mapBmp = new WriteableBitmap(mapPw, mapPh, 96, 96, PixelFormats.Bgra32, null);
            mapBmp.WritePixels(new Int32Rect(0, 0, mapPw, mapPh), buf, mapPw * 4, 0);
            imgMap.Source = mapBmp; imgMap.Stretch = Stretch.Fill; ApplyZoom();
        }

        void ApplyZoom()
        {
            imgMap.Width = mapPw * zoom; imgMap.Height = mapPh * zoom;
            host.Width = imgMap.Width; host.Height = imgMap.Height; DrawOverlay(); ApplyBg(); DrawPreview();
        }

        void DrawOverlay()
        {
            overlay.Children.Clear(); if (doc == null) return;
            overlay.Width = host.Width; overlay.Height = host.Height;
            foreach (var o in doc.Objs)
            {
                var b = doc.Bounds(o); bool isSel = o == sel || multi.Contains(o);
                var r = new Rectangle { Width = Math.Max(4, b.w * zoom), Height = Math.Max(4, b.h * zoom), Stroke = isSel ? Brushes.Yellow : (o.Link != 0 ? Brushes.Cyan : Brushes.LimeGreen), StrokeThickness = isSel ? 2 : 1, Opacity = isSel ? 1 : 0.55 };
                Canvas.SetLeft(r, b.x * zoom); Canvas.SetTop(r, b.y * zoom); overlay.Children.Add(r);
            }
            foreach (var g in doc.Objs.Where(o => o.Link != 0).GroupBy(o => o.Link))
            {
                var l = g.ToList();
                for (int i = 0; i < l.Count; i++)
                {
                    var a = l[i]; var c = l[(i + 1) % l.Count]; if (l.Count == 2 && i == 1) break;
                    var ba = doc.Bounds(a); var bc = doc.Bounds(c);
                    overlay.Children.Add(new Line { X1 = (ba.x + ba.w / 2) * zoom, Y1 = (ba.y + ba.h / 2) * zoom, X2 = (bc.x + bc.w / 2) * zoom, Y2 = (bc.y + bc.h / 2) * zoom, Stroke = Brushes.Cyan, StrokeThickness = 1, StrokeDashArray = new DoubleCollection { 3, 3 } });
                }
            }
        }

        (int x, int y) MapPos(MouseEventArgs e) { var p = e.GetPosition(imgMap); return ((int)(p.X / zoom), (int)(p.Y / zoom)); }

        EObj HitObj(int x, int y)
        {
            for (int i = doc.Objs.Count - 1; i >= 0; i--) { var b = doc.Bounds(doc.Objs[i]); if (x >= b.x && y >= b.y && x < b.x + Math.Max(8, b.w) && y < b.y + Math.Max(8, b.h)) return doc.Objs[i]; }
            return null;
        }

        void MapDown(object s, MouseButtonEventArgs e)
        {
            if (doc == null) return; var (x, y) = MapPos(e); int cx = x / 16, cy = y / 16;
            if (rSel.IsChecked == true)
            {
                var o = HitObj(x, y);
                if (Keyboard.Modifiers == ModifierKeys.Shift && o != null) { if (!multi.Add(o)) multi.Remove(o); if (sel != null) multi.Add(sel); }
                else if (!(o != null && multi.Contains(o))) multi.Clear();
                sel = o; ShowSel(); DrawOverlay();
                if (o != null) { Push(); dragging = true; dragDX = x - o.D.XPosition; dragDY = y - o.D.YPosition; imgMap.CaptureMouse(); }
                else { Status($"Cell {cx},{cy}: tile {doc.Cells[Math.Min(cy, doc.H - 1) * doc.W + Math.Min(cx, doc.W - 1)].Tile}"); }
            }
            else if (cx >= 0 && cy >= 0 && cx < doc.W && cy < doc.H)
            {
                if (rPick.IsChecked == true) { var c = doc.Cells[cy * doc.W + cx]; curTile = c.Tile; tbTile.Text = "Tile: " + curTile; cbType.SelectedIndex = TypeIndex(c.Type); return; }
                Push(); paintActive = true; imgMap.CaptureMouse(); Paint(cx, cy);
            }
        }

        int TypeIndex(int t) { for (int i = 0; i < cbType.Items.Count; i++) if (((string)cbType.Items[i]).StartsWith(t + " ")) return i; return 0; }
        int SelType() => int.Parse(((string)cbType.SelectedItem).Split(' ')[0]);

        void Paint(int cx, int cy)
        {
            if (cx < 0 || cy < 0 || cx >= doc.W || cy >= doc.H) return;
            var c = doc.Cells[cy * doc.W + cx];
            if (rTile.IsChecked == true) { c.Tile = curTile; c.Render = doc.RenderModeFor(curTile); }
            else c.Type = (byte)SelType();
            doc.Cells[cy * doc.W + cx] = c;
            var buf = new byte[16 * 16 * 4]; var full = new byte[mapPw * 4 * 16]; // re-render this cell only
            var tmp = new byte[mapPw * mapPh * 4];
            doc.RenderCell(tmp, mapPw * 4, cx, cy, chColl.IsChecked == true);
            var row = new byte[16 * 16 * 4];
            for (int y = 0; y < 16; y++) Array.Copy(tmp, ((cy * 16 + y) * mapPw + cx * 16) * 4, row, y * 64, 64);
            mapBmp.WritePixels(new Int32Rect(cx * 16, cy * 16, 16, 16), row, 64, 0);
            // objects on top of the cell are redrawn lazily on next Redraw
        }

        void MapMove(object s, MouseEventArgs e)
        {
            if (doc == null) return; var (x, y) = MapPos(e);
            if (dragging && sel != null)
            {
                int nx = x - dragDX, ny = y - dragDY; if (Keyboard.Modifiers == ModifierKeys.Control) { nx = nx / 16 * 16; ny = ny / 16 * 16; }
                int ddx = nx - sel.D.XPosition, ddy = ny - sel.D.YPosition;
                foreach (var o in multi.Count > 0 ? multi.Union(new[] { sel }) : new[] { sel }) Move(o, ddx, ddy);
                DrawOverlay(); UpdateProps();
            }
            else if (paintActive) Paint(x / 16, y / 16);
            Status($"x={x} y={y}  cell {x / 16},{y / 16}");
        }
        static void Move(EObj o, int dx, int dy) { o.D.XPosition += dx; o.D.YPosition += dy; o.D.InitialXPosition = (short)o.D.XPosition; o.D.InitialYPosition = (short)o.D.YPosition; }
        void MapUp(object s, MouseButtonEventArgs e) { bool was = dragging || paintActive; dragging = false; paintActive = false; imgMap.ReleaseMouseCapture(); if (was) Redraw(); }
        void MapRight(object s, MouseButtonEventArgs e) { if (doc == null) return; var (x, y) = MapPos(e); int cx = x / 16, cy = y / 16; if (cx < doc.W && cy < doc.H) { curTile = doc.Cells[cy * doc.W + cx].Tile; tbTile.Text = "Tile: " + curTile; } }

        // ---------- tile sheet ----------
        void BuildTileSheet()
        {
            int n = doc.TileCount, cols = 16, rows = (n + cols - 1) / cols; int w = cols * 17, h = rows * 17; var buf = new byte[w * h * 4];
            for (int i = 0; i < n; i++)
            {
                var t = doc.TileBgra((ushort)i); int ox = (i % cols) * 17, oy = (i / cols) * 17;
                for (int y = 0; y < 16; y++) for (int x = 0; x < 16; x++)
                    {
                        int s = (y * 16 + x) * 4, d = ((oy + y) * w + ox + x) * 4;
                        if (t[s + 3] != 0) { buf[d] = t[s]; buf[d + 1] = t[s + 1]; buf[d + 2] = t[s + 2]; } else { buf[d] = 0x50; buf[d + 1] = 0x30; buf[d + 2] = 0x20; }
                        buf[d + 3] = 255;
                    }
            }
            var bmp = new WriteableBitmap(w, h, 96, 96, PixelFormats.Bgra32, null); bmp.WritePixels(new Int32Rect(0, 0, w, h), buf, w * 4, 0); imgTiles.Source = bmp;
        }
        void OnTilePick(object s, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(imgTiles); int i = (int)(p.Y / 17) * 16 + (int)(p.X / 17); if (i < doc.TileCount) { curTile = (ushort)i; tbTile.Text = "Tile: " + i; if (rSel.IsChecked == true) rTile.IsChecked = true; }
        }

        // ---------- object panel ----------
        static readonly Dictionary<string, PropertyInfo> pi = typeof(ObjData).GetProperties().ToDictionary(p => p.Name);
        readonly Dictionary<string, TextBox> propBoxes = new Dictionary<string, TextBox>();
        readonly Dictionary<string, CheckBox> propChecks = new Dictionary<string, CheckBox>();
        readonly Dictionary<string, ComboBox> propCombos = new Dictionary<string, ComboBox>();

        void ShowSel()
        {
            props.Children.Clear(); propBoxes.Clear(); propChecks.Clear(); propCombos.Clear();
            if (sel == null) { tbSel.Text = "No object selected. Click an object (Shift = multiple), drag to move (Ctrl = grid)."; tbCmds.Text = ""; PvRebuild(); return; }
            loading = true;
            var dark = new SolidColorBrush(Color.FromRgb(0x2B, 0x2B, 0x2B)); var fg = Brushes.White; var dim = new SolidColorBrush(Color.FromRgb(0xA8, 0xB4, 0xC0));
            foreach (var g in ObjFields.Groups)
            {
                var fields = ObjFields.All.Where(f => f.Group == g && pi.ContainsKey(f.Name)).ToList();
                bool isFlags = g == ObjFields.GFlags;
                if (fields.Count == 0 && !isFlags) continue;
                var body = new StackPanel { Background = dark };
                if (g == ObjFields.GInt) body.Children.Add(new TextBlock { Text = "Warning: values the engine sets itself at runtime, plus counters/offsets. Do not edit by hand unless you know exactly what you are doing. Grey = read-only.", TextWrapping = TextWrapping.Wrap, Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xD0, 0x80)), FontSize = 11, Margin = new Thickness(4, 2, 4, 4) });
                foreach (var f in fields)
                {
                    var p = pi[f.Name];
                    var row = new DockPanel { Margin = new Thickness(2), Background = dark };
                    row.ToolTip = new ToolTip { Content = new TextBlock { Text = f.Name + (f.Runtime ? "  [runtime value]" : "") + "\n" + f.Tip, TextWrapping = TextWrapping.Wrap, MaxWidth = 380 } };
                    ToolTipService.SetShowDuration(row, 30000);
                    var lbl = new TextBlock { Width = 215, Foreground = fg, TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center };
                    lbl.Inlines.Add(new System.Windows.Documents.Run(f.Label)); lbl.Inlines.Add(new System.Windows.Documents.Run("  " + f.Name) { Foreground = dim, FontSize = 10 });
                    row.Children.Add(lbl);
                    if (p.PropertyType == typeof(bool)) { var cb = new CheckBox { VerticalAlignment = VerticalAlignment.Center, IsEnabled = !f.ReadOnly }; cb.Click += (s, e) => { if (loading) return; Push(); p.SetValue(sel.D, cb.IsChecked == true); Redraw(); }; propChecks[f.Name] = cb; row.Children.Add(cb); }
                    else if (p.PropertyType.IsEnum)
                    {
                        var cm = new ComboBox { ItemsSource = Enum.GetNames(p.PropertyType), IsEnabled = !f.ReadOnly }; cm.SelectionChanged += (s, e) => { if (loading || cm.SelectedItem == null) return; Push(); p.SetValue(sel.D, Enum.Parse(p.PropertyType, (string)cm.SelectedItem)); Redraw(); };
                        propCombos[f.Name] = cm; row.Children.Add(cm);
                    }
                    else { var tb = new TextBox { Width = 90, HorizontalAlignment = HorizontalAlignment.Left, IsReadOnly = f.ReadOnly }; if (f.ReadOnly) { tb.Background = Brushes.LightGray; tb.Foreground = Brushes.DimGray; } tb.LostFocus += (s, e) => EditProp(p, tb); propBoxes[f.Name] = tb; row.Children.Add(tb); }
                    body.Children.Add(row);
                }
                if (isFlags)
                {
                    var lrow = new DockPanel { Margin = new Thickness(2), Background = dark, ToolTip = "Objects with the same group number are activated/restarted together by the game (link chain). 0 = none." };
                    lrow.Children.Add(new TextBlock { Text = "Link group (0 = none)", Width = 215, Foreground = fg, VerticalAlignment = VerticalAlignment.Center });
                    var lk = new TextBox { Width = 60, HorizontalAlignment = HorizontalAlignment.Left, Text = sel.Link.ToString() };
                    lk.LostFocus += (s, e) => { if (int.TryParse(lk.Text, out int v) && v != sel.Link) { Push(); sel.Link = Math.Max(0, v); sel.D.IsLinked = v != 0; DrawOverlay(); } };
                    lrow.Children.Add(lk); body.Children.Add(lrow);
                }
                var ex = new Expander { Header = new TextBlock { Text = g, Foreground = Brushes.White, FontWeight = FontWeights.Bold }, Foreground = Brushes.White, IsExpanded = g != ObjFields.GInt, Content = body, Margin = new Thickness(0, 1, 0, 1), Background = dark };
                props.Children.Add(ex);
            }
            loading = false; UpdateProps(); PvRebuild();
        }

        void UpdateProps()
        {
            if (sel == null) return; loading = true;
            int idx = doc.Objs.IndexOf(sel);
            tbSel.Text = $"#{idx}  {sel.D.Type}  ({sel.D.XPosition},{sel.D.YPosition})" + (multi.Count > 1 ? $"  [{multi.Count} selected]" : "");
            foreach (var kv in propBoxes) { var v = pi[kv.Key].GetValue(sel.D); if (!kv.Value.IsKeyboardFocused) kv.Value.Text = Convert.ToString(v); }
            foreach (var kv in propChecks) kv.Value.IsChecked = (bool)pi[kv.Key].GetValue(sel.D);
            foreach (var kv in propCombos) kv.Value.SelectedItem = pi[kv.Key].GetValue(sel.D).ToString();
            if (!tbCmds.IsKeyboardFocused) { tbCmds.Text = CmdsToText(sel); tbErr.Text = ""; }
            loading = false;
        }

        void EditProp(PropertyInfo p, TextBox tb)
        {
            if (loading || sel == null) return;
            try
            {
                var cur = p.GetValue(sel.D); object nv = Convert.ChangeType(tb.Text, Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType);
                if (Equals(cur, nv)) return; Push(); p.SetValue(sel.D, nv);
                if (p.Name == "XPosition") sel.D.InitialXPosition = (short)sel.D.XPosition; if (p.Name == "YPosition") sel.D.InitialYPosition = (short)sel.D.YPosition;
                doc.ClearCaches(); Redraw(); UpdateProps();
            }
            catch { Status($"Invalid value for {p.Name} ({p.PropertyType.Name})"); tb.Text = Convert.ToString(p.GetValue(sel.D)); }
        }

        static string CmdsToText(EObj o) => ScriptText.ToText(o.Cmds, o.Labels);

        void OnCmds(object s, RoutedEventArgs e)
        {
            if (sel == null || loading) return;
            var r = ScriptText.Parse(tbCmds.Text);
            if (r.Errors.Count > 0) { tbErr.Text = string.Join("\n", r.Errors.Take(6)); Status("Invalid script: " + r.Errors[0]); return; }
            tbErr.Text = "";
            bool same = r.Labels.SequenceEqual(sel.Labels) && r.Cmds.Length == sel.Cmds.Length && r.Cmds.Zip(sel.Cmds, (x, y) => x.CommandType == y.CommandType && x.Arguments.SequenceEqual(y.Arguments)).All(v => v);
            if (same) return;
            Push(); sel.Cmds = r.Cmds; sel.Labels = r.Labels; sel.D.CommandsCount = (short)sel.Cmds.Sum(c => c.Arguments.Length + 1);
            Status($"Commands applied ({r.Cmds.Length} commands, {r.Labels.Length} label offsets recomputed)"); PvRebuild();
        }

        // ---------- object ops ----------
        void OnDup(object s, RoutedEventArgs e)
        {
            if (sel == null) return; Push(); var c = sel.Clone(); c.D.XPosition += 16; c.D.YPosition += 16; c.D.InitialXPosition = (short)c.D.XPosition; c.D.InitialYPosition = (short)c.D.YPosition; c.Link = 0; c.D.IsLinked = false;
            doc.Objs.Add(c); sel = c; multi.Clear(); Redraw(); ShowSel();
        }
        void OnDel(object s, RoutedEventArgs e)
        {
            if (sel == null) return; Push(); foreach (var o in multi.Union(new[] { sel }).ToList()) doc.Objs.Remove(o); sel = null; multi.Clear(); Redraw(); ShowSel();
        }
        void BuildTemplates()
        {
            // templates = first object of each (Type) used in this level + one from every other level of this world would need loading; keep to this level
            cbTpl.ItemsSource = doc.Objs.GroupBy(o => o.D.Type).Select(g => new Tpl { O = g.First(), N = $"{g.Key} ({g.Count()}x)" }).ToList(); cbTpl.DisplayMemberPath = "N"; if (cbTpl.Items.Count > 0) cbTpl.SelectedIndex = 0;
        }
        class Tpl { public EObj O; public string N; }
        void OnNew(object s, RoutedEventArgs e)
        {
            var t = cbTpl.SelectedItem as Tpl; if (t == null) return; Push(); var c = t.O.Clone(); c.Link = 0; c.D.IsLinked = false;
            var p = sv.ContentHorizontalOffset; int x = (int)((sv.ContentHorizontalOffset + 200) / zoom), y = (int)((sv.ContentVerticalOffset + 200) / zoom);
            c.D.XPosition = x; c.D.YPosition = y; c.D.InitialXPosition = (short)x; c.D.InitialYPosition = (short)y; doc.Objs.Add(c); sel = c; multi.Clear(); Redraw(); ShowSel();
        }

        // ---------- level panel ----------
        void BuildLevelPanel()
        {
            lvlPanel.Children.Clear();
            void Row(string n, Func<string> get, Action<string> set)
            {
                var d = new DockPanel { Margin = new Thickness(2) }; d.Children.Add(new TextBlock { Text = n, Width = 190, Foreground = Brushes.White });
                var tb = new TextBox { Width = 80, Text = get() }; tb.LostFocus += (s, e) => { try { Push(); set(tb.Text); Redraw(); BuildPal(); } catch { Status("Invalid"); } tb.Text = get(); }; d.Children.Add(tb); lvlPanel.Children.Add(d);
            }
            Row("Width (tiles)", () => doc.W.ToString(), v => doc.Resize(int.Parse(v), doc.H));
            Row("Height (tiles)", () => doc.H.ToString(), v => doc.Resize(doc.W, int.Parse(v)));
            Row("Background (Fond) index", () => doc.Fond.ToString(), v => doc.Fond = byte.Parse(v));
            Row("Parallax background index", () => doc.Fond2.ToString(), v => doc.Fond2 = byte.Parse(v));
            Row("Scroll-diff sprites (DES)", () => doc.ScrollDiff.ToString(), v => doc.ScrollDiff = int.Parse(v));
            Row("LastPlan1Palette", () => doc.Last.ToString(), v => doc.Last = byte.Parse(v));
            lvlPanel.Children.Add(new TextBlock { Text = "Rayman start position: move the TYPE_RAY_POS object in the Object tab.", Margin = new Thickness(2, 10, 2, 0), TextWrapping = TextWrapping.Wrap, Foreground = Brushes.LightGray });
            var rp = new Button { Content = "Jump to RAY_POS", Margin = new Thickness(2, 6, 2, 0), HorizontalAlignment = HorizontalAlignment.Left };
            rp.Click += (s, e) => { var o = doc.Objs.FirstOrDefault(x => x.D.Type == ObjType.TYPE_RAY_POS); if (o != null) { sel = o; ShowSel(); DrawOverlay(); sv.ScrollToHorizontalOffset(o.D.XPosition * zoom - 300); sv.ScrollToVerticalOffset(o.D.YPosition * zoom - 200); } };
            lvlPanel.Children.Add(rp);
        }

        // ---------- palette ----------
        int palSel = 0, palIdx = 0;
        void BuildPal() { cbPal.ItemsSource = Enumerable.Range(1, doc.Pal.Length).Select(i => "Pal " + i).ToList(); cbPal.SelectedIndex = Math.Min(palSel, doc.Pal.Length - 1); DrawPal(); }
        void OnPalSel(object s, SelectionChangedEventArgs e) { if (doc == null || cbPal.SelectedIndex < 0) return; palSel = cbPal.SelectedIndex; DrawPal(); }
        void DrawPal()
        {
            if (doc == null) return; var buf = new byte[16 * 16 * 16 * 16 * 4]; int w = 256;
            for (int i = 0; i < 256; i++) for (int y = 0; y < 16; y++) for (int x = 0; x < 16; x++)
                    { int d = (((i / 16) * 16 + y) * w + (i % 16) * 16 + x) * 4; var p = doc.Pal[palSel]; buf[d] = (byte)(p[i * 3 + 2] * 255 / 63); buf[d + 1] = (byte)(p[i * 3 + 1] * 255 / 63); buf[d + 2] = (byte)(p[i * 3] * 255 / 63); buf[d + 3] = 255; }
            var bmp = new WriteableBitmap(w, w, 96, 96, PixelFormats.Bgra32, null); bmp.WritePixels(new Int32Rect(0, 0, w, w), buf, w * 4, 0); imgPal.Source = bmp; ShowPalEntry();
        }
        void ShowPalEntry() { var p = doc.Pal[palSel]; tbPalIdx.Text = palIdx.ToString(); pR.Text = p[palIdx * 3].ToString(); pG.Text = p[palIdx * 3 + 1].ToString(); pB.Text = p[palIdx * 3 + 2].ToString(); }
        void OnPalPick(object s, MouseButtonEventArgs e) { var p = e.GetPosition(imgPal); palIdx = Math.Min(255, (int)(p.Y / 16) * 16 + (int)(p.X / 16)); ShowPalEntry(); }
        void OnPalEdit(object s, RoutedEventArgs e)
        {
            if (doc == null) return; try { var p = doc.Pal[palSel]; var n = new[] { byte.Parse(pR.Text), byte.Parse(pG.Text), byte.Parse(pB.Text) }; if (n.Any(v => v > 63)) throw new Exception(); if (n.SequenceEqual(p.Skip(palIdx * 3).Take(3))) return; Push(); for (int k = 0; k < 3; k++) p[palIdx * 3 + k] = n[k]; doc.ClearCaches(); DrawPal(); BuildTileSheet(); Redraw(); }
            catch { Status("Palette values 0..63"); ShowPalEntry(); }
        }
    }

    static class DocExt { public static string Names(this LevelDoc d) => $"{Ray1LevelEditor.Names.World[d.World - 1]} {d.LevelNo}"; }
}
