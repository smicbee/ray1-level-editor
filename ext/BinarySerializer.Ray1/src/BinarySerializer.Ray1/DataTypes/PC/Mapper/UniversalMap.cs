namespace BinarySerializer.Ray1.PC
{
    public class UniversalMap : BinarySerializable
    {
        /// <summary>
        /// The width of the map, in tiles
        /// </summary>
        public ushort Width { get; set; }

        /// <summary>
        /// The height of the map, in tiles
        /// </summary>
        public ushort Height { get; set; }

        /// <summary>
        /// The tiles for the map
        /// </summary>
        public UniversalMapBlock[] Tiles { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize map size
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            // Serialize tiles
            Tiles = s.SerializeObjectArray<UniversalMapBlock>(Tiles, Width * Height, name: nameof(Tiles));
        }
    }
}