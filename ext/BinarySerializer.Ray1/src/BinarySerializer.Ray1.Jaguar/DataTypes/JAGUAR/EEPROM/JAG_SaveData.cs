namespace BinarySerializer.Ray1.Jaguar
{
    public class JAG_SaveData : BinarySerializable
    {
        public JAG_SaveSlot[] SaveSlots { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoEncoded(new JAG_EEPROMEncoder(128), () =>
            {
                s.DoProcessed(new BigEndianChecksum16Processor(true, valueSize: 2), p =>
                {
                    s.SerializeMagic<ushort>(0x4B58);
                    SaveSlots = s.SerializeObjectArray<JAG_SaveSlot>(SaveSlots, 3, name: nameof(SaveSlots));
                    s.SerializePadding(16, logIfNotNull: true);
                    p.Serialize<ushort>(s, "Checksum");
                });
            }, endianness: Endian.Big);
        }
    }
}