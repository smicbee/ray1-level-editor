using System.Linq;

namespace BinarySerializer.Ray1.GBA
{
    /// <summary>
    /// Vignette intro data for Rayman Advance (GBA)
    /// </summary>
    public class GBA_IntroVignette : BinarySerializable
    {
        #region Vignette Data

        public int Width => (256 / 8);

        public int Height => (160 / 8) * FrameCount;

        public Pointer ImageDataPointer { get; set; }

        public byte[] Bytes_04 { get; set; }
        
        public Pointer ImageValuesPointer { get; set; }

        public byte[] Bytes_0C { get; set; }

        public Pointer PalettesPointer { get; set; }

        public byte[] Bytes_14 { get; set; }

        public byte FrameCount { get; set; }
        
        public byte[] Bytes_19 { get; set; }

        #endregion

        #region Parsed from Pointers

        public byte[] ImageData { get; set; }

        public ushort[] ImageValues { get; set; }

        /// <summary>
        /// The 6 available palettes (16 colors each)
        /// </summary>
        public RGBA5551Color[] Palettes { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize data

            ImageDataPointer = s.SerializePointer(ImageDataPointer, name: nameof(ImageDataPointer));
            Bytes_04 = s.SerializeArray<byte>(Bytes_04, 4, name: nameof(Bytes_04));
            ImageValuesPointer = s.SerializePointer(ImageValuesPointer, name: nameof(ImageValuesPointer));
            Bytes_0C = s.SerializeArray<byte>(Bytes_0C, 4, name: nameof(Bytes_0C));
            PalettesPointer = s.SerializePointer(PalettesPointer, name: nameof(PalettesPointer));
            Bytes_14 = s.SerializeArray<byte>(Bytes_14, 4, name: nameof(Bytes_14));
            FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
            Bytes_19 = s.SerializeArray<byte>(Bytes_19, 3, name: nameof(Bytes_19));

            // Serialize data from pointers

            ImageValues = s.DoAt(ImageValuesPointer, () => s.SerializeArray<ushort>(default, Width * Height, name: nameof(ImageValues)));
            var imgDataLength = ImageValues.Select(x => BitHelpers.ExtractBits(x, 12, 0)).Max() + 1;
            ImageData = s.DoAt(ImageDataPointer, () => s.SerializeArray<byte>(ImageData, 0x20 * imgDataLength, name: nameof(ImageData)));
            Palettes = s.DoAt(PalettesPointer, () => s.SerializeObjectArray<RGBA5551Color>(Palettes, 16 * 16, name: nameof(Palettes)));
        }

        #endregion
    }
}