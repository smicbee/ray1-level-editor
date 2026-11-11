namespace BinarySerializer.Ray1.PC
{
    /// <summary>
    /// An .SCT file
    /// </summary>
    public class LevelTime : BinarySerializable
    {
        public int Value { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            s.DoEncoded(new LevelTimeEncoder(settings), () => Value = s.Serialize<int>(Value, name: nameof(Value)));
        }
    }
}