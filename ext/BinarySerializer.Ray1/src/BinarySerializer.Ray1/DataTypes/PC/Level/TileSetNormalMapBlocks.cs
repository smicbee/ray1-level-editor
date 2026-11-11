namespace BinarySerializer.Ray1.PC
{
    public class TileSetNormalMapBlocks : BinarySerializable
    {
        public uint Pre_OpaqueBlocksCount { get; set; }
        public uint Pre_TransparentBlocksCount { get; set; }

        public TileSetBlock[] OpaqueBlocks { get; set; }
        public TileSetBlock[] TransparentBlocks { get; set; }

        // 32 unknown bytes - appears unused
        public byte[] UnknownBytes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize the textures
            OpaqueBlocks = s.SerializeObjectArray<TileSetBlock>(OpaqueBlocks, Pre_OpaqueBlocksCount, name: nameof(OpaqueBlocks));
            TransparentBlocks = s.SerializeObjectArray<TileSetBlock>(TransparentBlocks, Pre_TransparentBlocksCount, x => x.Pre_HasAlpha = true, name: nameof(TransparentBlocks));

            // Serialize unknown bytes
            UnknownBytes = s.SerializeArray<byte>(UnknownBytes, 32, name: nameof(UnknownBytes));
        }
    }
}