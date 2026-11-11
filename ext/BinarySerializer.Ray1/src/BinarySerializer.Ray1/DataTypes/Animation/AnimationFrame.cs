namespace BinarySerializer.Ray1
{
    /// <summary>
    /// A frame for sprite animations. This defines the rectangle area used for the
    /// sprites in each frame of an animation. It's not required for playing the
    /// animation itself, but rather used to provide additional data on it.
    /// </summary>
    public class AnimationFrame : BinarySerializable, ISerializerShortLog
    {
        public byte XPosition { get; set; }
        public byte YPosition { get; set; }
        public byte Width { get; set; }
        public byte Height { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            XPosition = s.Serialize<byte>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<byte>(YPosition, name: nameof(YPosition));
            Width = s.Serialize<byte>(Width, name: nameof(Width));
            Height = s.Serialize<byte>(Height, name: nameof(Height));
        }

        public string ShortLog => ToString();
        public override string ToString() => $"AnimationFrame(Pos: {XPosition}x{YPosition}, Size: {Width}x{Height})";
    }
}