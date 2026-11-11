using System.Collections.Generic;

namespace BinarySerializer.Ray1.GBA
{
    public static class DSi_DefinedPointers
    {
        public static Dictionary<DSi_DefinedPointer, long> DSi => new Dictionary<DSi_DefinedPointer, long>()
        {
            [DSi_DefinedPointer.JungleMaps] = 0x0226C6B4,
            [DSi_DefinedPointer.LevelMaps] = 0x02361968,
            [DSi_DefinedPointer.BackgroundVignette] = 0x025A1478,
            [DSi_DefinedPointer.WorldMapVignette] = 0x021E17FC,
            [DSi_DefinedPointer.SpecialPalettes] = 0x02268FEC,

            [DSi_DefinedPointer.StringPointers] = 0x022604D0,

            [DSi_DefinedPointer.TypeZDC] = 0x0225F73C,
            [DSi_DefinedPointer.ZdcData] = 0x02262398,
            [DSi_DefinedPointer.EventFlags] = 0x022600B8,

            [DSi_DefinedPointer.WorldInfo] = 0x0225F144,
            [DSi_DefinedPointer.WorldVignetteIndices] = 0x02236ED8,
            [DSi_DefinedPointer.LevelMapsBGIndices] = 0x02913F94,

            [DSi_DefinedPointer.WorldLevelOffsetTable] = 0x02236BF8,

            [DSi_DefinedPointer.EventGraphicsPointers] = 0x0284B5B0,
            [DSi_DefinedPointer.EventDataPointers] = 0x0284B6F8,
            [DSi_DefinedPointer.EventGraphicsGroupCountTablePointers] = 0x0284B988,
            [DSi_DefinedPointer.LevelEventGraphicsGroupCounts] = 0x0284B840,

            [DSi_DefinedPointer.DES_Ray] = 0x02815BF4,
            [DSi_DefinedPointer.DES_RayLittle] = 0x02815DA4,
            [DSi_DefinedPointer.DES_Clock] = 0x0281BA8C,
            [DSi_DefinedPointer.DES_Div] = 0x02816224,
            [DSi_DefinedPointer.DES_Map] = 0x02815A20,

            [DSi_DefinedPointer.ETA_Ray] = 0x02814264,
            //[R1_DSi_Pointer.ETA_Clock] = ,
            [DSi_DefinedPointer.ETA_Div] = 0x02816C74,
            [DSi_DefinedPointer.ETA_Map] = 0x02813048,
        };
    }
}