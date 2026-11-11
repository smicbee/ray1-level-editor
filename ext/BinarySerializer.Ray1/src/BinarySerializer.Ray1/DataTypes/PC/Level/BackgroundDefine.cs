namespace BinarySerializer.Ray1.PC
{
    public class BackgroundDefine : BinarySerializable
    {
        public BackgroundType Type { get; set; }
        public byte NormalScrollType { get; set; }
        public byte BandsCount { get; set; }
        public byte SpritesCount { get; set; }
        public BackgroundBandDefine[] Bands { get; set; }
        public BackgroundSpriteDefine[] Sprites { get; set; }
        public byte ScrollYType { get; set; }
        public byte ScrollXType { get; set; }
        public byte RepetitionSpeedY { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();
            bool isEncryptedAndChecksum = settings.EngineVersion != Ray1EngineVersion.PS1_Edu && settings.IsLoadingPackedPCData;

            s.DoProcessed(isEncryptedAndChecksum ? new Checksum8Processor() : null, p =>
            {
                p?.Serialize<byte>(s, "BackgroundDefineChecksum");

                s.DoProcessed(isEncryptedAndChecksum ? new Xor8Processor(0xA5) : null, () =>
                {
                    Type = s.Serialize<BackgroundType>(Type, name: nameof(Type));
                    NormalScrollType = s.Serialize<byte>(NormalScrollType, name: nameof(NormalScrollType));
                    BandsCount = s.Serialize<byte>(BandsCount, name: nameof(BandsCount));
                    SpritesCount = s.Serialize<byte>(SpritesCount, name: nameof(SpritesCount));
                    Bands = s.SerializeObjectArray<BackgroundBandDefine>(Bands, 1, name: nameof(Bands));
                    Sprites = s.SerializeObjectArray<BackgroundSpriteDefine>(Sprites, 1, name: nameof(Sprites));
                    ScrollYType = s.Serialize<byte>(ScrollYType, name: nameof(ScrollYType));
                    ScrollXType = s.Serialize<byte>(ScrollXType, name: nameof(ScrollXType));
                    RepetitionSpeedY = s.Serialize<byte>(RepetitionSpeedY, name: nameof(RepetitionSpeedY));
                    s.SerializePadding(1, logIfNotNull: true);
                });
            });
        }
    }
}