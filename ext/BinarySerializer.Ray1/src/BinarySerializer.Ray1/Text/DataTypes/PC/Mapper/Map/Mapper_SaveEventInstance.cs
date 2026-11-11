namespace BinarySerializer.Ray1
{
    // Same struct as JAG_Event
    public class Mapper_SaveEventInstance
    {
        public short IsValid { get; set; }
        public short OffsetX { get; set; }
        public short OffsetY { get; set; }
        public string EventDefinitionKey { get; set; }
        public short HitPoints { get; set; }
        public byte InitFlag { get; set; }
        public short LinkID { get; set; }
    }
}