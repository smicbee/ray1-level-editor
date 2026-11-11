namespace BinarySerializer.Ray1.PC
{
    /// <summary>
    /// Encrypted file archive entry data for PC
    /// </summary>
    public class FileArchiveEntry : BinarySerializable
    {
        /// <summary>
        /// The XOR key to use when decoding the file
        /// </summary>
        public byte XORKey { get; set; }

        /// <summary>
        /// The encoded file checksum
        /// </summary>
        public byte Checksum { get; set; }
        
        /// <summary>
        /// The file offset
        /// </summary>
        public uint FileOffset { get; set; }
        
        /// <summary>
        /// The file size
        /// </summary>
        public uint FileSize { get; set; }
        
        /// <summary>
        /// The file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            var settings = s.GetRequiredSettings<Ray1Settings>();

            if (settings.EngineVersion == Ray1EngineVersion.PC || settings.EngineVersion == Ray1EngineVersion.PocketPC)
            {
                FileOffset = s.Serialize<uint>(FileOffset, name: nameof(FileOffset));
                FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));
                XORKey = s.Serialize<byte>(XORKey, name: nameof(XORKey));
                Checksum = s.Serialize<byte>(Checksum, name: nameof(Checksum));

                s.SerializePadding(2);
            }
            else
            {
                XORKey = s.Serialize<byte>(XORKey, name: nameof(XORKey));
                Checksum = s.Serialize<byte>(Checksum, name: nameof(Checksum));
                FileOffset = s.Serialize<uint>(FileOffset, name: nameof(FileOffset));
                FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));

                s.DoProcessed(new Xor8Processor(XORKey), () => FileName = s.SerializeString(FileName, 9, name: nameof(FileName)));
            }
        }
    }
}