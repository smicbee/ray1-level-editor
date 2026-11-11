namespace BinarySerializer.Ray1.PC
{
    public class UniversalMapBlock : BinarySerializable
    {
        public ushort TileIndex { get; set; }
        public BlockType BlockType { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileIndex = s.Serialize<ushort>(TileIndex, name: nameof(TileIndex));
            BlockType = s.Serialize<BlockType>(BlockType, name: nameof(BlockType));
            s.SerializePadding(1, logIfNotNull: true);
        }
    }
}