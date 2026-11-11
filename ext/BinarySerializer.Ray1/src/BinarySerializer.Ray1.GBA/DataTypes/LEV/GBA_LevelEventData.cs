namespace BinarySerializer.Ray1.GBA
{
    /// <summary>
    /// Event data for a level
    /// </summary>
    public class GBA_LevelEventData
    {
        public Pointer EventGraphicsPointer { get; set; }
        public Pointer EventDataPointer { get; set; }
        public Pointer GraphicsGroupCountTablePointer { get; set; }
        public uint GraphicsGroupCount { get; set; }

        public byte[] GraphicsGroupCountTable { get; set; }

        public Pointer[] GraphicDataPointers { get; set; }
        public GBA_EventGraphicsData[] GraphicData { get; set; }
        
        public Pointer[] EventDataPointers { get; set; }
        public GBA_EventData[][] EventData { get; set; }

        public void SerializeData(SerializerObject s, Pointer eventGraphicsPointers, Pointer eventDataPointers, Pointer eventGraphicsGroupCountTablePointers, Pointer levelEventGraphicsGroupCounts, int levelIndex)
        {
            // Serialize data
            EventGraphicsPointer = s.DoAt(eventGraphicsPointers + (uint)(4 * levelIndex), 
                () => s.SerializePointer(EventGraphicsPointer, name: nameof(EventGraphicsPointer)));
            EventDataPointer = s.DoAt(eventDataPointers + (uint)(4 * levelIndex), 
                () => s.SerializePointer(EventDataPointer,  name: nameof(EventDataPointer)));
            GraphicsGroupCountTablePointer = s.DoAt(eventGraphicsGroupCountTablePointers + (uint)(4 * levelIndex), 
                () => s.SerializePointer(GraphicsGroupCountTablePointer, name: nameof(GraphicsGroupCountTablePointer)));
            GraphicsGroupCount = s.DoAt(levelEventGraphicsGroupCounts + (uint)(4 * levelIndex), 
                () => s.Serialize<uint>(GraphicsGroupCount, name: nameof(GraphicsGroupCount)));

            // Parse data from pointers
            GraphicsGroupCountTable = s.DoAt(GraphicsGroupCountTablePointer, () => s.SerializeArray<byte>(GraphicsGroupCountTable, GraphicsGroupCount, name: nameof(GraphicsGroupCountTable)));
            GraphicDataPointers = s.DoAt(EventGraphicsPointer, () => s.SerializePointerArray(GraphicDataPointers, GraphicsGroupCount, name: nameof(GraphicDataPointers)));

            GraphicData ??= new GBA_EventGraphicsData[GraphicsGroupCount];

            for (int i = 0; i < GraphicData.Length; i++)
                GraphicData[i] = s.DoAt(GraphicDataPointers[i], () => s.SerializeObject<GBA_EventGraphicsData>(GraphicData[i], name: $"{nameof(GraphicData)}[{i}]"));

            EventDataPointers = s.DoAt(EventDataPointer, () => s.SerializePointerArray(EventDataPointers, GraphicsGroupCount, name: nameof(EventDataPointers)));

            EventData ??= new GBA_EventData[GraphicsGroupCount][];

            for (int i = 0; i < EventData.Length; i++)
            {
                if (EventDataPointers[i] != null)
                    EventData[i] = s.DoAt(EventDataPointers[i], () => s.SerializeObjectArray<GBA_EventData>(EventData[i], GraphicsGroupCountTable[i], name: $"{nameof(EventData)}[{i}]"));
                else
                    EventData[i] = new GBA_EventData[0];
            }
        }
    }
}