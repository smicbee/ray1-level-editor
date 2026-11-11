namespace BinarySerializer.Ray1
{
    /// <summary>
    /// The block (tile) type
    /// </summary>
    public enum BlockType : byte
    {
        None = 0,
        ChangeDirection = 1,
        Solid_Right_45 = 2,
        Solid_Left_45 = 3,
        Solid_Right1_30 = 4,
        Solid_Right2_30 = 5,
        Solid_Left1_30 = 6,
        Solid_Left2_30 = 7,
        Damage = 8,
        Bounce = 9,
        Water = 10,
        Exit = 11,
        Climb = 12,
        WaterNoSplash = 13,
        Passthrough = 14,
        Solid = 15,
        Seed = 16,

        Slippery_Right_45 = 18,
        Slippery_Left_45 = 19,
        Slippery_Right1_30 = 20,
        Slippery_Right2_30 = 21,
        Slippery_Left1_30 = 22,
        Slippery_Left2_30 = 23,
        Spikes = 24,
        Cliff = 25,

        Slippery = 30
    }
}