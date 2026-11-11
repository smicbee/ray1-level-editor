namespace BinarySerializer.Ray1.PC
{
    public class WordsScript : BinarySerializable
    {
        public ushort WordsCount { get; set; }
        public string[] Words { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            WordsCount = s.Serialize<ushort>(WordsCount, name: nameof(WordsCount));
            Words = s.SerializeLengthPrefixedStringArray<byte>(Words, WordsCount, name: nameof(Words));
        }
    }
}