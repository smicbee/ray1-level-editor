using BinarySerializer.PlayStation.PS1;

namespace BinarySerializer.Ray1.PS1
{
    /// <summary>
    /// An XXX BigRay pack
    /// </summary>
    public class BigRayPack : Pack
    {
        public BigRayData BigRayData { get; set; }
        public byte[] ImageData { get; set; }
        public Clut Palette1 { get; set; }
        public Clut Palette2 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            base.SerializeImpl(s);

            // Serialize files
            SerializeFile(s, 0, length => BigRayData = s.SerializeObject<BigRayData>(BigRayData, x => x.Pre_Length = length, name: nameof(BigRayData)));
            SerializeFile(s, 1, length => ImageData = s.SerializeArray<byte>(ImageData, length, name: nameof(ImageData)));
            SerializeFile(s, 2, _ => Palette1 = s.SerializeObject<Clut>(Palette1, name: nameof(Palette1)));
            SerializeFile(s, 3, _ => Palette2 = s.SerializeObject<Clut>(Palette2, name: nameof(Palette2)));

            // Go to the end of the pack
            s.Goto(Offset + PackSize);
        }
    }
}