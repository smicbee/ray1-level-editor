namespace BinarySerializer.Ray1
{
    public class LevelLinkEntry : BinarySerializable
    {
        // Unused feature where each level has different variants, randomly picked when starting a new game
        public byte?[] LevelVariants { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LevelVariants = s.SerializeNullableArray<byte>(LevelVariants, 2, name: nameof(LevelVariants));
        }
    }
}