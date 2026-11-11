using System;

namespace BinarySerializer.Ray1
{
    [Flags]
    public enum SpriteFlags : byte
    {
        None = 0,
        Flag_0 = 1 << 0,
        SemiTransparent = 1 << 1, // Only used in the Rayman 2 2D prototype
        FlipX = 1 << 2, // Only used in the PC versions
        Flag_3 = 1 << 3,
        Flag_4 = 1 << 4,
        Flag_5 = 1 << 5,
        Flag_6 = 1 << 6,
        Flag_7 = 1 << 7,
    }
}