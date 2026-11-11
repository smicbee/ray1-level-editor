namespace BinarySerializer.Ray1.PC
{
    public class WorldDefine : BinarySerializable
    {
        public string WorldName { get; set; }
        public string LevelFilePrefix { get; set; }
        public string SoundGroup { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            WorldName = s.SerializeString(WorldName, 13, name: nameof(WorldName));
            LevelFilePrefix = s.SerializeString(LevelFilePrefix, 4, name: nameof(LevelFilePrefix));
            SoundGroup = s.SerializeString(SoundGroup, 9, name: nameof(SoundGroup));
        }
    }
}