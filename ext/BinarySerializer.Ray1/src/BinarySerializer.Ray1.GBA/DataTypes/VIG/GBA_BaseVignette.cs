using System.Linq;
using BinarySerializer.Nintendo.GBA;

namespace BinarySerializer.Ray1.GBA
{
    /// <summary>
    /// Base vignette data for Rayman Advance (GBA)
    /// </summary>
    public abstract class GBA_BaseVignette : BinarySerializable
    {
        #region Properties

        protected abstract int PaletteCount { get; }

        #endregion

        #region Vignette Data

        public Pointer ImageDataPointer { get; set; }

        public Pointer BlockIndicesPointer { get; set; }

        public Pointer PaletteIndicesPointer { get; set; }

        public Pointer PalettesPointer { get; set; }

        // Note: These sizes are based on the number of blocks, so the actual size is Width*8 x Heightx8
        public ushort Width { get; set; }
        public ushort Height { get; set; }

        #endregion

        #region Parsed from Pointers

        /// <summary>
        /// The image data
        /// </summary>
        public byte[] ImageData { get; set; }

        /// <summary>
        /// The image block indices
        /// </summary>
        public ushort[] BlockIndices { get; set; }

        /// <summary>
        /// The palette indices
        /// </summary>
        public byte[] PaletteIndices { get; set; }

        /// <summary>
        /// The 6 available palettes (16 colors each)
        /// </summary>
        public RGBA5551Color[] Palettes { get; set; }

        #endregion

        #region Public Methods

        public void SerializeVignette(SerializerObject s, bool isImgDataCompressed)
        {
            var settings = s.GetRequiredSettings<Ray1Settings>();

            // Serialize data from pointers

            BlockIndices = s.DoAt(BlockIndicesPointer, () => s.SerializeArray<ushort>(BlockIndices, Width * Height, name: nameof(BlockIndices)));

            s.DoAt(ImageDataPointer, () => 
            {
                int tilesCount = BlockIndices.Max() + 1;

                if (settings.EngineVersion == Ray1EngineVersion.DSi)
                {
                    if (isImgDataCompressed)
                        s.DoEncoded(new LZSSEncoder(), () => ImageData = s.SerializeArray<byte>(ImageData, 0x40 * tilesCount, name: nameof(ImageData)));
                    else
                        ImageData = s.SerializeArray<byte>(ImageData, 0x40 * tilesCount, name: nameof(ImageData));
                } 
                else 
                {
                    ImageData = s.SerializeArray<byte>(ImageData, 0x20 * tilesCount, name: nameof(ImageData));
                }
            });

            PaletteIndices = s.DoAt(PaletteIndicesPointer, () => s.SerializeArray<byte>(PaletteIndices, Width * Height, name: nameof(PaletteIndices)));

            s.DoAt(PalettesPointer, () => 
            {
                Palettes = s.SerializeObjectArray<RGBA5551Color>(Palettes,
                    (settings.EngineVersion == Ray1EngineVersion.DSi) ? 256 : (PaletteCount * 16),
                    name: nameof(Palettes));
            });
        }

        #endregion
    }
}