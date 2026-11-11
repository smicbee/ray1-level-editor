namespace BinarySerializer.Ray1.PC
{
    public class VersionScript : BinarySerializable
    {
        public byte VersionsCount { get; set; }
        public byte RuntimeCurrentVersion { get; set; }

        public string[] VersionCodes { get; set; }
        public string[] VersionModes { get; set; }

        public MemorySizes[] MemorySizes { get; set; }
        public GameVersion GameVersion { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            VersionsCount = s.Serialize<byte>(VersionsCount, name: nameof(VersionsCount));
            RuntimeCurrentVersion = s.Serialize<byte>(RuntimeCurrentVersion, name: nameof(RuntimeCurrentVersion));

            // TODO: Read as linear file

            s.DoAt(Offset + 0x02, () => VersionCodes = s.SerializeStringArray(VersionCodes, VersionsCount, 5, name: nameof(VersionCodes)));
            s.DoAt(Offset + 0x52, () => VersionModes = s.SerializeStringArray(VersionModes, VersionsCount, 20, name: nameof(VersionModes)));
            s.DoAt(Offset + 0x192, () => MemorySizes = s.SerializeObjectArray<MemorySizes>(MemorySizes, VersionsCount, name: nameof(MemorySizes)));

            s.DoAt(Offset + (settings.EngineVersion is Ray1EngineVersion.PC_Kit or Ray1EngineVersion.PC_Fan ? 0x392 : 0x312), () =>
            {
                GameVersion = s.SerializeObject<GameVersion>(GameVersion, name: nameof(GameVersion));
            });

            // Make sure we actually end up at the end of the file.
            s.Goto(Offset + (settings.EngineVersion is Ray1EngineVersion.PC_Kit or Ray1EngineVersion.PC_Fan ? 0x39E : 0x31E));
        }
    }
}
