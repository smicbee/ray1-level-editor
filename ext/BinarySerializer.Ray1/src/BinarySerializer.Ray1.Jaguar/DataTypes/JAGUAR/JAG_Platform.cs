namespace BinarySerializer.Ray1.Jaguar
{
    public class JAG_Platform : BinarySerializable
    {
        public ushort Master { get; set; } // Pointer?
        public ushort Slave { get; set; } // Pointer?
        public short XPos { get; set; }
        public short YPos { get; set; }
        public ushort Flags { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Master = s.Serialize<ushort>(Master, name: nameof(Master));
            Slave = s.Serialize<ushort>(Slave, name: nameof(Slave));
            XPos = s.Serialize<short>(XPos, name: nameof(XPos));
            YPos = s.Serialize<short>(YPos, name: nameof(YPos));
            Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));
        }
    }
}