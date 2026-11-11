using System.Collections.Generic;

namespace BinarySerializer.Ray1.Jaguar
{
    public static class JAG_DefinedPointers
    {
        public static Dictionary<JAG_DefinedPointer, long> JAG => new Dictionary<JAG_DefinedPointer, long>()
        {
            [JAG_DefinedPointer.EventDefinitions] = 0x00906130,
            [JAG_DefinedPointer.FixSprites] = 0x009496C8,
            [JAG_DefinedPointer.WorldSprites] = 0x00949034,
            [JAG_DefinedPointer.MapData] = 0x00949054,
            [JAG_DefinedPointer.Music] = 0x009210F0,
            [JAG_DefinedPointer.WorldInfos] = 0x009492E0,
        };

        public static Dictionary<JAG_DefinedPointer, long> JAG_Demo => new Dictionary<JAG_DefinedPointer, long>()
        {
            [JAG_DefinedPointer.EventDefinitions] = 0x00918B40,
            [JAG_DefinedPointer.FixSprites] = 0x008028BA,
            [JAG_DefinedPointer.WorldSprites] = 0x00874F14,
            [JAG_DefinedPointer.MapData] = 0x00874F34,
            [JAG_DefinedPointer.Music] = 0x00846C80,
            [JAG_DefinedPointer.WorldInfos] = 0x0083BEFC,
        };
    }
}