namespace BinarySerializer.Ray1.PC.PS1EDU
{
    public class TEXDescriptor : BinarySerializable
    {
        public byte XInPage { get; set; }
        public byte YInPage { get; set; }
        public byte Width { get; set; }
        public byte Height { get; set; }
        public byte PageIndex { get; set; }

        // The next fields are set at runtime, so they can be ignored
        public byte BitDepth { get; set; }
        public ushort PageInfo { get; set; }
        public ushort Ushort_08 { get; set; }
        public ushort Ushort_0A { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            XInPage = s.Serialize<byte>(XInPage, name: nameof(XInPage));
            YInPage = s.Serialize<byte>(YInPage, name: nameof(YInPage));
            Width = s.Serialize<byte>(Width, name: nameof(Width));
            Height = s.Serialize<byte>(Height, name: nameof(Height));
            PageIndex = s.Serialize<byte>(PageIndex, name: nameof(PageIndex));
            BitDepth = s.Serialize<byte>(BitDepth, name: nameof(BitDepth));
            PageInfo = s.Serialize<ushort>(PageInfo, name: nameof(PageInfo));
            Ushort_08 = s.Serialize<ushort>(Ushort_08, name: nameof(Ushort_08));
            Ushort_0A = s.Serialize<ushort>(Ushort_0A, name: nameof(Ushort_0A));
        }
    }
}