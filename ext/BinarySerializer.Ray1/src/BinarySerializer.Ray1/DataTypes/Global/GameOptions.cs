namespace BinarySerializer.Ray1
{
    public class GameOptions : BinarySerializable
    {
        // Serialize as uint instead of pointer for now since we're only using this when reading save data
        public uint Fire1ButtonFunctionPointer { get; set; }
        public uint Fire0ButtonFunctionPointer { get; set; }
        public uint Button4FunctionPointer { get; set; }
        public uint Button3FunctionPointer { get; set; }

        public ushort JumpButton { get; set; }
        public ushort FistButton { get; set; }
        public ushort UnusedButton { get; set; }
        public ushort ActionButton { get; set; }

        public ushort MusicVolume { get; set; }
        public ushort SoundVolume { get; set; }
        public ushort SteroEnabled { get; set; }

        public short Fire1ButtonValue { get; set; }
        public short Fire0ButtonValue { get; set; }
        public short Button4Value { get; set; }
        public short Button3Value { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Fire1ButtonFunctionPointer = s.Serialize<uint>(Fire1ButtonFunctionPointer, name: nameof(Fire1ButtonFunctionPointer));
            Fire0ButtonFunctionPointer = s.Serialize<uint>(Fire0ButtonFunctionPointer, name: nameof(Fire0ButtonFunctionPointer));
            Button4FunctionPointer = s.Serialize<uint>(Button4FunctionPointer, name: nameof(Button4FunctionPointer));
            Button3FunctionPointer = s.Serialize<uint>(Button3FunctionPointer, name: nameof(Button3FunctionPointer));

            JumpButton = s.Serialize<ushort>(JumpButton, name: nameof(JumpButton));
            FistButton = s.Serialize<ushort>(FistButton, name: nameof(FistButton));
            UnusedButton = s.Serialize<ushort>(UnusedButton, name: nameof(UnusedButton));
            ActionButton = s.Serialize<ushort>(ActionButton, name: nameof(ActionButton));

            MusicVolume = s.Serialize<ushort>(MusicVolume, name: nameof(MusicVolume));
            SoundVolume = s.Serialize<ushort>(SoundVolume, name: nameof(SoundVolume));
            SteroEnabled = s.Serialize<ushort>(SteroEnabled, name: nameof(SteroEnabled));

            Fire1ButtonValue = s.Serialize<short>(Fire1ButtonValue, name: nameof(Fire1ButtonValue));
            Fire0ButtonValue = s.Serialize<short>(Fire0ButtonValue, name: nameof(Fire0ButtonValue));
            Button4Value = s.Serialize<short>(Button4Value, name: nameof(Button4Value));
            Button3Value = s.Serialize<short>(Button3Value, name: nameof(Button3Value));
        }
    }
}