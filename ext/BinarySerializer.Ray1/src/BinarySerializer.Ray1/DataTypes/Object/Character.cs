namespace BinarySerializer.Ray1
{
    public class Character : BinarySerializable
    {
        public ObjTypeFlags Flags { get; set; }
        public ZDCReference ZDC { get; set; }

        public short ZoneDiffX { get; set; }
        public short ZoneDiffY { get; set; }

        public byte OffsetBX { get; set; }
        public byte OffsetBY { get; set; }
        public byte OffsetHY { get; set; }
        public byte Byte_0D { get; set; } // Some x offset

        public override void SerializeImpl(SerializerObject s)
        {
            Flags = s.SerializeObject<ObjTypeFlags>(Flags, name: nameof(Flags));
            ZDC = s.SerializeObject<ZDCReference>(ZDC, name: nameof(ZDC));

            ZoneDiffX = s.Serialize<short>(ZoneDiffX, name: nameof(ZoneDiffX));
            ZoneDiffY = s.Serialize<short>(ZoneDiffY, name: nameof(ZoneDiffY));

            OffsetBX = s.Serialize<byte>(OffsetBX, name: nameof(OffsetBX));
            OffsetBY = s.Serialize<byte>(OffsetBY, name: nameof(OffsetBY));
            OffsetHY = s.Serialize<byte>(OffsetHY, name: nameof(OffsetHY));
            Byte_0D = s.Serialize<byte>(Byte_0D, name: nameof(Byte_0D));

            s.SerializePadding(2, logIfNotNull: true);
        }
    }
}