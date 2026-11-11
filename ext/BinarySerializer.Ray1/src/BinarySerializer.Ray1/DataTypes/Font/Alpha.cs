namespace BinarySerializer.Ray1
{
    /// <summary>
    /// Group of font sprites
    /// </summary>
    public class Alpha : BinarySerializable
    {
        public Pointer SpritesPointer { get; set; }
        public Pointer ImageBufferPointer { get; set; }
        public byte SpritesCount { get; set; }

        public Sprite[] Sprites { get; set; }
        public byte[] ImageBuffer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            SpritesPointer = s.SerializePointer(SpritesPointer, name: nameof(SpritesPointer));
            ImageBufferPointer = s.SerializePointer(ImageBufferPointer, allowInvalid: true, name: nameof(ImageBufferPointer));
            SpritesCount = s.Serialize<byte>(SpritesCount, name: nameof(SpritesCount));
            s.SerializePadding(3);

            s.DoAt(SpritesPointer, () => 
                Sprites = s.SerializeObjectArray<Sprite>(Sprites, SpritesCount, name: nameof(Sprites)));

            if (settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol3)
            {
                s.DoAt(ImageBufferPointer, () => 
                    ImageBuffer = s.SerializeArray<byte>(ImageBuffer, InternalHelpers.GetImageBufferLength(Sprites, settings), name: nameof(ImageBuffer)));
            }
        }
    }
}