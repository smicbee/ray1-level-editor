namespace BinarySerializer.Ray1
{
    public enum World : byte
    {
        None = 0,

        Jungle = 1,
        Music = 2,
        Mountain = 3,
        Image = 4,
        Cave = 5,
        Cake = 6,

        // Reserved for the menu, used in the Jaguar version
        Menu = 7,
        
        // GBA only
        Multiplayer = 8
    }
}