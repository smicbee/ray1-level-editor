namespace BinarySerializer.Ray1
{
    // NOTE: Rayman 2 has an updated struct for this with 50 items in the map define (and other changes)
    public class WorldMap : BinarySerializable
    {
        public byte PelletsCount { get; set; }
        public byte CdTrack { get; set; }
        public WorldMapDisplayType DisplayType { get; set; }
        public string Background { get; set; }
        public string SelectLevelBackground { get; set; }
        public byte WorldsCount { get; set; }
        public byte[] LevelCounts { get; set; }
        public byte SelectedPellete { get; set; }
        public short SaveGameString { get; set; }
        public short GameSavedString { get; set; }
        public short NoSaveGameString { get; set; }

        public WorldInfo[] MapDefine { get; set; } // First entry is always the start point
        public ParcheminDefine ParcheminDefine { get; set; }
        public HelpDefine HelpDefine { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            PelletsCount = s.Serialize<byte>(PelletsCount, name: nameof(PelletsCount));
            s.SerializePadding(1, logIfNotNull: true);

            CdTrack = s.Serialize<byte>(CdTrack, name: nameof(CdTrack));
            DisplayType = s.Serialize<WorldMapDisplayType>(DisplayType, name: nameof(DisplayType));
            Background = s.SerializeString(Background, 9, name: nameof(Background));
            SelectLevelBackground = s.SerializeString(SelectLevelBackground, 9, name: nameof(SelectLevelBackground));
            WorldsCount = s.Serialize<byte>(WorldsCount, name: nameof(WorldsCount));
            LevelCounts = s.SerializeArray<byte>(LevelCounts, 6, name: nameof(LevelCounts));
            SelectedPellete = s.Serialize<byte>(SelectedPellete, name: nameof(SelectedPellete));
            SaveGameString = s.Serialize<short>(SaveGameString, name: nameof(SaveGameString));
            GameSavedString = s.Serialize<short>(GameSavedString, name: nameof(GameSavedString));
            NoSaveGameString = s.Serialize<short>(NoSaveGameString, name: nameof(NoSaveGameString));

            MapDefine = s.SerializeObjectArray<WorldInfo>(MapDefine, 32, name: nameof(MapDefine));
            ParcheminDefine = s.SerializeObject<ParcheminDefine>(ParcheminDefine, name: nameof(ParcheminDefine));
            HelpDefine = s.SerializeObject<HelpDefine>(HelpDefine, name: nameof(HelpDefine));

            // Serialize packed running demos data
            if (settings.IsLoadingPackedPCData)
            {
                for (int i = 0; i < PelletsCount; i++)
                {
                    int length = s.Serialize<int>(MapDefine[i].RunningDemo?.InputsBufferLength ?? 0, name: "RunningDemoBufferLength");

                    if (length > 0)
                    {
                        MapDefine[i].RunningDemo = s.SerializeObject<Record>(MapDefine[i].RunningDemo, name: $"{nameof(WorldInfo.RunningDemo)}[{i}]");
                    }
                }
            }
        }
    }
}