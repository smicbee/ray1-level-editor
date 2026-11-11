namespace BinarySerializer.Ray1.Jaguar
{
    public class JAG_WorldInfo : BinarySerializable
    {
        public short World { get; set; } // Negative for save points
        public short Level { get; set; } // Negative for save points
        public short XPosition { get; set; }
        public short YPosition { get; set; }
        public int SaveValuePointer { get; set; } // Pointer to save byte in memory
        public ushort WorldIndex { get; set; } // The index of the entry in this world
        public short IconXPosition { get; set; }
        public short IconYPosition { get; set; }
        public Pointer<Jag_ColoredString> WorldNamePointer { get; set; }
        public Pointer<Jag_ColoredString> LevelNamePointer { get; set; }
        public Pointer UnusedPointer { get; set; } // Always null
        public JAG_WorldInfoLink[] Links { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            var settings = s.GetRequiredSettings<Ray1Settings>();

            World = s.Serialize<short>(World, name: nameof(World));
            Level = s.Serialize<short>(Level, name: nameof(Level));
            XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
            SaveValuePointer = s.Serialize<int>(SaveValuePointer, name: nameof(SaveValuePointer));
            WorldIndex = s.Serialize<ushort>(WorldIndex, name: nameof(WorldIndex));

            if (settings.EngineVersion == Ray1EngineVersion.Jaguar)
            {
                IconXPosition = s.Serialize<short>(IconXPosition, name: nameof(IconXPosition));
                IconYPosition = s.Serialize<short>(IconYPosition, name: nameof(IconYPosition));
                WorldNamePointer = s.SerializePointer<Jag_ColoredString>(WorldNamePointer, name: nameof(WorldNamePointer))?.ResolveObject(s);
                LevelNamePointer = s.SerializePointer<Jag_ColoredString>(LevelNamePointer, name: nameof(LevelNamePointer))?.ResolveObject(s);
                UnusedPointer = s.SerializePointer(UnusedPointer, name: nameof(UnusedPointer));
            }

            Links = s.SerializeObjectArrayUntil<JAG_WorldInfoLink>(Links,
                conditionCheckFunc: x => x.Directions == JAG_WorldInfoLink.Direction.None, 
                getLastObjFunc: () => new JAG_WorldInfoLink(), 
                name: nameof(Links));
        }
    }
}