namespace BinarySerializer.Ray1.GBA
{
    /// <summary>
    /// Vignette background data for Rayman Advance (GBA)
    /// </summary>
    public class GBA_BackgroundVignette : GBA_BaseVignette
    {
        #region Properties

        protected override int PaletteCount => 6;

        #endregion

        #region Vignette Data

        // DSi Unknown dwords
        public uint DSi_Uint_00;
        public uint DSi_Uint_04;
        public uint DSi_Uint_2C;

        // Always 0x00 except for first byte which is sometimes 1
        public byte[] Bytes_14 { get; set; }

        // Both pointers lead to a pointer array - size seems to be specified in UnkBytes_20 - parallax parts?
        public Pointer Pointer_18 { get; set; }
        public Pointer Pointer_1C { get; set; }

        // Third byte is either 0 or 1
        public byte[] Bytes_20 { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            var settings = s.GetRequiredSettings<Ray1Settings>();

            // Serialize data
            if (settings.EngineVersion == Ray1EngineVersion.DSi) 
            {
                DSi_Uint_00 = s.Serialize<uint>(DSi_Uint_00, name: nameof(DSi_Uint_00));
                DSi_Uint_04 = s.Serialize<uint>(DSi_Uint_04, name: nameof(DSi_Uint_04));
            }
            ImageDataPointer = s.SerializePointer(ImageDataPointer, name: nameof(ImageDataPointer));
            BlockIndicesPointer = s.SerializePointer(BlockIndicesPointer, name: nameof(BlockIndicesPointer));
            PaletteIndicesPointer = s.SerializePointer(PaletteIndicesPointer, name: nameof(PaletteIndicesPointer));
            PalettesPointer = s.SerializePointer(PalettesPointer, name: nameof(PalettesPointer));

            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            Bytes_14 = s.SerializeArray<byte>(Bytes_14, 4, name: nameof(Bytes_14));

            Pointer_18 = s.SerializePointer(Pointer_18, name: nameof(Pointer_18));
            Pointer_1C = s.SerializePointer(Pointer_1C, name: nameof(Pointer_1C));

            Bytes_20 = s.SerializeArray<byte>(Bytes_20, 4, name: nameof(Bytes_20));

            if (settings.EngineVersion == Ray1EngineVersion.DSi)
                DSi_Uint_2C = s.Serialize<uint>(DSi_Uint_2C, name: nameof(DSi_Uint_2C));

            // Serialize data from pointers
            SerializeVignette(s, settings.EngineVersion == Ray1EngineVersion.DSi);
        }

        #endregion
    }
}