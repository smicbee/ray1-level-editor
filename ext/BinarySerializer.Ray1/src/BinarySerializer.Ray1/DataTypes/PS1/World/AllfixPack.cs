using BinarySerializer.PlayStation.PS1;

namespace BinarySerializer.Ray1.PS1
{
    /// <summary>
    /// An XXX allfix file
    /// </summary>
    public class AllfixPack : Pack
    {
        public AllfixData AllfixData { get; set; }

        public byte[] ImageData { get; set; }

        public Clut Palette1 { get; set; }
        public Clut Palette2 { get; set; }
        public Clut Palette3 { get; set; }
        public Clut Palette4 { get; set; }
        public Clut Palette5 { get; set; }
        public Clut Palette6 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            base.SerializeImpl(s);

            // Serialize files
            SerializeFile(s, 0, length => AllfixData = s.SerializeObject<AllfixData>(AllfixData, x => x.Pre_Length = length, name: nameof(AllfixData)));
            SerializeFile(s, 1, length => ImageData = s.SerializeArray<byte>(ImageData, length, name: nameof(ImageData)));
            SerializeFile(s, 2, _ => Palette1 = s.SerializeObject<Clut>(Palette1, name: nameof(Palette1)));
            SerializeFile(s, 3, _ => Palette2 = s.SerializeObject<Clut>(Palette2, name: nameof(Palette2)));
            SerializeFile(s, 4, _ => Palette3 = s.SerializeObject<Clut>(Palette3, name: nameof(Palette3)));
            SerializeFile(s, 5, _ => Palette4 = s.SerializeObject<Clut>(Palette4, name: nameof(Palette4)));
            SerializeFile(s, 6, _ => Palette5 = s.SerializeObject<Clut>(Palette5, name: nameof(Palette5)));
            SerializeFile(s, 7, _ => Palette6 = s.SerializeObject<Clut>(Palette6, name: nameof(Palette6)));

            // Go to the end of the pack
            s.Goto(Offset + PackSize);
        }
    }
}