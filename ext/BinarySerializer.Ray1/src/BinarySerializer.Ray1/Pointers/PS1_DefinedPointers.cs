using System.Collections.Generic;

namespace BinarySerializer.Ray1
{
    public static class PS1_DefinedPointers
    {
        public static Dictionary<PS1_DefinedPointer, long> PS1_EU => new Dictionary<PS1_DefinedPointer, long>()
        {
            [PS1_DefinedPointer.TypeZDC] = 0x801C1F54,
            [PS1_DefinedPointer.ZDCData] = 0x801C0F54,
            [PS1_DefinedPointer.ObjTypeFlags] = 0x801C0754,
            [PS1_DefinedPointer.WorldInfo] = 0x801C281C,
            [PS1_DefinedPointer.PS1_LevelBackgroundIndexTable] = 0x801C35C8,
        };

        public static Dictionary<PS1_DefinedPointer, long> PS1_EUDemo => new Dictionary<PS1_DefinedPointer, long>()
        {
            [PS1_DefinedPointer.TypeZDC] = 0x801C2198,
            [PS1_DefinedPointer.ZDCData] = 0x801C1198,
            [PS1_DefinedPointer.ObjTypeFlags] = 0x801C0998,
            // [PS1_DefinedPointer.WorldInfo] = , // TODO: Find offset
            [PS1_DefinedPointer.PS1_LevelBackgroundIndexTable] = 0x801C380C,
        };

        public static Dictionary<PS1_DefinedPointer, long> PS1_JP => new Dictionary<PS1_DefinedPointer, long>()
        {
            [PS1_DefinedPointer.TypeZDC] = 0x801BFB08,
            [PS1_DefinedPointer.ZDCData] = 0x801BEB08,
            [PS1_DefinedPointer.ObjTypeFlags] = 0x801BE308,
            [PS1_DefinedPointer.WorldInfo] = 0x801C03D0,
            [PS1_DefinedPointer.PS1_LevelBackgroundIndexTable] = 0x801C1358,
        };

        public static Dictionary<PS1_DefinedPointer, long> PS1_US => new Dictionary<PS1_DefinedPointer, long>()
        {
            [PS1_DefinedPointer.TypeZDC] = 0x801C2A94,
            [PS1_DefinedPointer.ZDCData] = 0x801C1A94,
            [PS1_DefinedPointer.ObjTypeFlags] = 0x801C1294,
            [PS1_DefinedPointer.WorldInfo] = 0x801C335C,
            [PS1_DefinedPointer.PS1_LevelBackgroundIndexTable] = 0x801C43A4,
        };

        public static Dictionary<PS1_DefinedPointer, long> PS1_USDemo => new Dictionary<PS1_DefinedPointer, long>()
        {
            [PS1_DefinedPointer.TypeZDC] = 0x801C05F8,
            [PS1_DefinedPointer.ZDCData] = 0x801BF5F8,
            [PS1_DefinedPointer.ObjTypeFlags] = 0x801BEDF8,
            [PS1_DefinedPointer.WorldInfo] = 0x801C0EC0,
            [PS1_DefinedPointer.PS1_LevelBackgroundIndexTable] = 0x801C1F08,
        };

        public static Dictionary<PS1_DefinedPointer, long> SAT_EU => new Dictionary<PS1_DefinedPointer, long>()
        {
            [PS1_DefinedPointer.TypeZDC] = 0x06010000 + 0x7EB22,
            [PS1_DefinedPointer.ZDCData] = 0x06010000 + 0x7DB22,
            [PS1_DefinedPointer.ObjTypeFlags] = 0x06010000 + 0x7D320,
            [PS1_DefinedPointer.WorldInfo] = 0x06010000 + 0x7F3F0,
            [PS1_DefinedPointer.SAT_Palettes] = 0x06010000 + 0x78D14,
            [PS1_DefinedPointer.SAT_FndFileTable] = 0x06010000 + 0x8142C,
            [PS1_DefinedPointer.SAT_FndSPFileTable] = 0x06010000 + 0x81823,
            [PS1_DefinedPointer.SAT_FndIndexTable] = 0x06010000 + 0x8175B,
        };

        public static Dictionary<PS1_DefinedPointer, long> SAT_JP => new Dictionary<PS1_DefinedPointer, long>()
        {
            [PS1_DefinedPointer.TypeZDC] = 0x06010000 + 0x7EFD2,
            [PS1_DefinedPointer.ZDCData] = 0x06010000 + 0x7DFD2,
            [PS1_DefinedPointer.ObjTypeFlags] = 0x06010000 + 0x7D7D0,
            [PS1_DefinedPointer.WorldInfo] = 0x06010000 + 0x7F8A0,
            [PS1_DefinedPointer.SAT_Palettes] = 0x06010000 + 0x791C4,
            [PS1_DefinedPointer.SAT_FndFileTable] = 0x06010000 + 0x818E8,
            [PS1_DefinedPointer.SAT_FndSPFileTable] = 0x06010000 + 0x81CDF,
            [PS1_DefinedPointer.SAT_FndIndexTable] = 0x06010000 + 0x81C17,
        };

        public static Dictionary<PS1_DefinedPointer, long> SAT_Proto => new Dictionary<PS1_DefinedPointer, long>()
        {
            [PS1_DefinedPointer.TypeZDC] = 0x06010000 + 0x832B2,
            [PS1_DefinedPointer.ZDCData] = 0x06010000 + 0x822B2,
            [PS1_DefinedPointer.ObjTypeFlags] = 0x06010000 + 0x81AB0,
            [PS1_DefinedPointer.WorldInfo] = 0x06010000 + 0x83B80,
            [PS1_DefinedPointer.SAT_Palettes] = 0x06010000 + 0x87754,
            [PS1_DefinedPointer.SAT_FndFileTable] = 0x06010000 + 0x85BA0,
            [PS1_DefinedPointer.SAT_FndSPFileTable] = 0x06010000 + 0x85F97,
            [PS1_DefinedPointer.SAT_FndIndexTable] = 0x06010000 + 0x85ECF,
        };

        public static Dictionary<PS1_DefinedPointer, long> SAT_US => new Dictionary<PS1_DefinedPointer, long>()
        {
            [PS1_DefinedPointer.TypeZDC] = 0x06010000 + 0x7F032,
            [PS1_DefinedPointer.ZDCData] = 0x06010000 + 0x7e032,
            [PS1_DefinedPointer.ObjTypeFlags] = 0x06010000 + 0x7D830,
            [PS1_DefinedPointer.WorldInfo] = 0x06010000 + 0x7F900,
            [PS1_DefinedPointer.SAT_Palettes] = 0x06010000 + 0x79224,
            [PS1_DefinedPointer.SAT_FndFileTable] = 0x06010000 + 0x81948,
            [PS1_DefinedPointer.SAT_FndSPFileTable] = 0x06010000 + 0x81D3F,
            [PS1_DefinedPointer.SAT_FndIndexTable] = 0x06010000 + 0x81C77,
        };

        public static Dictionary<PS1_DefinedPointer, long> SAT_USDemo => new Dictionary<PS1_DefinedPointer, long>()
        {
            [PS1_DefinedPointer.TypeZDC] = 0x06010000 + 0x7EC56,
            [PS1_DefinedPointer.ZDCData] = 0x06010000 + 0x7DC56,
            [PS1_DefinedPointer.ObjTypeFlags] = 0x06010000 + 0x7D454,
            [PS1_DefinedPointer.WorldInfo] = 0x06010000 + 0x7F524,
            [PS1_DefinedPointer.SAT_Palettes] = 0x06010000 + 0x78E48,
            [PS1_DefinedPointer.SAT_FndFileTable] = 0x06010000 + 0x8156C,
            [PS1_DefinedPointer.SAT_FndSPFileTable] = 0x06010000 + 0x81963,
            [PS1_DefinedPointer.SAT_FndIndexTable] = 0x06010000 + 0x8189B,
        };

        public static Dictionary<PS1_DefinedPointer, long> SAT_JPDemo => new Dictionary<PS1_DefinedPointer, long>()
        {
            [PS1_DefinedPointer.TypeZDC] = 0x06010000 + 0x836F2,
            [PS1_DefinedPointer.ZDCData] = 0x06010000 + 0x826F2,
            [PS1_DefinedPointer.ObjTypeFlags] = 0x06010000 + 0x81EF0,
            [PS1_DefinedPointer.WorldInfo] = 0x06010000 + 0x83FC0,
            [PS1_DefinedPointer.SAT_Palettes] = 0x06010000 + 0x87B98,
            [PS1_DefinedPointer.SAT_FndFileTable] = 0x06010000 + 0x85FE0,
            [PS1_DefinedPointer.SAT_FndSPFileTable] = 0x06010000 + 0x863D7,
            [PS1_DefinedPointer.SAT_FndIndexTable] = 0x06010000 + 0x8630F,
        };
    }
}