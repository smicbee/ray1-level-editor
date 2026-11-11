namespace BinarySerializer.Ray1.PC
{
    public class LevelDefine : BinarySerializable
    {
        public uint CdTrackTime { get; set; } // Gets filled in during runtime
        public short AllowedTime { get; set; } // For timed levels
        public byte CdTrack { get; set; }
        public byte CurrentFnd { get; set; } // Gets set during runtime
        public byte Fnd { get; set; }
        public byte ScrollDiffFnd { get; set; }
        public byte WorldInfoIcon { get; set; } // ?
        public LevelEffects Effects { get; set; }
        public RayEvts RayEvts { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            bool isEncryptedAndChecksum = settings.EngineVersion != Ray1EngineVersion.PS1_Edu && settings.IsLoadingPackedPCData;

            s.DoProcessed(isEncryptedAndChecksum ? new Checksum8Processor() : null, p =>
            {
                p?.Serialize<byte>(s, "LevelDefineChecksum");

                s.DoProcessed(isEncryptedAndChecksum ? new Xor8Processor(0x57) : null, () =>
                {
                    CdTrackTime = s.Serialize<uint>(CdTrackTime, name: nameof(CdTrackTime));
                    AllowedTime = s.Serialize<short>(AllowedTime, name: nameof(AllowedTime));
                    CdTrack = s.Serialize<byte>(CdTrack, name: nameof(CdTrack));
                    CurrentFnd = s.Serialize<byte>(CurrentFnd, name: nameof(CurrentFnd));
                    Fnd = s.Serialize<byte>(Fnd, name: nameof(Fnd));
                    ScrollDiffFnd = s.Serialize<byte>(ScrollDiffFnd, name: nameof(ScrollDiffFnd));
                    WorldInfoIcon = s.Serialize<byte>(WorldInfoIcon, name: nameof(WorldInfoIcon));
                    Effects = s.Serialize<LevelEffects>(Effects, name: nameof(Effects));
                    RayEvts = s.SerializeObject<RayEvts>(RayEvts, name: nameof(RayEvts));
                });
            });
        }
    }
}