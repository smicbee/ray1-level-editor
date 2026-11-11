namespace BinarySerializer.Ray1.PC
{
    /// <summary>
    /// Map data for PC
    /// </summary>
    public class MapInfo : BinarySerializable
    {
        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public RGB666Color[][] Palettes { get; set; }
        public byte LastPlan1Palette { get; set; } // Always 2

        public Block[] Blocks { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();
            bool hasChecksum = settings.EngineVersion is Ray1EngineVersion.PC_Kit or Ray1EngineVersion.PC_Fan or Ray1EngineVersion.PC_Edu;

            s.DoProcessed(hasChecksum ? new Checksum8Processor() : null, p =>
            {
                p?.Serialize<byte>(s, "MapBlockChecksum");

                // Serialize map size
                Width = s.Serialize<ushort>(Width, name: nameof(Width));
                Height = s.Serialize<ushort>(Height, name: nameof(Height));

                // Serialize palettes
                int palettesCount = settings.EngineVersion is Ray1EngineVersion.PC_Kit or Ray1EngineVersion.PC_Fan ? 1 : 3;
                Palettes = s.InitializeArray(Palettes, palettesCount);
                s.DoArray(Palettes, (x, _, name) => s.SerializeObjectArray<RGB666Color>(x, 256, name: name), name: nameof(Palettes));

                // Serialize last palette
                LastPlan1Palette = s.Serialize<byte>(LastPlan1Palette, name: nameof(LastPlan1Palette));

                // Serialize the map blocks
                Blocks = s.SerializeObjectArray<Block>(Blocks, Height * Width, name: nameof(Blocks));
            });
        }
    }
}