namespace BinarySerializer.Ray1.PC
{
    /// <summary>
    /// Event localization data for Rayman Mapper (PC)
    /// </summary>
    public class EventNames : BinarySerializable
    {
        public GameVersion GameVersion { get; set; }

        /// <summary>
        /// The amount of localization items
        /// </summary>
        public uint LocCount { get; set; }

        /// <summary>
        /// The size of each string in each loc item
        /// </summary>
        public ushort[][] StringLengths { get; set; }

        /// <summary>
        /// The localization items
        /// </summary>
        public EventName[] LocItems { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            // Serialize header
            if (settings.IsVersioned)
                GameVersion = s.SerializeObject<GameVersion>(GameVersion, name: nameof(GameVersion));

            // Serialize the count
            LocCount = s.Serialize<uint>(LocCount, name: nameof(LocCount));

            // Serialize the string lengths. We however don't use these values as the strings are all null-terminated.
            StringLengths ??= new ushort[LocCount][];

            for (int i = 0; i < StringLengths.Length; i++)
                StringLengths[i] = s.SerializeArray<ushort>(StringLengths[i], 3, name: $"{nameof(StringLengths)}[{i}]");

            // Serialize the localization items
            LocItems = s.SerializeObjectArray<EventName>(LocItems, LocCount, name: nameof(LocItems));
        }
    }
}