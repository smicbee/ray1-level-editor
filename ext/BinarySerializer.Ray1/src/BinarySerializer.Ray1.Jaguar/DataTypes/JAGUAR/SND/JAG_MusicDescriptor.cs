namespace BinarySerializer.Ray1.Jaguar
{
    public class JAG_MusicDescriptor : BinarySerializable
    {
        public Pointer MusicDataPointer { get; set; }
        public ushort UShort_04 { get; set; }
        public short Short_06 { get; set; }
        public byte[] Bytes_08 { get; set; }

        // Parsed
        public JAG_MusicData[] MusicData { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            MusicDataPointer = s.SerializePointer(MusicDataPointer, name: nameof(MusicDataPointer));
            UShort_04 = s.Serialize<ushort>(UShort_04, name: nameof(UShort_04));
            Short_06 = s.Serialize<short>(Short_06, name: nameof(Short_06));
            Bytes_08 = s.SerializeArray<byte>(Bytes_08, 8, name: nameof(Bytes_08));

            s.DoAt(MusicDataPointer, () => {
                s.DoEncoded(new RNC2Encoder(), () => {
                    MusicData = s.SerializeObjectArray(MusicData, s.CurrentLength / 0x8, name: nameof(MusicData));
                });
            });
        }
    }
}