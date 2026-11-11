using BinarySerializer.Nintendo.GBA;
using System.Text;

namespace BinarySerializer.Ray1.GBA
{
    /// <summary>
    /// ROM data
    /// </summary>
    public class GBA_ROM : ROMBase, IGBAData
    {
        /// <summary>
        /// The map data for the current level
        /// </summary>
        public GBA_LevelMapData LevelMapData { get; set; }

        /// <summary>
        /// The event data for the current level
        /// </summary>
        public GBA_LevelEventData LevelEventData { get; set; }


        /// <summary>
        /// The background vignette data
        /// </summary>
        public GBA_BackgroundVignette[] BackgroundVignettes { get; set; }

        /// <summary>
        /// The intro vignette data
        /// </summary>
        public GBA_IntroVignette[] IntroVignettes { get; set; }

        /// <summary>
        /// The world map vignette
        /// </summary>
        public GBA_WorldMapVignette WorldMapVignette { get; set; }


        /// <summary>
        /// The sprite palettes. The game uses the same 16 palettes (with 16 colors) for every sprite in the game. During runtime this gets copied to 0x05000200.
        /// </summary>
        public RGBA5551Color[] SpritePalettes { get; set; }

        /// <summary>
        /// The sprite palette for the current level
        /// </summary>
        /// <param name="settings">The game settings</param>
        public RGBA5551Color[] GetSpritePalettes(Ray1Settings settings) => SpritePalettes; 


        /// <summary>
        /// World level index offset table for global level array
        /// </summary>
        public byte[] WorldLevelOffsetTable { get; set; }


        public Pointer[] StringPointerTable { get; set; }

        /// <summary>
        /// The strings
        /// </summary>
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

            // Serialize ROM header
            base.SerializeImpl(s);

            s.DoAt(s.GetRequiredPreDefinedPointer(GBA_DefinedPointer.WorldLevelOffsetTable),
                () => WorldLevelOffsetTable = s.SerializeArray<byte>(WorldLevelOffsetTable, 12, name: nameof(WorldLevelOffsetTable)));

            // Get the global level index
            var levelIndex = WorldLevelOffsetTable[(int)settings.World] + (settings.Level - 1);

            // Hardcode files
            if (s is BinaryDeserializer && settings.World == World.Image && settings.Level == 4) 
            {
                s.DoAt(Offset + 0x55cc, () => {
                    void CreateFakeFile(int index, int size) 
                    {
                        uint memAddress = s.Serialize<uint>(default, name: nameof(memAddress));
                        Pointer off_array = s.SerializePointer(default, name: nameof(off_array));
                        var bytes = s.DoAt(off_array, () => s.SerializeArray<byte>(default, size, name: "bytes"));

                        if (bytes != null) 
                        {
                            var file = new MemoryMappedStreamFile(s.Context, "ETA_" + index, memAddress, bytes);

                            s.Context.AddFile(file);
                        }
                    }
                    CreateFakeFile(0, 0x58);
                    CreateFakeFile(1, 0x8);
                    CreateFakeFile(2, 0x78);
                    CreateFakeFile(3, 0x210);
                    CreateFakeFile(4, 0x60);
                    CreateFakeFile(5, 0x110);
                    CreateFakeFile(6, 0x1a8);
                    CreateFakeFile(7, 0x70);
                    CreateFakeFile(8, 0x70);
                });
            }

            DES_Ray = s.DoAt(s.GetRequiredPreDefinedPointer(GBA_DefinedPointer.DES_Ray), () => s.SerializeObject<GBA_EventGraphicsData>(DES_Ray, name: nameof(DES_Ray)));
            ETA_Ray = s.DoAt(s.GetPreDefinedPointer(GBA_DefinedPointer.ETA_Ray), () => s.SerializeObject<GBA_ETA>(ETA_Ray, x => x.Pre_Lengths = new byte[] { 66, 12, 34, 53, 14, 14, 1, 2 }, name: nameof(ETA_Ray)));

            DES_RayLittle = s.DoAt(s.GetRequiredPreDefinedPointer(GBA_DefinedPointer.DES_RayLittle), () => s.SerializeObject<GBA_EventGraphicsData>(DES_RayLittle, name: nameof(DES_RayLittle)));

            DES_Clock = s.DoAt(s.GetRequiredPreDefinedPointer(GBA_DefinedPointer.DES_Clock), () => s.SerializeObject<GBA_EventGraphicsData>(DES_Clock, name: nameof(DES_Clock)));
            ETA_Clock = s.DoAt(s.GetPreDefinedPointer(GBA_DefinedPointer.ETA_Clock), () => s.SerializeObject<GBA_ETA>(ETA_Clock, x => x.Pre_Lengths = new byte[] { 3 }, name: nameof(ETA_Clock)));

            DES_Div = s.DoAt(s.GetRequiredPreDefinedPointer(GBA_DefinedPointer.DES_Div), () => s.SerializeObject<GBA_EventGraphicsData>(DES_Div, name: nameof(DES_Div)));
            ETA_Div = s.DoAt(s.GetPreDefinedPointer(GBA_DefinedPointer.ETA_Div), () => s.SerializeObject<GBA_ETA>(ETA_Div, x => x.Pre_Lengths = new byte[] { 1, 1, 1, 1, 1, 1, 2, 2, 12, 12, 4 }, name: nameof(ETA_Div)));

            DES_Map = s.DoAt(s.GetRequiredPreDefinedPointer(GBA_DefinedPointer.DES_Map), () => s.SerializeObject<GBA_EventGraphicsData>(DES_Map, name: nameof(DES_Map)));
            ETA_Map = s.DoAt(s.GetPreDefinedPointer(GBA_DefinedPointer.ETA_Map), () => s.SerializeObject<GBA_ETA>(ETA_Map, x => x.Pre_Lengths = new byte[] { 64, 1, 19, 1, 1, 69, 3 }, name: nameof(ETA_Map)));

            // Serialize data from the ROM
            if (settings.World != World.Menu)
                s.DoAt(s.GetRequiredPreDefinedPointer(GBA_DefinedPointer.LevelMaps) + (levelIndex * 28),
                    () => LevelMapData = s.SerializeObject<GBA_LevelMapData>(LevelMapData, name: nameof(LevelMapData)));

            s.DoAt(s.GetRequiredPreDefinedPointer(GBA_DefinedPointer.BackgroundVignette), 
                () => BackgroundVignettes = s.SerializeObjectArray<GBA_BackgroundVignette>(BackgroundVignettes, 48, name: nameof(BackgroundVignettes)));
            s.DoAt(s.GetRequiredPreDefinedPointer(GBA_DefinedPointer.IntroVignette), 
                () => IntroVignettes = s.SerializeObjectArray<GBA_IntroVignette>(IntroVignettes, 14, name: nameof(IntroVignettes)));
            WorldMapVignette = s.SerializeObject<GBA_WorldMapVignette>(WorldMapVignette, name: nameof(WorldMapVignette));

            s.DoAt(s.GetRequiredPreDefinedPointer(GBA_DefinedPointer.SpritePalettes), 
                () => SpritePalettes = s.SerializeObjectArray<RGBA5551Color>(SpritePalettes, 16 * 16, name: nameof(SpritePalettes)));

            if (settings.World != World.Menu)
            {
                // Serialize the level event data
                LevelEventData = new GBA_LevelEventData();
                LevelEventData.SerializeData(s, s.GetRequiredPreDefinedPointer(GBA_DefinedPointer.EventGraphicsPointers), s.GetRequiredPreDefinedPointer(GBA_DefinedPointer.EventDataPointers), s.GetRequiredPreDefinedPointer(GBA_DefinedPointer.EventGraphicsGroupCountTablePointers), s.GetRequiredPreDefinedPointer(GBA_DefinedPointer.LevelEventGraphicsGroupCounts), levelIndex);
            }

            // Serialize strings
            s.DoAt(s.GetRequiredPreDefinedPointer(GBA_DefinedPointer.StringPointers), () =>
            {
                StringPointerTable = s.SerializePointerArray(StringPointerTable, 5 * 259, name: nameof(StringPointerTable));

                // NOTE: There is data for two leftover languages from the PC version (Japanese and Chinese) but they have incorrect lengths, so we don't parse them
                Strings ??= new string[5][];

                var enc = new Encoding[]
                {
                    // English
                    Encoding.GetEncoding(437),
                    // French
                    Encoding.GetEncoding(1252),
                    // German
                    Encoding.GetEncoding(437),
                    // Spanish
                    Encoding.GetEncoding(1252),
                    // Italian
                    Encoding.GetEncoding(1252),
                };

                for (int i = 0; i < Strings.Length; i++)
                {
                    Strings[i] ??= new string[259];

                    for (int j = 0; j < Strings[i].Length; j++)
                    {
                        s.DoAt(StringPointerTable[i * 259 + j], () => Strings[i][j] = s.SerializeString(Strings[i][j], encoding: enc[i], name: $"{nameof(Strings)}[{i}][{j}]"));
                    }
                }
            });

            // Serialize tables
            s.DoAt(s.GetRequiredPreDefinedPointer(GBA_DefinedPointer.TypeZDC), () => TypeZDC = s.SerializeObjectArray<ZDCReference>(TypeZDC, 262, name: nameof(TypeZDC)));
            s.DoAt(s.GetRequiredPreDefinedPointer(GBA_DefinedPointer.ZdcData), () => ZdcData = s.SerializeObjectArray<ZDCBox>(ZdcData, 200, name: nameof(ZdcData)));
            s.DoAt(s.GetRequiredPreDefinedPointer(GBA_DefinedPointer.EventFlags), () => EventFlags = s.SerializeObjectArray<ObjTypeFlags>(EventFlags, 262, name: nameof(EventFlags)));

            WorldVignetteIndicesPointers = s.DoAt(s.GetRequiredPreDefinedPointer(GBA_DefinedPointer.WorldVignetteIndices), () => s.SerializePointerArray(WorldVignetteIndicesPointers, 9, name: nameof(WorldVignetteIndicesPointers)));
            WorldVignetteIndices = s.DoAt(WorldVignetteIndicesPointers[(int)settings.World], () => s.SerializeArray<byte>(WorldVignetteIndices, 8, name: nameof(WorldVignetteIndices))); // The max size is 8

            WorldInfos = s.DoAt(s.GetRequiredPreDefinedPointer(GBA_DefinedPointer.WorldInfo), () => s.SerializeObjectArray<WorldInfo>(WorldInfos, 24, name: nameof(WorldInfos)));
        }
    }

    /*

    ARRAYS:
     
    Pointer array at 0x086DCE14 - 60 items
    ushort array at 0x08549774
    
    uint array at 0x08549674
    uint array at 0x0854925E
    ushort array at 0x08549200
        
    (these might begin 1 byte earlier)
    byte[4] array at 0x0854925C    


    SPLASH SCREENS:

    16 palettes for Ubi logo are     at 0x086EEDD8
    16 palettes for Eclipse logo are at 0x086EEFD8
    16 palettes for Rayman logo are  at 0x086EF188

    0x086DEC00 has 6 pointers. First 3 to image data and last 3 to the palettes for Ubi, Eclipse and RayLogo - where are the index tables?


    LOADING + CREDITS SCREENS:

    Palettes don't seem to exist in the rom - compressed?


    MEMORY LOCATIONS:

    0x02030394 - current world
    0x0202FB7C - current level
    0x0202A2DA - some array where first two bytes are used to compare level and world somehow
    0x020226B0 - events
    0x0202D408 - link table

    0x020226A8 - multiplayer flag
    0x0202a588 - multiplayer flag count (p1, p2)

     */
}