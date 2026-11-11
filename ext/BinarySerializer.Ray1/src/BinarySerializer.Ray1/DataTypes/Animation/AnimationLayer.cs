namespace BinarySerializer.Ray1
{
    /// <summary>
    /// A layer in a sprite animation. This defines the sprite and its placement.
    /// </summary>
    public class AnimationLayer : BinarySerializable, ISerializerShortLog
    {
        public bool FlipX { get; set; }
        public bool FlipY { get; set; }

        public byte XPosition { get; set; }
        public byte YPosition { get; set; }

        /// <summary>
        /// The sprite index. For Rayman 2 this is invalid if 0x3fff.
        /// </summary>
        public ushort SpriteIndex { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            if (settings.EngineVersion == Ray1EngineVersion.R2_PS1)
            {
                s.DoBits<ushort>(b =>
                {
                    // TODO: Serialize as nullable? Might be annoying to use the property for other games then though.
                    SpriteIndex = b.SerializeBits<ushort>(SpriteIndex, 14, name: nameof(SpriteIndex));
                    FlipX = b.SerializeBits<bool>(FlipX, 1, name: nameof(FlipX));
                    FlipY = b.SerializeBits<bool>(FlipY, 1, name: nameof(FlipY));
                });

                XPosition = s.Serialize<byte>(XPosition, name: nameof(XPosition));
                YPosition = s.Serialize<byte>(YPosition, name: nameof(YPosition));
            }
            else if (settings.EngineBranch == Ray1EngineBranch.SNES)
            {
                s.DoBits<byte>(b =>
                {
                    XPosition = b.SerializeBits<byte>(XPosition, 7, name: nameof(XPosition));
                    FlipX = b.SerializeBits<bool>(FlipX, 1, name: nameof(FlipX));
                });
                s.DoBits<byte>(b =>
                {
                    YPosition = b.SerializeBits<byte>(YPosition, 7, name: nameof(YPosition));
                    FlipY = b.SerializeBits<bool>(FlipY, 1, name: nameof(FlipY));
                });
                SpriteIndex = s.Serialize<byte>((byte)SpriteIndex, name: nameof(SpriteIndex));
            }
            else if (settings.EngineBranch == Ray1EngineBranch.Jaguar)
            {
                XPosition = s.Serialize<byte>(XPosition, name: nameof(XPosition));
                YPosition = s.Serialize<byte>(YPosition, name: nameof(YPosition));
                SpriteIndex = s.Serialize<byte>((byte)SpriteIndex, name: nameof(SpriteIndex));
            }
            else
            {
                FlipX = s.Serialize<bool>(FlipX, name: nameof(FlipX));
                XPosition = s.Serialize<byte>(XPosition, name: nameof(XPosition));
                YPosition = s.Serialize<byte>(YPosition, name: nameof(YPosition));
                SpriteIndex = s.Serialize<byte>((byte)SpriteIndex, name: nameof(SpriteIndex));
            }
        }

        public string ShortLog => ToString();
        public override string ToString() => $"AnimationLayer(Pos: {XPosition}x{YPosition}, Sprite: {SpriteIndex}, FlipX: {FlipX}, FlipY: {FlipY})";
    }
}