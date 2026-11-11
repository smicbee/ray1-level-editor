using BinarySerializer;
using BinarySerializer.Ray1;
using BinarySerializer.Ray1.PC;

// usage: RoundTrip <PCMAP dir> <out dir>
var pcmap = args[0]; var outDir = args[1];
Directory.CreateDirectory(outDir);
string[] worlds = { "JUNGLE", "MUSIC", "MOUNTAIN", "IMAGE", "CAVE", "CAKE" };
int ok = 0, bad = 0, err = 0;
for (int w = 0; w < worlds.Length; w++)
{
    foreach (var f in Directory.GetFiles(Path.Combine(pcmap, worlds[w]), "RAY*.LEV").OrderBy(x => x))
    {
        int lvl = int.Parse(Path.GetFileNameWithoutExtension(f).Substring(3));
        string rel = $"{worlds[w]}/RAY{lvl}.LEV";
        try
        {
            string src = Path.Combine(outDir, "orig", rel); Directory.CreateDirectory(Path.GetDirectoryName(src)); File.Copy(f, src, true);
            string orig = File.Exists(src) ? src : f;
            var a = File.ReadAllBytes(f);
            using (var ctx = new Context(Path.Combine(outDir, "orig")))
            {
                ctx.AddSettings(new Ray1Settings(Ray1EngineVersion.PC, (World)(w + 1), lvl, Ray1PCVersion.PC_1_21));
                ctx.AddFile(new LinearFile(ctx, rel));
                var lev = FileFactory.Read<LevelFile>(ctx, rel);
                FileFactory.Write<LevelFile>(ctx, rel);
            }
            var b = File.ReadAllBytes(src);
            bool same = a.SequenceEqual(b);
            if (same) ok++; else { bad++; int d = 0; while (d < Math.Min(a.Length, b.Length) && a[d] == b[d]) d++; Console.WriteLine($"DIFF {rel} len {a.Length}/{b.Length} firstdiff {d}"); }
        }
        catch (Exception e) { err++; Console.WriteLine($"ERR {rel}: {e.GetType().Name} {e.Message}"); }
    }
}
Console.WriteLine($"identical={ok} different={bad} errors={err}");
