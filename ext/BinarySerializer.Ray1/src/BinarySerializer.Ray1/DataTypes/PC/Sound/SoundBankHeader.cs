namespace BinarySerializer.Ray1.PC
{
    public class SoundBankHeader : BinarySerializable
    {
        public SoundFileEntry[] SoundFileEntries { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            SoundFileEntries = s.SerializeObjectArray<SoundFileEntry>(SoundFileEntries, 128, name: nameof(SoundFileEntries));
        }
    }
}