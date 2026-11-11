namespace BinarySerializer.Ray1.PC
{
    public class SampleName : BinarySerializable
    {
        /// <summary>
        /// The name of the sample
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The time before the sample can be repeated, in seconds
        /// </summary>
        public ushort RepeatTime { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Name = s.SerializeString(Name, 9, name: nameof(Name));
            RepeatTime = s.Serialize<ushort>(RepeatTime, name: nameof(RepeatTime));
        }
    }
}