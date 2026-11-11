namespace BinarySerializer.Ray1
{
    public class MapDimensions : BinarySerializable
    {
        public short Width { get; set; }
        public short Height { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Width = s.Serialize<short>(Width, name: nameof(Width));
            Height = s.Serialize<short>(Height, name: nameof(Height));
        }
    }
}