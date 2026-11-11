namespace BinarySerializer.Ray1.PC.PS1EDU
{
    public class TEX : BinarySerializable
    {
        public uint NumDescriptors { get; set; }
        public uint NumPages { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public uint BitDepth { get; set; }
        public uint NumPalettes { get; set; }
        public uint Uint_18 { get; set; } // Padding? Not referenced in the code

        // Parsed
        public Pointer[] PagePointers { get; set; }
        public ObjectArray<RGBA5551Color>[] Palettes { get; set; }
        public TEXDescriptor[] Descriptors { get; set; }
        public byte[][] TexturePages { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            NumDescriptors = s.Serialize<uint>(NumDescriptors, name: nameof(NumDescriptors));
            NumPages = s.Serialize<uint>(NumPages, name: nameof(NumPages));
            Width = s.Serialize<uint>(Width, name: nameof(Width));
            Height = s.Serialize<uint>(Height, name: nameof(Height));
            BitDepth = s.Serialize<uint>(BitDepth, name: nameof(BitDepth)); // can be 4, 8 or 16
            NumPalettes = s.Serialize<uint>(NumPalettes, name: nameof(NumPalettes));
            Uint_18 = s.Serialize<uint>(Uint_18, name: nameof(Uint_18));

            // Initialize array sizes
            PagePointers ??= new Pointer[NumPages];
            TexturePages ??= new byte[NumPages][];
            Palettes ??= new ObjectArray<RGBA5551Color>[NumPalettes];
            Descriptors ??= new TEXDescriptor[NumDescriptors];

            PagePointers = s.SerializePointerArray(PagePointers, PagePointers.Length, anchor: Offset, name: nameof(PagePointers));
            Palettes = s.SerializeObjectArray(Palettes, Palettes.Length, onPreSerialize: a => a.Pre_Length = PaletteLength, name: nameof(Palettes));
            Descriptors = s.SerializeObjectArray(Descriptors, Descriptors.Length, name: nameof(Descriptors));

            for (int i = 0; i < TexturePages.Length; i++)
                s.DoAt(PagePointers[i], () => 
                    TexturePages[i] = s.SerializeArray<byte>(TexturePages[i], PageLength, name: $"{nameof(TexturePages)}[{i}]"));
        }

        private uint PaletteLength =>
            BitDepth switch
            {
                4 => 16,
                8 => 256,
                16 => 0, // 16 bit is direct color, no palette
                _ => 0
            };

        private uint PageLength => Width * Height; // * BitDepth / 8;

        public ushort GetPagePixel(int pageIndex, int x, int y)
        {
            int actualX = x * (int)BitDepth / 8;
            //int actualW = (int)Width * 8 / (int)BitDepth;
            int ind = y * (int)Width + actualX;
            switch (BitDepth) 
            {
                case 4:
                    if (x % 2 == 0)
                        return (ushort)BitHelpers.ExtractBits(TexturePages[pageIndex][ind], 4, 0);
                    else
                        return (ushort)BitHelpers.ExtractBits(TexturePages[pageIndex][ind], 4, 4);
                case 8:
                    return TexturePages[pageIndex][ind];
                case 16:
                    return (ushort)((TexturePages[pageIndex][ind]) | (TexturePages[pageIndex][ind + 1] << 8));
            }
            return 0;
        }
    }
}