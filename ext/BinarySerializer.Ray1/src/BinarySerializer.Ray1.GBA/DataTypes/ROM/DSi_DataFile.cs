using System.Linq;
using System.Text;

namespace BinarySerializer.Ray1.GBA
{
    public class DSi_DataFile : BinarySerializable, IGBAData
    {
        /// <summary>
        /// The map data for the current level
        /// </summary>
        public GBA_LevelMapData LevelMapData { get; set; }

        /// <summary>
        /// The event data for the current level
        /// </summary>
        public GBA_LevelEventData LevelEventData { get; set; }

        public DSi_PaletteReference[] Palettes { get; set; }

        /// <summary>
        /// The sprite palette for the current level
        /// </summary>
        /// <param name="settings">The game settings</param>
        public RGBA5551Color[] GetSpritePalettes(Ray1Settings settings)
        {
            DSi_PaletteReference palRef;
            switch (settings.World)
            {
                default:
                case World.Jungle:
                    palRef = Palettes.FirstOrDefault(p => p.Name == "PALETTE_ray");
                    break;
                case World.Music:
                    palRef = Palettes.FirstOrDefault(p => p.Name == "PALETTE_mus");
                    break;
                case World.Mountain:
                    palRef = Palettes.FirstOrDefault(p => p.Name == "PALETTE_mnt");
                    // NOTE: There's a mnt2. It appears to be unused?
                    break;
                case World.Image:
                    palRef = Palettes.FirstOrDefault(p => p.Name == "PALETTE_img");
                    break;
                case World.Cave:
                    palRef = Palettes.FirstOrDefault(p => p.Name == "PALETTE_cav");
                    break;
                case World.Cake:
                    palRef = Palettes.FirstOrDefault(p => p.Name == "PALETTE_ray");
                    break;
            }
            return palRef?.Palette;
        }

        /// <summary>
        /// The background vignette data
        /// </summary>
        public GBA_BackgroundVignette[] BackgroundVignettes { get; set; }

        // TODO: Parse these from data
        public GBA_IntroVignette[] IntroVignettes => null;
        public GBA_WorldMapVignette WorldMapVignette { get; set; }

        public byte[] WorldLevelOffsetTable { get; set; }

        public Pointer[] StringPointerTable { get; set; }
        public string[][] Strings { get; set; }

        public ZDCReference[] TypeZDC { get; set; }
        public ZDCBox[] ZdcData { get; set; }
        public ObjTypeFlags[] EventFlags { get; set; }

        public Pointer[] WorldVignetteIndicesPointers { get; set; }
        public byte[] WorldVignetteIndices { get; set; }

        public WorldInfo[] WorldInfos { get; set; }

        public GBA_EventGraphicsData DES_Ray { get; set; }
        public GBA_EventGraphicsData DES_RayLittle { get; set; }
        public GBA_EventGraphicsData DES_Clock { get; set; }
        public GBA_EventGraphicsData DES_Div { get; set; }
        public GBA_EventGraphicsData DES_Map { get; set; }
        public GBA_ETA ETA_Ray { get; set; }
        public GBA_ETA ETA_Clock { get; set; }
        public GBA_ETA ETA_Div { get; set; }
        public GBA_ETA ETA_Map { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            var settings = s.GetRequiredSettings<Ray1Settings>();

            s.DoAt(s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.WorldLevelOffsetTable),
                () => WorldLevelOffsetTable = s.SerializeArray<byte>(WorldLevelOffsetTable, 8, name: nameof(WorldLevelOffsetTable)));

            // Get the global level index
            var levelIndex = WorldLevelOffsetTable[(int)settings.World] + (settings.Level - 1);

            DES_Ray = s.DoAt(s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.DES_Ray), () => s.SerializeObject<GBA_EventGraphicsData>(DES_Ray, name: nameof(DES_Ray)));
            ETA_Ray = s.DoAt(s.GetPreDefinedPointer(DSi_DefinedPointer.ETA_Ray), () => s.SerializeObject<GBA_ETA>(ETA_Ray, x => x.Pre_Lengths = new byte[] { 66, 12, 34, 53, 14, 14, 1, 2 }, name: nameof(ETA_Ray)));

            DES_RayLittle = s.DoAt(s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.DES_RayLittle), () => s.SerializeObject<GBA_EventGraphicsData>(DES_RayLittle, name: nameof(DES_RayLittle)));

            DES_Clock = s.DoAt(s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.DES_Clock), () => s.SerializeObject<GBA_EventGraphicsData>(DES_Clock, name: nameof(DES_Clock)));
            ETA_Clock = s.DoAt(s.GetPreDefinedPointer(DSi_DefinedPointer.ETA_Clock), () => s.SerializeObject<GBA_ETA>(ETA_Clock, x => x.Pre_Lengths = new byte[] { 3 }, name: nameof(ETA_Clock)));

            DES_Div = s.DoAt(s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.DES_Div), () => s.SerializeObject<GBA_EventGraphicsData>(DES_Div, name: nameof(DES_Div)));
            ETA_Div = s.DoAt(s.GetPreDefinedPointer(DSi_DefinedPointer.ETA_Div), () => s.SerializeObject<GBA_ETA>(ETA_Div, x => x.Pre_Lengths = new byte[] { 1, 1, 1, 1, 1, 1, 2, 2, 12, 12, 4 }, name: nameof(ETA_Div)));

            DES_Map = s.DoAt(s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.DES_Map), () => s.SerializeObject<GBA_EventGraphicsData>(DES_Map, name: nameof(DES_Map)));
            ETA_Map = s.DoAt(s.GetPreDefinedPointer(DSi_DefinedPointer.ETA_Map), () => s.SerializeObject<GBA_ETA>(ETA_Map, x => x.Pre_Lengths = new byte[] { 64, 1, 19, 1, 1, 69, 3 }, name: nameof(ETA_Map)));

            // Serialize data from the ROM
            if (settings.World != World.Menu)
                s.DoAt((settings.World == World.Jungle ? s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.JungleMaps) : s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.LevelMaps)) + (levelIndex * 32), 
                    () => LevelMapData = s.SerializeObject<GBA_LevelMapData>(LevelMapData, name: nameof(LevelMapData)));

            s.DoAt(s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.BackgroundVignette),
                () => BackgroundVignettes = s.SerializeObjectArray<GBA_BackgroundVignette>(BackgroundVignettes, 48, name: nameof(BackgroundVignettes)));

            WorldMapVignette = s.SerializeObject<GBA_WorldMapVignette>(WorldMapVignette, name: nameof(WorldMapVignette));

            // Serialize the level event data
            if (settings.World != World.Menu)
            {
                LevelEventData = new GBA_LevelEventData();
                LevelEventData.SerializeData(s, s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.EventGraphicsPointers), s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.EventDataPointers), s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.EventGraphicsGroupCountTablePointers), s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.LevelEventGraphicsGroupCounts), levelIndex);
            }

            s.DoAt(s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.SpecialPalettes), () => Palettes = s.SerializeObjectArray<DSi_PaletteReference>(Palettes, 10, name: nameof(Palettes)));

            // Serialize strings
            s.DoAt(s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.StringPointers), () =>
            {
                StringPointerTable = s.SerializePointerArray(StringPointerTable, 5 * 394, name: nameof(StringPointerTable));
                
                Strings ??= new string[5][];

                var enc = new Encoding[]
                {
                    // Spanish
                    Encoding.GetEncoding(1252),
                    // English
                    Encoding.GetEncoding(437),
                    // French
                    Encoding.GetEncoding(1252),
                    // Italian
                    Encoding.GetEncoding(1252),
                    // German
                    Encoding.GetEncoding(437),
                };

                for (int i = 0; i < Strings.Length; i++)
                {
                    Strings[i] ??= new string[394];

                    for (int j = 0; j < Strings[i].Length; j++)
                    {
                        s.DoAt(StringPointerTable[i * 394 + j], () => Strings[i][j] = s.SerializeString(Strings[i][j], encoding: enc[i], name: $"{nameof(Strings)}[{i}][{j}]"));
                    }
                }
            });

            // Serialize tables
            s.DoAt(s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.TypeZDC), () => TypeZDC = s.SerializeObjectArray<ZDCReference>(TypeZDC, 262, name: nameof(TypeZDC)));
            s.DoAt(s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.ZdcData), () => ZdcData = s.SerializeObjectArray<ZDCBox>(ZdcData, 200, name: nameof(ZdcData)));
            s.DoAt(s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.EventFlags), () => EventFlags = s.SerializeObjectArray<ObjTypeFlags>(EventFlags, 262, name: nameof(EventFlags)));

            if (settings.World != World.Menu)
            {
                WorldVignetteIndicesPointers = s.DoAt(s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.WorldVignetteIndices), () => s.SerializePointerArray(WorldVignetteIndicesPointers, 7, name: nameof(WorldVignetteIndicesPointers)));
                WorldVignetteIndices = s.DoAt(WorldVignetteIndicesPointers[(int)settings.World], () => s.SerializeArray<byte>(WorldVignetteIndices, 8, name: nameof(WorldVignetteIndices))); // The max size is 8

                // Get the background indices
                s.DoAt(s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.LevelMapsBGIndices) + (levelIndex * 32), () =>
                {
                    LevelMapData.GBA_Byte_14 = s.Serialize<byte>(LevelMapData.GBA_Byte_14, name: nameof(LevelMapData.GBA_Byte_14));
                    LevelMapData.GBA_Byte_15 = s.Serialize<byte>(LevelMapData.GBA_Byte_15, name: nameof(LevelMapData.GBA_Byte_15));
                    LevelMapData.BackgroundIndex = s.Serialize<byte>(LevelMapData.BackgroundIndex, name: nameof(LevelMapData.BackgroundIndex));
                    LevelMapData.ParallaxBackgroundIndex = s.Serialize<byte>(LevelMapData.ParallaxBackgroundIndex, name: nameof(LevelMapData.ParallaxBackgroundIndex));
                });
            }

            WorldInfos = s.DoAt(s.GetRequiredPreDefinedPointer(DSi_DefinedPointer.WorldInfo), () => s.SerializeObjectArray<WorldInfo>(WorldInfos, 24, name: nameof(WorldInfos)));
        }
    }
}