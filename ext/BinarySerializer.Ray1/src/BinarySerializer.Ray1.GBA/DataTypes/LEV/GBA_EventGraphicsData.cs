namespace BinarySerializer.Ray1.GBA
{
    /// <summary>
    /// Event graphic data
    /// </summary>
    public class GBA_EventGraphicsData : BinarySerializable 
    {
        public Pointer ImageBufferPointer { get; set; }

        // Gets set to obj offset 30 in memory, which gets overwritten by the position
        public uint ImageBufferSize { get; set; }

        public Pointer SpritesPointer { get; set; }
        public uint SpritesLength { get; set; }

        public Pointer ETAsPointer { get; set; }
        public uint ETAsCount { get; set; }

        public Pointer AnimationsPointer { get; set; }
        public uint AnimationsCount { get; set; }

        // Parsed from pointers
        public byte[] ImageBuffer { get; set; }
        public Sprite[] Sprites { get; set; }
        public Animation[] Animations { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            // Serialize data
            ImageBufferPointer = s.SerializePointer(ImageBufferPointer, name: nameof(ImageBufferPointer));
            ImageBufferSize = s.Serialize<uint>(ImageBufferSize, name: nameof(ImageBufferSize));
            SpritesPointer = s.SerializePointer(SpritesPointer, name: nameof(SpritesPointer));
            SpritesLength = s.Serialize<uint>(SpritesLength, name: nameof(SpritesLength));
            ETAsPointer = s.SerializePointer(ETAsPointer, name: nameof(ETAsPointer));
            ETAsCount = s.Serialize<uint>(ETAsCount, name: nameof(ETAsCount));
            AnimationsPointer = s.SerializePointer(AnimationsPointer, name: nameof(AnimationsPointer));
            AnimationsCount = s.Serialize<uint>(AnimationsCount, name: nameof(AnimationsCount));

            // Serialize data from pointers
            ImageBuffer = s.DoAt(ImageBufferPointer, () => s.SerializeArray<byte>(ImageBuffer, ImageBufferSize, name: nameof(ImageBuffer)));
            Animations = s.DoAt(AnimationsPointer, () => s.SerializeObjectArray<Animation>(Animations, AnimationsCount, name: nameof(Animations)));
            Sprites = s.DoAt(SpritesPointer, () => s.SerializeObjectArray<Sprite>(Sprites, SpritesLength / 12, name: nameof(Sprites)));
        }
    }
}