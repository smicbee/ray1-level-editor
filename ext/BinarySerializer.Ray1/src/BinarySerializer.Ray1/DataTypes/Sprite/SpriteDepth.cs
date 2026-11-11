namespace BinarySerializer.Ray1
{
    /// <summary>
    /// This enum comes from the Jaguar Object structure. For Rayman 1
    /// only the 4-bit and 8-bit depths are used for sprites
    /// </summary>
    public enum SpriteDepth : ushort
    {
        BPP_1 = 0,
        BPP_2 = 1,
        BPP_4 = 2,
        BPP_8 = 3,
        BPP_16 = 4,
        BPP_24 = 5,
    }
}