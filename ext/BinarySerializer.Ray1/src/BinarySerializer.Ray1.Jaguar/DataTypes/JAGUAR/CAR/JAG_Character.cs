namespace BinarySerializer.Ray1.Jaguar
{
    public class JAG_Character : BinarySerializable
    {
        public JAG_Object Tech { get; set; }

        public byte _1 { get; set; }
        public byte _2 { get; set; }
        public byte _3 { get; set; }

        public byte DisplayPrio { get; set; }
        public byte Force { get; set; } // ?
        public byte Resistance { get; set; } // Hp
        public byte DispScore { get; set; } // ?
        public byte Family { get; set; }
        
        public short ColX { get; set; }
        public short ColY { get; set; }
        public short ColWidth { get; set; }
        public short ColHeight { get; set; }

        public JAG_CollideType Type { get; set; }
        public short ColSprite { get; set; } // Offset to displays. If 0 it's relative to the object itself instead.

        public JAG_CharacterCollide[] Collides { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Tech = s.SerializeObject<JAG_Object>(Tech, name: nameof(Tech));

            _1 = s.Serialize<byte>(_1, name: nameof(_1));
            _2 = s.Serialize<byte>(_2, name: nameof(_2));
            _3 = s.Serialize<byte>(_3, name: nameof(_3));

            DisplayPrio = s.Serialize<byte>(DisplayPrio, name: nameof(DisplayPrio));
            Force = s.Serialize<byte>(Force, name: nameof(Force));
            Resistance = s.Serialize<byte>(Resistance, name: nameof(Resistance));
            DispScore = s.Serialize<byte>(DispScore, name: nameof(DispScore));
            Family = s.Serialize<byte>(Family, name: nameof(Family));

            ColX = s.Serialize<short>(ColX, name: nameof(ColX));
            ColY = s.Serialize<short>(ColY, name: nameof(ColY));
            ColWidth = s.Serialize<short>(ColWidth, name: nameof(ColWidth));
            ColHeight = s.Serialize<short>(ColHeight, name: nameof(ColHeight));

            Type = s.Serialize<JAG_CollideType>(Type, name: nameof(Type));
            ColSprite = s.Serialize<short>(ColSprite, name: nameof(ColSprite));

            Collides = s.SerializeObjectArrayUntil(Collides, x => x.Sprite == -1, () => new JAG_CharacterCollide()
            {
                Sprite = -1
            }, name: nameof(Collides));
        }
    }
}