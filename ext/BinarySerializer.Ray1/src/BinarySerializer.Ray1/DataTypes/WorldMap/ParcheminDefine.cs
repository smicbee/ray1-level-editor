namespace BinarySerializer.Ray1
{
    public class ParcheminDefine : BinarySerializable
    {
        public short MapXPosition { get; set; }
        public short MapYPosition { get; set; }
        public short SaveXPosition { get; set; }
        public short SaveYPosition { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapXPosition = s.Serialize<short>(MapXPosition, name: nameof(MapXPosition));
            MapYPosition = s.Serialize<short>(MapYPosition, name: nameof(MapYPosition));
            SaveXPosition = s.Serialize<short>(SaveXPosition, name: nameof(SaveXPosition));
            SaveYPosition = s.Serialize<short>(SaveYPosition, name: nameof(SaveYPosition));
        }
    }
}