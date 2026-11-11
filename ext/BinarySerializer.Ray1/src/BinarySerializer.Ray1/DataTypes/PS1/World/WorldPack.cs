using BinarySerializer.PlayStation.PS1;

namespace BinarySerializer.Ray1.PS1
{
    /// <summary>
    /// An XXX world pack
    /// </summary>
    public class WorldPack : Pack
    {
        public byte[] DataBlock { get; set; }
        public byte[] SoundData { get; set; } // TODO: Parse
        public byte[] ImageData { get; set; }

        public Clut Palette1 { get; set; }
        public Clut Palette2 { get; set; }

        public RGBA5551Color[] JP_RawTiles { get; set; }

        public byte[] PalettedTiles { get; set; }
        public Clut[] TilePalettes { get; set; }
        public byte[] TilePaletteIndexTable { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            // Serialize header
            base.SerializeImpl(s);

            // Serialize files
            SerializeFile(s, 0, length => DataBlock = s.SerializeArray<byte>(DataBlock, length, name: nameof(DataBlock)));
            SerializeFile(s, 1, length => SoundData = s.SerializeArray<byte>(SoundData, length, name: nameof(SoundData)));
            SerializeFile(s, 2, length => ImageData = s.SerializeArray<byte>(ImageData, length, name: nameof(ImageData)));
            SerializeFile(s, 3, _ => Palette1 = s.SerializeObject<Clut>(Palette1, name: nameof(Palette1)));
            SerializeFile(s, 4, _ => Palette2 = s.SerializeObject<Clut>(Palette2, name: nameof(Palette2)));

            if (settings.EngineVersion is Ray1EngineVersion.PS1 or Ray1EngineVersion.PS1_EUDemo)
            {
                SerializeFile(s, 5, length => PalettedTiles = s.SerializeArray<byte>(PalettedTiles, length, name: nameof(PalettedTiles)));
                SerializeFile(s, 6, length => TilePalettes = s.SerializeObjectArray<Clut>(TilePalettes, length / (256 * 2), name: nameof(TilePalettes)));
                SerializeFile(s, 7, length => TilePaletteIndexTable = s.SerializeArray<byte>(TilePaletteIndexTable, length, name: nameof(TilePaletteIndexTable)));
            }
            else if (settings.EngineVersion == Ray1EngineVersion.PS1_JP)
            {
                SerializeFile(s, 5, length =>
                {
                    long tileCount = JP_RawTiles?.Length ?? length / 2;
                    JP_RawTiles = s.SerializeObjectArray<RGBA5551Color>(JP_RawTiles, tileCount, name: nameof(JP_RawTiles));
                });
            }

            // Go to the end of the pack
            s.Goto(Offset + PackSize);
        }
    }
}