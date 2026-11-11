namespace BinarySerializer.Ray1.PC
{
    /// <summary>
    /// Tile texture data for PC
    /// </summary>
    public class TileSetBlock : BinarySerializable
    {
        public bool Pre_HasAlpha { get; set; }

        public byte[] ImgData { get; set; }

        /// <summary>
        /// The alpha channel values for each texture pixel
        /// </summary>
        public byte[] Alpha { get; set; }

        /// <summary>
        /// Flags determining the tile transparency mode
        /// </summary>
        public uint TransparencyMode { get; set; } // 2 bits per row of the tile, determines the transparency

        /// <summary>
        /// Unknown array of bytes. Appears to be leftover garbage data.
        /// </summary>
        public byte[] UnkownBytes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ImgData = s.SerializeArray<byte>(ImgData, 256, name: nameof(ImgData));
            
            if (Pre_HasAlpha)
                Alpha = s.SerializeArray<byte>(Alpha, 256, name: nameof(Alpha));

            TransparencyMode = s.Serialize<uint>(TransparencyMode, name: nameof(TransparencyMode));
            UnkownBytes = s.SerializeArray<byte>(UnkownBytes, 28, name: nameof(UnkownBytes));
        }
    }
}