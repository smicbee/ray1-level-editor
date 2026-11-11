namespace BinarySerializer.Ray1
{
    public enum RayMode : short
    {
        PlaceRay = -1, // All other values are PlaceRay. The game flips the sign of the value to set this so the original value can be saved. Because of this -1 will be the most commonly used value.
        Rayman = 1,
        RayOnMS = 2,
        MortDeRayman = 3,
        MortDeRaymanOnMS = 4,
        RayCasseBrique = 5, // PC only
    }
}