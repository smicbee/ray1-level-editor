namespace BinarySerializer.Ray1.PC
{
    public class GeneralScript : BinarySerializable
    {
        public int WorldMapNumber { get; set; } // Determines the file name of the WLDMAP file

        public CdTracks CdTracks { get; set; }
        public GameInfo GameInfo { get; set; }
        public ChatSamples ChatSamples { get; set; }
        public RecordedDemos RecordedDemos { get; set; }

        public int CreditsCount { get; set; }
        public Credit[] Credits { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            WorldMapNumber = s.Serialize<int>(WorldMapNumber, name: nameof(WorldMapNumber));
            CdTracks = s.SerializeObject<CdTracks>(CdTracks, name: nameof(CdTracks));
            GameInfo = s.SerializeObject<GameInfo>(GameInfo, name: nameof(GameInfo));
            ChatSamples = s.SerializeObject<ChatSamples>(ChatSamples, name: nameof(ChatSamples));
            RecordedDemos = s.SerializeObject<RecordedDemos>(RecordedDemos, name: nameof(RecordedDemos));

            CreditsCount = s.Serialize<int>(CreditsCount, name: nameof(CreditsCount));
            Credits = s.SerializeObjectArray<Credit>(Credits, CreditsCount, name: nameof(Credits));
        }
    }
}