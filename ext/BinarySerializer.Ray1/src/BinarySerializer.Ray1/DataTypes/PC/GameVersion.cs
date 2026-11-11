namespace BinarySerializer.Ray1.PC
{
    public class GameVersion : BinarySerializable
    {
        public string PrimaryVersion { get; set; } // EDU, QUI, KIT
        public string SecondaryVersion { get; set; } // Either same as primary or a secondary version like US1
        public ushort Ushort_0A { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
            PrimaryVersion = s.SerializeString(PrimaryVersion, 5, name: nameof(PrimaryVersion));
            SecondaryVersion = s.SerializeString(SecondaryVersion, 5, name: nameof(SecondaryVersion));
            Ushort_0A = s.Serialize<ushort>(Ushort_0A, name: nameof(Ushort_0A));
        }
    }
}