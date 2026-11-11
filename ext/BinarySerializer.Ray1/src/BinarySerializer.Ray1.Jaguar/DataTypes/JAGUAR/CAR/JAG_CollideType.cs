using System;

namespace BinarySerializer.Ray1.Jaguar
{
    // Note: These are for the prototype. The final game has more types defined.

    [Flags]
    public enum JAG_CollideType : ushort
    {
        nib = 0, // None

        ami = 1 << 0,
        amit = 1 << 1,

        enn = 1 << 2, // Detect fist?
        ennt = 1 << 3, // Detect Rayman?

        neut = 1 << 4,

        Col_5 = 1 << 5,
        Col_6 = 1 << 6,
        Col_7 = 1 << 7,
        Col_8 = 1 << 8,
        Col_9 = 1 << 9,
        Col_10 = 1 << 10,
        Col_11 = 1 << 11,
        Col_12 = 1 << 12,
        Col_13 = 1 << 13,

        plat = 1 << 14, // Platform
        spr = 1 << 15, // Relative to sprite?
    }
}