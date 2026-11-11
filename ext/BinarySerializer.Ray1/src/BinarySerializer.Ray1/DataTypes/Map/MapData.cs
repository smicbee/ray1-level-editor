namespace BinarySerializer.Ray1
{
    /// <summary>
    /// A common map .MAP file
    /// </summary>
    public class MapData : BinarySerializable
    {
        public MapData() { }

        public MapData(int width, int height)
        {
            Width = (ushort)width;
            Height = (ushort)height;
            Blocks = new Block[width * height];

            for (int i = 0; i < Blocks.Length; i++)
                Blocks[i] = new Block();
        }

        /// <summary>
        /// The width of the map
        /// </summary>
        public ushort Width { get; set; }

        /// <summary>
        /// The height of the map
        /// </summary>
        public ushort Height { get; set; }

        /// <summary>
        /// The map blocks (tiles). Each block is 16x16 pixels.
        /// </summary>
        public Block[] Blocks { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
            // Serialize map size
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            // Serialize tiles
            Blocks = s.SerializeObjectArray<Block>(Blocks, Width * Height, name: nameof(Blocks));
        }
    }
}