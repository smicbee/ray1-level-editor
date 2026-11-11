namespace BinarySerializer.Ray1
{
    public class SoundEventKeyFrame : BinarySerializable
    {
        public short Event { get; set; } // Index to global event table
        public byte TimerIndex { get; set; }
        public byte Byte_03 { get; set; }
        public byte AnimationFrame { get; set; }
        public byte Byte_05 { get; set; }
        public sbyte TimerStartValue { get; set; }
        public sbyte TimerMode { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Event = s.Serialize<short>(Event, name: nameof(Event));
            TimerIndex = s.Serialize<byte>(TimerIndex, name: nameof(TimerIndex));
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            AnimationFrame = s.Serialize<byte>(AnimationFrame, name: nameof(AnimationFrame));
            Byte_05 = s.Serialize<byte>(Byte_05, name: nameof(Byte_05));
            TimerStartValue = s.Serialize<sbyte>(TimerStartValue, name: nameof(TimerStartValue));
            TimerMode = s.Serialize<sbyte>(TimerMode, name: nameof(TimerMode));
        }
    }
}