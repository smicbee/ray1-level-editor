namespace BinarySerializer.Ray1.PC
{
    public class MemorySizes : BinarySerializable
    {
        public uint Tmp { get; set; }
        public uint Fix { get; set; }
        public uint World { get; set; }
        public uint Level { get; set; }
        public uint Sprite { get; set; }
        public uint SamplesTable { get; set; }
        public uint Edit { get; set; }
        public uint SaveEvent { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            var settings = s.GetRequiredSettings<Ray1Settings>();

            Tmp = s.Serialize<uint>(Tmp, name: nameof(Tmp));
            s.Log("{0}: {1} bytes", nameof(Tmp), Tmp << 10);
            
            Fix = s.Serialize<uint>(Fix, name: nameof(Fix));
            s.Log("{0}: {1} bytes", nameof(Fix), Fix << 10);
            
            World = s.Serialize<uint>(World, name: nameof(World));
            s.Log("{0}: {1} bytes", nameof(World), World << 10);
            
            Level = s.Serialize<uint>(Level, name: nameof(Level));
            s.Log("{0}: {1} bytes", nameof(Level), Level << 10);
            
            Sprite = s.Serialize<uint>(Sprite, name: nameof(Sprite));
            s.Log("{0}: {1} bytes", nameof(Sprite), Sprite << 10);
            
            SamplesTable = s.Serialize<uint>(SamplesTable, name: nameof(SamplesTable));
            s.Log("{0}: {1} bytes", nameof(SamplesTable), SamplesTable << 10);

            if (settings.EngineVersion == Ray1EngineVersion.PC_Kit || settings.EngineVersion == Ray1EngineVersion.PC_Fan)
            {
                Edit = s.Serialize<uint>(Edit, name: nameof(Edit));
                s.Log("{0}: {1} bytes", nameof(Edit), Edit << 10);
                
                SaveEvent = s.Serialize<uint>(SaveEvent, name: nameof(SaveEvent));
                s.Log("{0}: {1} bytes", nameof(SaveEvent), SaveEvent << 10);
            }
        }
    }
}