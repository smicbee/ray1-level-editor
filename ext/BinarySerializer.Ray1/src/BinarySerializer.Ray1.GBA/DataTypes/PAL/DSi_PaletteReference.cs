namespace BinarySerializer.Ray1.GBA
{
    /// <summary>
    /// Palette Reference for Rayman 1 (DSi)
    /// </summary>
    public class DSi_PaletteReference : BinarySerializable
    {
        public Pointer PalettePointer { get; set; }
        public uint UInt_04 { get; set; }
        public Pointer NamePointer { get; set; }

        public RGBA5551Color[] Palette { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            PalettePointer = s.SerializePointer(PalettePointer, name: nameof(PalettePointer));
            UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
            NamePointer = s.SerializePointer(NamePointer, name: nameof(NamePointer));

            Name = s.DoAt(NamePointer, () => s.SerializeString(Name, name: nameof(Name)));
            Palette = s.DoAt(PalettePointer, () => s.SerializeObjectArray<RGBA5551Color>(Palette, 256, name: nameof(Palette)));
        }
    }
}