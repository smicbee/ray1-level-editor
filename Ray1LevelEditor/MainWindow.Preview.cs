using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BinarySerializer.Ray1;

namespace Ray1LevelEditor
{
    public partial class MainWindow
    {
        // Script preview: approximation, never writes into the level.
        System.Windows.Threading.DispatcherTimer pvTimer; SimResult pvRes; double pvT; bool pvPlaying;
        const double PvTicksPerSec = 60; // assumption: 1 tick = 1 game cycle, played at 60/s for 1x
        Image pvImg; readonly List<int> pvLines = new List<int>();

        void OnPvOpt(object s, RoutedEventArgs e) { if (doc != null && IsLoaded) PvRebuild(); }

        void PvRebuild()
        {
            pvTimer?.Stop(); pvPlaying = false; if (btPlay != null) btPlay.Content = "▶ Play";
            pvRes = null; pvT = 0;
            if (doc != null && sel != null && sel.Cmds.Length > 0)
                try { pvRes = Sim.Run(doc, sel, cbTest.SelectedIndex == 1); } catch (Exception ex) { tbPv.Text = "Preview error: " + ex.Message; }
            DrawPreview(); PvUpdateAnim();
        }

        void DrawPreview()
        {
            if (pv == null) return; pv.Children.Clear(); pvAnim.Children.Clear(); pvImg = null;
            if (doc == null) return; pv.Width = pvAnim.Width = host.Width; pv.Height = pvAnim.Height = host.Height;
            if (pvRes == null || pvRes.Segs.Count == 0) { tbPv.Text = sel == null ? "Select an object to simulate its script." : (pvRes?.Note ?? "No script."); return; }
            if (chPath.IsChecked != true) return;
            var b = doc.Bounds(sel); double ox = sel.D.XPosition - b.x, oy = sel.D.YPosition - b.y; // object origin -> bounds center offset
            // polyline of the object's reference point (its position); drawn at the object origin + sprite center
            var cxo = b.w / 2.0 + (b.x - sel.D.XPosition); var cyo = b.h / 2.0 + (b.y - sel.D.YPosition);
            var pl = new Polyline { Stroke = Brushes.Orange, StrokeThickness = 2, StrokeLineJoin = PenLineJoin.Round, Opacity = 0.95 };
            var pts = new PointCollection(); var first = pvRes.Segs[0];
            pts.Add(new Point((first.X0 + cxo) * zoom, (first.Y0 + cyo) * zoom));
            foreach (var sg in pvRes.Segs) pts.Add(new Point((sg.XAt(sg.T1) + cxo) * zoom, (sg.YAt(sg.T1) + cyo) * zoom));
            pl.Points = pts; pv.Children.Add(pl);
            int n = 0;
            foreach (var sg in pvRes.Segs)
            {
                if (sg.T1 == sg.T0 && !sg.Desc.StartsWith("STATE") && !sg.Desc.StartsWith("SUBSTATE")) continue;
                n++; double px = (sg.X0 + cxo) * zoom, py = (sg.Y0 + cyo) * zoom;
                var dot = new Ellipse { Width = 8, Height = 8, Fill = sg.T1 == sg.T0 ? Brushes.Magenta : Brushes.Yellow, Stroke = Brushes.Black, StrokeThickness = 1, ToolTip = $"Command {sg.Cmd + 1}: {sg.Desc} (t={sg.T0})" };
                Canvas.SetLeft(dot, px - 4); Canvas.SetTop(dot, py - 4); pv.Children.Add(dot);
                if (zoom >= 0.5) { var tl = new TextBlock { Text = (sg.Cmd + 1).ToString(), Foreground = Brushes.White, FontSize = 9, Background = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0)) }; Canvas.SetLeft(tl, px + 5); Canvas.SetTop(tl, py - 12); pv.Children.Add(tl); }
            }
            var end = pvRes.Segs[^1]; var ed = new Rectangle { Width = 8, Height = 8, Fill = Brushes.Red, Stroke = Brushes.White };
            Canvas.SetLeft(ed, (end.XAt(end.T1) + cxo) * zoom - 4); Canvas.SetTop(ed, (end.YAt(end.T1) + cyo) * zoom - 4); pv.Children.Add(ed);
            pvImg = new Image { Stretch = Stretch.Fill, Opacity = 0.9 }; RenderOptions.SetBitmapScalingMode(pvImg, BitmapScalingMode.NearestNeighbor); pvAnim.Children.Add(pvImg);
        }

        void PvUpdateAnim()
        {
            if (pvRes == null || pvRes.Segs.Count == 0 || pvImg == null) { if (pvRes != null && pvRes.Segs.Count == 0) tbPv.Text = pvRes.Note ?? ""; return; }
            double tt = pvRes.TotalTicks > 0 ? pvT % pvRes.TotalTicks : 0; var sg = pvRes.At(tt); if (sg == null) return;
            double x = sg.XAt(tt), y = sg.YAt(tt);
            int fc = Math.Max(1, doc.FrameCount(sel.D, sg.Main, sg.Sub)); int frame = (int)((tt - sg.T0) / Math.Max(1, sg.AnimSpeed)) % fc;
            var layers = doc.VisualAt(sel.D, sg.Main, sg.Sub, sg.Flip, frame);
            if (layers.Count > 0)
            {
                int x0 = layers.Min(l => l.X), y0 = layers.Min(l => l.Y), x1 = layers.Max(l => l.X + l.W), y1 = layers.Max(l => l.Y + l.H); int w = x1 - x0, h = y1 - y0;
                var buf = new byte[w * h * 4];
                foreach (var l in layers) for (int yy = 0; yy < l.H; yy++) for (int xx = 0; xx < l.W; xx++)
                        { int s = (yy * l.W + xx) * 4; if (l.Bgra[s + 3] == 0) continue; Array.Copy(l.Bgra, s, buf, ((l.Y - y0 + yy) * w + (l.X - x0 + xx)) * 4, 4); }
                var bmp = new WriteableBitmap(w, h, 96, 96, PixelFormats.Bgra32, null); bmp.WritePixels(new Int32Rect(0, 0, w, h), buf, w * 4, 0); pvImg.Source = bmp;
                pvImg.Width = w * zoom; pvImg.Height = h * zoom; Canvas.SetLeft(pvImg, (x + x0) * zoom); Canvas.SetTop(pvImg, (y + y0) * zoom);
            }
            tbPv.Text = $"t={tt:0}/{pvRes.TotalTicks} ticks | Command {sg.Cmd + 1}: {sg.Desc} | State {sg.Main}/{sg.Sub} | Pos ({x:0},{y:0})" + (pvRes.Truncated ? " | simulation truncated at limit" : "") + (pvRes.Note != null ? " | " + pvRes.Note : "");
            HighlightCmd(sg.Cmd);
        }

        int hlCmd = -1;
        void OnCmdsChanged(object s, TextChangedEventArgs e) { if (hlCmd >= 0) HighlightCmd(hlCmd); }
        void HighlightCmd(int cmdIdx)
        {
            hlCmd = cmdIdx; var all = tbCmds.Text; int k = -1, ps = 0;
            while (ps <= all.Length)
            {
                int nl = all.IndexOf('\n', ps); int end = nl < 0 ? all.Length : nl; var line = all.Substring(ps, end - ps).Trim();
                if (line.Length > 0 && !line.StartsWith("#")) { k++; if (k == cmdIdx) break; }
                if (nl < 0) { hlRect.Visibility = Visibility.Collapsed; return; } ps = nl + 1;
            }
            if (k != cmdIdx) { hlRect.Visibility = Visibility.Collapsed; return; }
            tbCmds.UpdateLayout(); var r = tbCmds.GetRectFromCharacterIndex(ps);
            if (r.IsEmpty) { hlRect.Visibility = Visibility.Collapsed; return; }
            hlRect.Width = Math.Max(10, tbCmds.ActualWidth - 20); hlRect.Height = r.Height; Canvas.SetLeft(hlRect, 2); Canvas.SetTop(hlRect, r.Top); hlRect.Visibility = Visibility.Visible;
        }

        void OnPvTick(object s, EventArgs e)
        {
            if (pvRes == null || pvRes.TotalTicks <= 0) return;
            double[] sp = { 0.25, 0.5, 1, 2, 4 }; pvT += sp[Math.Max(0, cbPvSpeed.SelectedIndex)] * PvTicksPerSec * pvTimer.Interval.TotalSeconds; PvUpdateAnim();
        }
        void OnPvPlay(object s, RoutedEventArgs e)
        {
            if (pvRes == null || pvRes.Segs.Count == 0) { PvRebuild(); if (pvRes == null || pvRes.Segs.Count == 0) return; }
            pvPlaying = !pvPlaying; btPlay.Content = pvPlaying ? "⏸ Pause" : "▶ Play"; if (pvPlaying) pvTimer.Start(); else pvTimer.Stop();
        }
        void OnPvStep(object s, RoutedEventArgs e)
        {
            if (pvRes == null || pvRes.Segs.Count == 0) { PvRebuild(); if (pvRes == null || pvRes.Segs.Count == 0) return; }
            pvTimer.Stop(); pvPlaying = false; btPlay.Content = "▶ Play";
            // jump to the start of the next command segment
            var cur = pvRes.At(pvT % Math.Max(1, pvRes.TotalTicks)); int i = pvRes.Segs.IndexOf(cur); int nx = (i + 1) % pvRes.Segs.Count;
            pvT = nx == 0 ? 0 : pvRes.Segs[nx].T0; PvUpdateAnim();
        }
        void OnPvReset(object s, RoutedEventArgs e) { pvTimer.Stop(); pvPlaying = false; btPlay.Content = "▶ Play"; pvT = 0; PvUpdateAnim(); }
    }
}
