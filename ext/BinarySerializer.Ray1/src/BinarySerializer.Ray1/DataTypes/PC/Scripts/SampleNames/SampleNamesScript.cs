namespace BinarySerializer.Ray1.PC
{
    public class SampleNamesScript : BinarySerializable
    {
        public ushort SampleNamesCount { get; set; }
        public SampleName[] SampleNames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            SampleNamesCount = s.Serialize<ushort>(SampleNamesCount, name: nameof(SampleNamesCount));
            SampleNames = s.SerializeObjectArray<SampleName>(SampleNames, SampleNamesCount, name: nameof(SampleNames));
        }
    }
}