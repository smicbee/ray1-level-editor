namespace BinarySerializer.Ray1.PS1
{
    /// <summary>
    /// A PS1 background .BGM file
    /// </summary>
    public class BackgroundData : BinarySerializable
    {
        public BackgroundSpriteDefine[] SpriteDefines { get; set; }
        public Sprite[] Sprites { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();
            int count = settings.EngineVersion == Ray1EngineVersion.PS1_JP ? 20 : 16;

            // Serialize the background layer information
            SpriteDefines = s.SerializeObjectArray<BackgroundSpriteDefine>(SpriteDefines, count, name: nameof(SpriteDefines));
            Sprites = s.SerializeObjectArray<Sprite>(Sprites, count, name: nameof(Sprites));
        }
    }
}