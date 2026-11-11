namespace BinarySerializer.Ray1.GBA
{
    public class SaveData : BinarySerializable
    {
        public SaveSlot[] SaveSlots { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            SaveSlots = s.SerializeObjectArray<SaveSlot>(SaveSlots, 3, name: nameof(SaveSlots));
        }
    }
}