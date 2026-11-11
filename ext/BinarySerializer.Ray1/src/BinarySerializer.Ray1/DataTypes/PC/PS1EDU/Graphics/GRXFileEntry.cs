namespace BinarySerializer.Ray1.PC.PS1EDU
{
    /// <summary>
    /// GRX file data for EDU on PS1
    /// </summary>
    public class GRXFileEntry : BinarySerializable
    {
        /// <summary>
        /// The file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The file size
        /// </summary>
        public uint FileSize { get; set; }

        /// <summary>
        /// The file offset, based on the base offset
        /// </summary>
        public uint FileOffset { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FileName = s.SerializeString(FileName, name: nameof(FileName));
            FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));
            FileOffset = s.Serialize<uint>(FileOffset, name: nameof(FileOffset));
        }
    }
}