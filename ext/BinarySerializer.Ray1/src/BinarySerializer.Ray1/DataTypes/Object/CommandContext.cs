namespace BinarySerializer.Ray1
{
    public class CommandContext : BinarySerializable
    {
        /// <summary>
        /// The offset where the context was stored, used to remember where to jump back to
        /// after execution of the sub-function has finished
        /// </summary>
        public short ReturnOffset { get; set; }

        /// <summary>
        /// The amount of times the execution should repeat before continuing, used for loops
        /// </summary>
        public ushort Count { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ReturnOffset = s.Serialize<short>(ReturnOffset, name: nameof(ReturnOffset));
            Count = s.Serialize<ushort>(Count, name: nameof(Count));
        }
    }
}