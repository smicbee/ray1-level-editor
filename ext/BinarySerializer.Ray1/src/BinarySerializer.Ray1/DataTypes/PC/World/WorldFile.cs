namespace BinarySerializer.Ray1.PC
{
    public class WorldFile : BaseWorldFile
    {
        // Used to allocate the buffer
        public ushort MaxBackgroundWidth { get; set; }
        public ushort MaxBackgroundHeight { get; set; }

        // DRM for older versions
        public byte BiosCheckSum { get; set; }
        public byte VideoBiosCheckSum { get; set; }

        public byte PcxFilesCount { get; set; }
        public byte[] PcxFileIndexes { get; set; }
        public string[] PcxFileNames { get; set; }

        public WorldDefine WorldDefine { get; set; }

        public string[] DESFileNames { get; set; }
        public string[] ETAFileNames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            // Serialize PC Header
            if (settings.IsVersioned)
                GameVersion = s.SerializeObject<GameVersion>(GameVersion, name: nameof(GameVersion));

            // Serialize world data
            MaxBackgroundWidth = s.Serialize<ushort>(MaxBackgroundWidth, name: nameof(MaxBackgroundWidth));
            MaxBackgroundHeight = s.Serialize<ushort>(MaxBackgroundHeight, name: nameof(MaxBackgroundHeight));
            PcxFilesCount = s.Serialize<byte>(PcxFilesCount, name: nameof(PcxFilesCount));
            VideoBiosCheckSum = s.Serialize<byte>(VideoBiosCheckSum, name: nameof(VideoBiosCheckSum));
            BiosCheckSum = s.Serialize<byte>(BiosCheckSum, name: nameof(BiosCheckSum));

            if (settings.EngineVersion == Ray1EngineVersion.PC || settings.EngineVersion == Ray1EngineVersion.PocketPC)
                s.DoProcessed(new Xor8Processor(0x15), () => PcxFileIndexes = s.SerializeArray<byte>(PcxFileIndexes, PcxFilesCount, name: nameof(PcxFileIndexes)));
            else
                s.DoProcessed(new Xor8Processor(0x19), () => PcxFileNames = s.SerializeStringArray(PcxFileNames, PcxFilesCount, 8, name: nameof(PcxFileNames)));

            // Serialize the DES
            DesItemCount = s.Serialize<ushort>(DesItemCount, name: nameof(DesItemCount));

            DesItems = s.SerializeObjectArray<Design>(DesItems, DesItemCount, onPreSerialize: data => data.Pre_Type = Design.Type.World, name: nameof(DesItems));

            // Serialize the ETA
            Eta = s.SerializeArraySize<States, byte>(Eta, name: nameof(Eta));
            Eta = s.SerializeObjectArray<States>(Eta, Eta.Length, name: nameof(Eta));

            // Kit and EDU have more data...
            if (settings.EngineVersion == Ray1EngineVersion.PC_Kit || 
                settings.EngineVersion == Ray1EngineVersion.PC_Edu || 
                settings.EngineVersion == Ray1EngineVersion.PC_Fan)
            {
                s.DoProcessed(new Checksum8Processor(), p =>
                {
                    p.Serialize<byte>(s, "WorldDefineChecksum");

                    s.DoProcessed(new Xor8Processor(0x71), () =>
                    {
                        WorldDefine = s.SerializeObject<WorldDefine>(WorldDefine, name: nameof(WorldDefine));
                    });
                });

                // Serialize file tables
                if (settings.EngineVersion == Ray1EngineVersion.PC_Kit || settings.EngineVersion == Ray1EngineVersion.PC_Fan)
                {
                    DESFileNames = s.SerializeStringArray(DESFileNames, 100, 13, name: nameof(DESFileNames));
                    ETAFileNames = s.SerializeStringArray(ETAFileNames, 60, 13, name: nameof(ETAFileNames));
                }
            }
        }
    }
}