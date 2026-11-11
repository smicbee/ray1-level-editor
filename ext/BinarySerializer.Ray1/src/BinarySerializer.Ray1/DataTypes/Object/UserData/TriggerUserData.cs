namespace BinarySerializer.Ray1
{
    public class TriggerUserData : BinarySerializable
    {
        public Pointer LinkedObjectsPointer { get; set; }
        public ushort LinkedObjectsCount { get; set; }
        public ushort Flags { get; set; }
        public byte[] Bytes_08 { get; set; }

        public short[] LinkedObjects { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LinkedObjectsPointer = s.SerializePointer(LinkedObjectsPointer, name: nameof(LinkedObjectsPointer));
            LinkedObjectsCount = s.Serialize<ushort>(LinkedObjectsCount, name: nameof(LinkedObjectsCount));
            Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));
            Bytes_08 = s.SerializeArray<byte>(Bytes_08, 4, name: nameof(Bytes_08));

            s.DoAt(LinkedObjectsPointer, () => 
                LinkedObjects = s.SerializeArray<short>(LinkedObjects, LinkedObjectsCount, name: nameof(LinkedObjects)));
        }
    }
}