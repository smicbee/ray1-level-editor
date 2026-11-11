namespace BinarySerializer.Ray1
{
    /// <summary>
    /// Event map data for the Jaguar version, as well as the event.mev files in Rayman Designer
    /// </summary>
    public class MapEvents : BinarySerializable
    {
        public bool HasEvents { get; set; } // Always 1?

        // Event map dimensions, always the map size divided by 4
        public ushort Width { get; set; }
        public ushort Height { get; set; }

        // Mapped to a 2D plane based on width and height
        public ushort[] EventIndexMap { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            HasEvents = s.Serialize<bool>(HasEvents, name: nameof(HasEvents));
            s.SerializePadding(3);

            // Serialize event map dimensions
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            EventIndexMap = s.SerializeArray<ushort>(EventIndexMap, Width * Height, name: nameof(EventIndexMap));
        }
    }
}