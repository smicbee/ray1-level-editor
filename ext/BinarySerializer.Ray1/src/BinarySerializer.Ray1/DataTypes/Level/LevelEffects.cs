using System;

namespace BinarySerializer.Ray1
{
    [Flags]
    public enum LevelEffects : ushort
    {
        // Invalid combos:
        // Storm + firefly
        // Rain + snow
        // Wind without rain
        // Hot effect with differential scroll

        None = 0,

        Effect_0 = 1 << 0, // Rendering related?
        Clipping = 1 << 1,
        LockScrollX = 1 << 2,
        LockScrollY = 1 << 3,
        Cutscene = 1 << 4,
        Storm = 1 << 5,
        Rain = 1 << 6,
        Snow = 1 << 7,

        Wind = 1 << 8,
        Firefly = 1 << 9,
        HotEffect = 1 << 10,
        Effect_11 = 1 << 11, // Timed level related
        HideHud = 1 << 12,
        Effect_13 = 1 << 13, // Only used in EDU

        // Appear unused
        Effect_14 = 1 << 14,
        Effect_15 = 1 << 15,
    }
}