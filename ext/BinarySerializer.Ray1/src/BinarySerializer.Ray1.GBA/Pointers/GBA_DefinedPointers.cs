using System.Collections.Generic;
using System.Linq;

namespace BinarySerializer.Ray1.GBA
{
    public static class GBA_DefinedPointers
    {
        public static Dictionary<GBA_DefinedPointer, long> GBA_US => new Dictionary<GBA_DefinedPointer, long>()
        {
            [GBA_DefinedPointer.LevelMaps] = 0x08548688,
            [GBA_DefinedPointer.BackgroundVignette] = 0x086D4E34,
            [GBA_DefinedPointer.IntroVignette] = 0x080F7A04,
            [GBA_DefinedPointer.SpritePalettes] = 0x0854902A,

            [GBA_DefinedPointer.EventGraphicsPointers] = 0x081A63B8,
            [GBA_DefinedPointer.EventDataPointers] = 0x081A6518,
            [GBA_DefinedPointer.EventGraphicsGroupCountTablePointers] = 0x081A6678,
            [GBA_DefinedPointer.LevelEventGraphicsGroupCounts] = 0x081A67D8,
            
            [GBA_DefinedPointer.WorldLevelOffsetTable] = 0x08153A40,

            [GBA_DefinedPointer.WorldInfo] = 0x0854A530,
            [GBA_DefinedPointer.WorldMapVignetteImageData] = 0x081452A4,
            [GBA_DefinedPointer.WorldMapVignetteBlockIndices] = 0x08151504,
            [GBA_DefinedPointer.WorldMapVignettePaletteIndices] = 0x08152284,
            [GBA_DefinedPointer.WorldMapVignettePalettes] = 0x08152944,

            [GBA_DefinedPointer.StringPointers] = 0x0854ADB4,

            [GBA_DefinedPointer.TypeZDC] = 0x0854A304,
            [GBA_DefinedPointer.ZdcData] = 0x08549CC4,
            [GBA_DefinedPointer.EventFlags] = 0x08549330,

            [GBA_DefinedPointer.WorldVignetteIndices] = 0x08153A1C,

            [GBA_DefinedPointer.DES_Ray] = 0x0835F9B4,
            [GBA_DefinedPointer.DES_RayLittle] = 0x0835F9D8,
            [GBA_DefinedPointer.DES_Clock] = 0x082C90C8,
            [GBA_DefinedPointer.DES_Div] = 0x082D1D98,
            [GBA_DefinedPointer.DES_Map] = 0x0832A2F0,

            [GBA_DefinedPointer.ETA_Ray] = 0x0832D234,
            //[R1_GBA_ROMPointer.ETA_Clock] = ,
            [GBA_DefinedPointer.ETA_Div] = 0x082CC884,
            [GBA_DefinedPointer.ETA_Map] = 0x082ED3B8,

            [GBA_DefinedPointer.DES_Alpha] = 0x082E74F4,
            [GBA_DefinedPointer.DES_Alpha2] = 0x082E7514,

            [GBA_DefinedPointer.DES_DrumWalkerGraphics] = 0x082C6C5C,
            [GBA_DefinedPointer.DES_InkGraphics] = 0x082D33D0,
            [GBA_DefinedPointer.DES_PinsGraphics] = 0x0832CBF4,

            [GBA_DefinedPointer.ExtFontImgBuffers] = 0x086DCEE8,
            [GBA_DefinedPointer.MultiplayerImgBuffers] = 0x086DCF98,

            [GBA_DefinedPointer.MusyxFile] = 0x086EFADC,
        };

        public static Dictionary<GBA_DefinedPointer, long> GBA_EU => new Dictionary<GBA_DefinedPointer, long>()
        {
            [GBA_DefinedPointer.LevelMaps] = 0x085485B4,
            [GBA_DefinedPointer.BackgroundVignette] = 0x086D4D60,
            [GBA_DefinedPointer.IntroVignette] = 0x080F7968,
            [GBA_DefinedPointer.SpritePalettes] = 0x08548F56,

            [GBA_DefinedPointer.EventGraphicsPointers] = 0x081A62E4,
            [GBA_DefinedPointer.EventDataPointers] = 0x081A6444,
            [GBA_DefinedPointer.EventGraphicsGroupCountTablePointers] = 0x081A65A4,
            [GBA_DefinedPointer.LevelEventGraphicsGroupCounts] = 0x081A6704,

            [GBA_DefinedPointer.WorldLevelOffsetTable] = 0x081539A4,

            [GBA_DefinedPointer.WorldInfo] = 0x0854A45C,
            [GBA_DefinedPointer.WorldMapVignetteImageData] = 0x08145208,
            [GBA_DefinedPointer.WorldMapVignetteBlockIndices] = 0x08151468,
            [GBA_DefinedPointer.WorldMapVignettePaletteIndices] = 0x081521E8,
            [GBA_DefinedPointer.WorldMapVignettePalettes] = 0x081528A8,

            [GBA_DefinedPointer.StringPointers] = 0x0854ACE0,

            [GBA_DefinedPointer.TypeZDC] = 0x0854A230,
            [GBA_DefinedPointer.ZdcData] = 0x08549BF0,
            [GBA_DefinedPointer.EventFlags] = 0x0854925C,

            [GBA_DefinedPointer.WorldVignetteIndices] = 0x08153980,

            [GBA_DefinedPointer.DES_Ray] = 0x0835F8E0,
            [GBA_DefinedPointer.DES_RayLittle] = 0x0835F904,
            [GBA_DefinedPointer.DES_Clock] = 0x082C8FF4,
            [GBA_DefinedPointer.DES_Div] = 0x082D1CC4,
            [GBA_DefinedPointer.DES_Map] = 0x0832A21C,

            [GBA_DefinedPointer.ETA_Ray] = 0x0832D160,
            //[R1_GBA_ROMPointer.ETA_Clock] = ,
            [GBA_DefinedPointer.ETA_Div] = 0x082CC7B0,
            [GBA_DefinedPointer.ETA_Map] = 0x082ED2E4,

            [GBA_DefinedPointer.DES_Alpha] = 0x082E7420,
            [GBA_DefinedPointer.DES_Alpha2] = 0x082E7440,

            [GBA_DefinedPointer.DES_DrumWalkerGraphics] = 0x082C6B88,
            [GBA_DefinedPointer.DES_InkGraphics] = 0x082D32FC,
            [GBA_DefinedPointer.DES_PinsGraphics] = 0x0832CB20,

            [GBA_DefinedPointer.ExtFontImgBuffers] = 0x086dce14,
            [GBA_DefinedPointer.MultiplayerImgBuffers] = 0x086dcec4,

            [GBA_DefinedPointer.MusyxFile] = 0x086EF6D4,
        };

        public static Dictionary<GBA_DefinedPointer, long> GBA_EUBeta
        {
            get
            {
                var d = GBA_EU;

                foreach (var k in d.Keys.ToList())
                    d[k] -= 0xC;

                return d;
            }
        }
    }
}