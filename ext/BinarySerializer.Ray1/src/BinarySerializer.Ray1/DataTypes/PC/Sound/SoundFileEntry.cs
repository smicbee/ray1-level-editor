namespace BinarySerializer.Ray1.PC
{
    public class SoundFileEntry : BinarySerializable
    {
        public uint FileOffset { get; set; }
        public uint FileSize { get; set; }
        public uint LoopStart { get; set; }
        public uint LoopLength { get; set; } // 0 if the sound doesn't loop (unit is in samples)

        public override void SerializeImpl(SerializerObject s)
        {
            FileOffset = s.Serialize<uint>(FileOffset, name: nameof(FileOffset));
            FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));
            LoopStart = s.Serialize<uint>(LoopStart, name: nameof(LoopStart));
            LoopLength = s.Serialize<uint>(LoopLength, name: nameof(LoopLength));
        }
    }
}