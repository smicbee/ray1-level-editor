namespace BinarySerializer.Ray1.Jaguar
{
    public class SNES_Sprite : BinarySerializable
    {
        // See https://wiki.superfamicom.org/snes-sprites (Sprite Table 2)
        public byte Padding0 { get; set; }
        public bool IsEmpty { get; set; } // true = no sprite?
        public byte Padding1 { get; set; }
        public bool IsLarge { get; set; } // true = 16x16, false = 8x8
        // Same link (Sprite Table 1)
        public int TileIndex { get; set; }
        public int Palette { get; set; }
        public int Priority { get; set; }
        public bool FlipX { get; set; }
        public bool FlipY { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoBits<byte>(b => 
            {
                Padding0 = b.SerializeBits<byte>(Padding0, 3, name: nameof(Padding0));
                IsEmpty = b.SerializeBits<bool>(IsEmpty, 1, name: nameof(IsEmpty));
                Padding1 = b.SerializeBits<byte>(Padding1, 3, name: nameof(Padding1));
                IsLarge = b.SerializeBits<bool>(IsLarge, 1, name: nameof(IsLarge));
            });
            s.DoBits<ushort>(b =>
            {
                TileIndex = b.SerializeBits<int>(TileIndex, 9, name: nameof(TileIndex));
                Palette = b.SerializeBits<int>(Palette, 3, name: nameof(Palette));
                Priority = b.SerializeBits<int>(Priority, 2, name: nameof(Priority));
                FlipX = b.SerializeBits<bool>(FlipX, 1, name: nameof(FlipX));
                FlipY = b.SerializeBits<bool>(FlipY, 1, name: nameof(FlipY));
            });
        }
    }
}