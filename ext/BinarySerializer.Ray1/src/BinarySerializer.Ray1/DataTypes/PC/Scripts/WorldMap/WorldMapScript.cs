namespace BinarySerializer.Ray1
{
    public class WorldMapScript : BinarySerializable
    {
        public byte[][] Alpha { get; set; }
        public WorldMap WorldMap { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            // Serialize alpha data (only on EDU)
            if (settings.EngineVersion is Ray1EngineVersion.PC_Edu or Ray1EngineVersion.PS1_Edu)
            {
                Alpha = s.InitializeArray(Alpha, 160);
                s.DoArray(Alpha, (x, _, name) => s.SerializeArray<byte>(x, 256, name: name), name: nameof(Alpha));
            }

            WorldMap = s.SerializeObject<WorldMap>(WorldMap, name: nameof(WorldMap));
        }
    }
}