namespace BinarySerializer.Ray1
{
    public class FontTable : BinarySerializable
    {
        public Pointer TablePointer { get; set; }
        public byte Byte_04 { get; set; } // The HUD sprites count, 0 for font
        public byte Byte_05 { get; set; } // 1 if full 256 font table, 0 if HUD sprites

        // Serialized from pointers
        public ushort[] SpriteIndexTable { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TablePointer = s.SerializePointer(TablePointer, name: nameof(TablePointer));
            Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
            Byte_05 = s.Serialize<byte>(Byte_05, name: nameof(Byte_05));

            s.DoAt(TablePointer, () => 
                SpriteIndexTable = s.SerializeArray<ushort>(SpriteIndexTable, Byte_05 == 1 ? 256 : Byte_04, name: nameof(SpriteIndexTable)));
        }
    }
}