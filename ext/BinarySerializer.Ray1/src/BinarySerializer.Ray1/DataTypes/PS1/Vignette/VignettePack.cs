namespace BinarySerializer.Ray1.PS1
{
    /// <summary>
    /// An XXX vignettes pack
    /// </summary>
    public class VignettePack : Pack
    {
        public ObjectArray<RGBA5551Color>[] ImageBlocks { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            base.SerializeImpl(s);

            // Go to the end of the pack
            ImageBlocks ??= new ObjectArray<RGBA5551Color>[FilesCount];

            for (int i = 0; i < FilesCount; i++)
                SerializeFile(s, i, length => ImageBlocks[i] = s.SerializeObject<ObjectArray<RGBA5551Color>>(ImageBlocks[i], x => x.Pre_Length = length / 2, name: $"{nameof(ImageBlocks)} [{i}]"));

            // Go to the end of the pack
            s.Goto(Offset + PackSize);
        }
    }
}