namespace BinarySerializer.Ray1.PS1
{
    /// <summary>
    /// Fond (background) file data
    /// </summary>
    public class Fond : BinarySerializable
    {
        public byte XRelated { get; set; } // xmap/this
        public byte YRelated { get; set; } // ymap/this
        public short Width { get; set; }
        public short Height { get; set; }
        public byte Byte_06 { get; set; } // Read, but unused
        public byte Type { get; set; } // Determines how to scroll this background

        /// <summary>
        /// The image blocks, each one 64 pixels wide
        /// </summary>
        public ImageBlock[] ImageBlocks { get; set; }

        /// <summary>
        /// Gets the block width based on engine version
        /// </summary>
        /// <param name="engineVersion">The engine version</param>
        /// <returns>The block width</returns>
        public int GetBlockWidth(Ray1EngineVersion engineVersion) => engineVersion == Ray1EngineVersion.PS1_JPDemoVol3 ? 32 : 64;

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            // Serialize header values
            XRelated = s.Serialize<byte>(XRelated, name: nameof(XRelated));
            YRelated = s.Serialize<byte>(YRelated, name: nameof(YRelated));
            Width = s.Serialize<short>(Width, name: nameof(Width));
            Height = s.Serialize<short>(Height, name: nameof(Height));
            Byte_06 = s.Serialize<byte>(Byte_06, name: nameof(Byte_06));
            Type = s.Serialize<byte>(Type, name: nameof(Type));

            int blockWidth = GetBlockWidth(settings.EngineVersion);
            
            int length;

            if (Type == 0xC)
                length = 0; // TODO: The game sets this to 2, but that doesn't work...
            else
                length = Width / blockWidth;

            // Serialize blocks
            ImageBlocks = s.SerializeObjectArray<ImageBlock>(ImageBlocks, length, x =>
            {
                x.Pre_Width = blockWidth;
                x.Pre_Height = Height;
            }, name: nameof(ImageBlocks));
        }

        public class ImageBlock : BinarySerializable
        {
            public int Pre_Width { get; set; }
            public int Pre_Height { get; set; }
            
            public RGBA5551Color[] ImageData { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                ImageData = s.SerializeObjectArray<RGBA5551Color>(ImageData, Pre_Width * Pre_Height, name: nameof(ImageData));
            }
        }
    }
}