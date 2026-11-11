namespace BinarySerializer.Ray1.PC.PS1EDU
{
    // This class uses the same structure as normal objects. Dummy data is always 0 as the obj values aren't used here.

    /// <summary>
    /// DES data for EDU on PS1
    /// </summary>
    public class DESTemplate : BinarySerializable
    {
        // These get set during runtime
        public uint SpritesPointer { get; set; }
        public uint AnimationsPointer { get; set; }

        public byte[] Dummy1 { get; set; }

        public uint ImageBufferLength { get; set; }

        public byte[] Dummy2 { get; set; }

        public ushort SpritesCount { get; set; }

        public byte[] Dummy3 { get; set; }

        public byte AnimationsCount { get; set; }

        public byte[] Dummy4 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            SpritesPointer = s.Serialize<uint>(SpritesPointer, name: nameof(SpritesPointer));
            AnimationsPointer = s.Serialize<uint>(AnimationsPointer, name: nameof(AnimationsPointer));
            Dummy1 = s.SerializeArray<byte>(Dummy1, 32, name: nameof(Dummy1));
            ImageBufferLength = s.Serialize<uint>(ImageBufferLength, name: nameof(ImageBufferLength));
            Dummy2 = s.SerializeArray<byte>(Dummy2, 24, name: nameof(Dummy2));
            SpritesCount = s.Serialize<ushort>(SpritesCount, name: nameof(SpritesCount));
            Dummy3 = s.SerializeArray<byte>(Dummy3, 62, name: nameof(Dummy3));
            AnimationsCount = s.Serialize<byte>(AnimationsCount, name: nameof(AnimationsCount));
            Dummy4 = s.SerializeArray<byte>(Dummy4, 3, name: nameof(Dummy4));
        }
    }
}