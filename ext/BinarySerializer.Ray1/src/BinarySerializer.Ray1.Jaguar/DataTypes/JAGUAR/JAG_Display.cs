namespace BinarySerializer.Ray1.Jaguar
{
    public class JAG_Display : BinarySerializable
    {
        public Pointer CollisionPointer { get; set; }

        public short RefXPos { get; set; }
        public short RefYPos { get; set; }
        public short XPos { get; set; }
        public short YPos { get; set; }

        public JAG_Object Tech { get; set; }

        public byte _1 { get; set; }
        public byte _2 { get; set; }
        public byte _3 { get; set; }

        public byte DisplayPrio { get; set; }
        public byte Force { get; set; }
        public byte Resistance { get; set; }
        public byte Score { get; set; }
        public byte Family { get; set; }
        public byte Nib { get; set; }

        public byte EvType { get; set; }
        public Pointer EvPointer { get; set; } // Event pointer?

        // Serialized from pointers
        public JAG_Collide Collision { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            CollisionPointer = s.SerializePointer(CollisionPointer, name: nameof(CollisionPointer));

            RefXPos = s.Serialize<short>(RefXPos, name: nameof(RefXPos));
            RefYPos = s.Serialize<short>(RefYPos, name: nameof(RefYPos));
            XPos = s.Serialize<short>(XPos, name: nameof(XPos));
            YPos = s.Serialize<short>(YPos, name: nameof(YPos));

            Tech = s.SerializeObject<JAG_Object>(Tech, name: nameof(Tech));

            _1 = s.Serialize<byte>(_1, name: nameof(_1));
            _2 = s.Serialize<byte>(_2, name: nameof(_2));
            _3 = s.Serialize<byte>(_3, name: nameof(_3));

            DisplayPrio = s.Serialize<byte>(DisplayPrio, name: nameof(DisplayPrio));
            Force = s.Serialize<byte>(Force, name: nameof(Force));
            Resistance = s.Serialize<byte>(Resistance, name: nameof(Resistance));
            Score = s.Serialize<byte>(Score, name: nameof(Score));
            Family = s.Serialize<byte>(Family, name: nameof(Family));
            Nib = s.Serialize<byte>(Nib, name: nameof(Nib));

            EvType = s.Serialize<byte>(EvType, name: nameof(EvType));
            EvPointer = s.SerializePointer(EvPointer, size: PointerSize.Pointer16, name: nameof(EvPointer));

            s.DoAt(CollisionPointer, () => Collision = s.SerializeObject<JAG_Collide>(Collision, name: nameof(Collision)));
        }
    }
}