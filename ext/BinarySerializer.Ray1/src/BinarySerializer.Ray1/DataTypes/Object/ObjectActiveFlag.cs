namespace BinarySerializer.Ray1
{
    public enum ObjectActiveFlag : byte
    {
        Alive = 0,
        Dead = 1,
        Reinit = 2,
        Special = 4,
    }
}