namespace BinarySerializer.Ray1
{
    public class ActiveTable : BinarySerializable
    {
        public short[] ActiveObjects { get; set; }
        public short ActiveObjectsCount { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            var settings = s.GetRequiredSettings<Ray1Settings>();

            int count = settings.EngineBranch switch
            {
                Ray1EngineBranch.PS1 when settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol3 => 49,
                Ray1EngineBranch.GBA => 200,
                _ => 100
            };

            ActiveObjects = s.SerializeArray<short>(ActiveObjects, count, name: nameof(ActiveObjects));
            ActiveObjectsCount = s.Serialize<short>(ActiveObjectsCount, name: nameof(ActiveObjectsCount));
        }
    }
}