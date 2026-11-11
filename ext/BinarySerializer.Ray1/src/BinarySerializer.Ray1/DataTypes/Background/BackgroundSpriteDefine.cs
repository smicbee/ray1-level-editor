namespace BinarySerializer.Ray1
{
    public class BackgroundSpriteDefine : BinarySerializable
    {
        public short XPosition { get; set; }
        public short YPosition { get; set; }
        public ushort BandIndex { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));

            // In the PS1 versions the band index is defined in the sprite id
            if (settings.EngineBranch == Ray1EngineBranch.PC)
            {
                BandIndex = s.Serialize<ushort>(BandIndex, name: nameof(BandIndex));
                s.SerializePadding(2, logIfNotNull: true);
            }
        }
    }
}