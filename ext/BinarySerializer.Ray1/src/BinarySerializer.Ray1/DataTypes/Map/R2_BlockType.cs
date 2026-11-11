namespace BinarySerializer.Ray1
{
    public enum R2_BlockType : byte
    {
        None = 0,

        ChangeDirection_Left = 1,
        ChangeDirection_Right = 2,
        ChangeDirection_Up = 3,
        ChangeDirection_Down = 4,

        ChangeDirection_UpLeft = 5,
        ChangeDirection_UpRight = 6,
        ChangeDirection_DownLeft = 7,
        ChangeDirection_DownRight = 8,

        Unknown_11 = 11,
        Unknown_14 = 14,
        Spikes = 15,
        Cliff = 18,
        Water = 19,
        Damage = 20,
        Bounce = 21,
        Solid = 22,
        Passthrough = 23,
        Slippery = 24,

        Solid_Right1_30 = 25,
        Solid_Right2_30 = 26,
        Solid_Right_45 = 27,

        Solid_Left1_30 = 28,
        Solid_Left2_30 = 29,
        Solid_Left_45 = 30,

        Slippery_Right1_30 = 31,
        Slippery_Right2_30 = 32,
        Slippery_Right_45 = 33,

        Slippery_Left1_30 = 34,
        Slippery_Left2_30 = 35,
        Slippery_Left_45 = 36,
        
        MoveLeft = 37,
        MoveRight = 38,

        Hook = 42, // Can place hook here by punching

        Climb = 44,

        ChangeDirection_Reverse = 47,

        ChangeDirection_Counter = 49, // Appears to change the direction of a moving platform after a certain amount of hits

        Canon_ValidTarget = 50,
        Canon_InvalidTarget = 51,
    }
}