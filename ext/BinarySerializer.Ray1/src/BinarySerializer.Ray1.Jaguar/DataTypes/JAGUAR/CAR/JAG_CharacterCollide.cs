namespace BinarySerializer.Ray1.Jaguar
{
    public class JAG_CharacterCollide : BinarySerializable
    {
        public short Sprite { get; set; } // Offset to displays
        public short XPos { get; set; }
        public short YPos { get; set; }
        public short Width { get; set; }
        public short Height { get; set; }
        public JAG_CollideType Type { get; set; }
        public short Family { get; set; } // The family is usually responsible for handling the collision

        public override void SerializeImpl(SerializerObject s)
        {
            Sprite = s.Serialize<short>(Sprite, name: nameof(Sprite));

            if (Sprite == -1)
                return;

            XPos = s.Serialize<short>(XPos, name: nameof(XPos));
            YPos = s.Serialize<short>(YPos, name: nameof(YPos));
            Width = s.Serialize<short>(Width, name: nameof(Width));
            Height = s.Serialize<short>(Height, name: nameof(Height));
            Type = s.Serialize<JAG_CollideType>(Type, name: nameof(Type));
            Family = s.Serialize<short>(Family, name: nameof(Family));
        }
    }
}