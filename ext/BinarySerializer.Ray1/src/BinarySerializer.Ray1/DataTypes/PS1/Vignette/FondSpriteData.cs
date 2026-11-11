using BinarySerializer.PlayStation.PS1;

namespace BinarySerializer.Ray1.PS1
{
    public class FondSpriteData : BinarySerializable
    {
        public byte SpritesCount { get; set; }
        public byte BackBandsCount { get; set; }
        public byte FrontBandsCount { get; set; }
        public byte PalettesCount { get; set; }
        public byte[] Bytes_05 { get; set; } // Seems to be unused by the game. Padding? Has data though.
        
        public BackgroundBandDefine[] BackBands { get; set; }
        public BackgroundBandDefine[] FrontBands { get; set; }

        public Clut[] Palettes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            SpritesCount = s.Serialize<byte>(SpritesCount, name: nameof(SpritesCount));
            s.SerializePadding(1, logIfNotNull: true);
            BackBandsCount = s.Serialize<byte>(BackBandsCount, name: nameof(BackBandsCount));
            FrontBandsCount = s.Serialize<byte>(FrontBandsCount, name: nameof(FrontBandsCount));

            if (settings.EngineVersion == Ray1EngineVersion.PS1_JP)
            {
                PalettesCount = SpritesCount;
            }
            else
            {
                PalettesCount = s.Serialize<byte>(PalettesCount, name: nameof(PalettesCount));
                Bytes_05 = s.SerializeArray<byte>(Bytes_05, 3, name: nameof(Bytes_05));
            }

            BackBands = s.SerializeObjectArray<BackgroundBandDefine>(BackBands, BackBandsCount, name: nameof(BackBands));
            FrontBands = s.SerializeObjectArray<BackgroundBandDefine>(FrontBands, FrontBandsCount, name: nameof(FrontBands));

            s.Align();

            Palettes = s.SerializeObjectArray<Clut>(Palettes, PalettesCount, name: nameof(Palettes));
        }
    }
}