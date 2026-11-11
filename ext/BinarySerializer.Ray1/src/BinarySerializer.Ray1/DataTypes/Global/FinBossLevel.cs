using System;

namespace BinarySerializer.Ray1
{
    [Flags]
    public enum FinBossLevel : ushort
    {
        None = 0,

        Bzzit = 1 << 0,
        Moskito = 1 << 1,
        MrSax = 1 << 2,
        MrStone = 1 << 3,
        VikingMama = 1 << 4,
        SpaceMama = 1 << 5,
        MrSkops = 1 << 6,
        MrDark = 1 << 7,
        CrazyDrummer = 1 << 8, // Unused, indicates if Bongo Hills 6 is not to be replayed
        HelpedJoe1 = 1 << 9, // This indicates if Eat at Joe's has been completed
        HelpedJoe2 = 1 << 10, // joe_exp_probleme, indicates if the light switch has been activated
        HelpedMusician = 1 << 11,
    }
}