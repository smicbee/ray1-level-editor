using Ray1LevelEditor; using BinarySerializer.Ray1;
// usage: EditTest <TestCopy Rayman dir> <world> <level>   -- edits the TEST COPY only
var root=args[0]; int w=int.Parse(args[1]), l=int.Parse(args[2]);
if (root.Contains("Rayman Forever\\") && !root.Contains("Editor Test")) { Console.WriteLine("refusing to touch original"); return 2; }
var d=LevelDoc.Open(root,w,l);
var ray=d.Objs.First(o=>o.D.Type==ObjType.TYPE_RAY_POS);
Console.WriteLine($"RAY_POS at {ray.D.XPosition},{ray.D.YPosition}; objs {d.Objs.Count}; map {d.W}x{d.H}");
// 1) add a duplicate of a collectible/enemy near start: clone the first TYPE_WIZ/ONEUP-like object
var tpl=d.Objs.FirstOrDefault(o=>o.D.Type==ObjType.TYPE_ONEUP)??d.Objs.First(o=>o.D.Type==ObjType.TYPE_WIZ);
var n=tpl.Clone(); n.Link=0; n.D.IsLinked=false; n.D.XPosition=ray.D.XPosition+120; n.D.YPosition=ray.D.YPosition; n.D.InitialXPosition=(short)n.D.XPosition; n.D.InitialYPosition=(short)n.D.YPosition; d.Objs.Add(n);
// 2) edit tiles: flat solid platform right under start, 10 tiles wide
int cx=ray.D.XPosition/16, cy=ray.D.YPosition/16+6;
ushort solidTile=0; foreach(var c in d.Cells){ if(c.Type==15 && c.Tile!=0){solidTile=c.Tile;break;} }
for(int i=-2;i<12;i++){ int x=cx+i; if(x<0||x>=d.W||cy>=d.H)continue; var c=d.Cells[cy*d.W+x]; c.Tile=solidTile; c.Type=15; c.Render=d.RenderModeFor(solidTile); d.Cells[cy*d.W+x]=c; }
// 3) move Rayman start 48px right
ray.D.XPosition+=48; ray.D.InitialXPosition=(short)ray.D.XPosition;
d.Save();
Console.WriteLine($"saved. new obj count {d.Objs.Count}, platform tile {solidTile} at row {cy}");
var d2=LevelDoc.Open(root,w,l);
Console.WriteLine($"reload: objs {d2.Objs.Count}, ray {d2.Objs.First(o=>o.D.Type==ObjType.TYPE_RAY_POS).D.XPosition}, cell {d2.Cells[cy*d2.W+cx].Tile}/{d2.Cells[cy*d2.W+cx].Type}");
return 0;
