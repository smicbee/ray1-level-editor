namespace BinarySerializer.Ray1.Jaguar
{
    public class Jag_ColoredString : BinarySerializable
    {
        public byte[] Bytes_00 { get; set; } // First two bytes define color, last two bytes define multi-color pattern
        public string Text { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Bytes_00 = s.SerializeArray<byte>(Bytes_00, 4, name: nameof(Bytes_00));
            Text = s.SerializeString(Text, name: nameof(Text));
        }
    }
}