namespace BinarySerializer.Ray1
{
    public class HelpDefineEntry : BinarySerializable
    {
        public string HelpVignette { get; set; }
        public string HelpSound { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            HelpVignette = s.SerializeString(HelpVignette, length: 9, name: nameof(HelpVignette));
            HelpSound = s.SerializeString(HelpSound, length: 9, name: nameof(HelpSound));
        }
    }
}