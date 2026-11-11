using System.Linq;
using BinarySerializer.Nintendo.GBA;

namespace BinarySerializer.Ray1.GBA
{
    /// <summary>
    /// Map data for a level
    /// </summary>
    public class GBA_LevelMapData : BinarySerializable
    {
        #region Level Data

        // Always 0
        public uint DSi_Uint_00 { get; set; }

        /// <summary>
        /// Pointer to the tiles
        /// </summary>
        public Pointer TileDataPointer { get; set; }

        // Always 0
        public uint DSi_Uint_08 { get; set; }

        /// <summary>
        /// Pointer to the compressed map data. Gets copied to 0x02002230 during runtime.
        /// </summary>
        public Pointer MapDataPointer { get; set; }

        /// <summary>
        /// Pointer to the compressed tile palette index table.
        /// </summary>
        public Pointer TilePaletteIndicesPointer { get; set; }

        // Always 0
        public uint DSi_Uint_10 { get; set; }

        /// <summary>
        /// Pointer to the tile header data (2 bytes per tile)
        /// </summary>
        public Pointer TileBlockIndicesPointer { get; set; }

        /// <summary>
        /// Pointer to the tile palettes
        /// </summary>
        public Pointer TilePalettePointer { get; set; }

        public uint DSi_Uint_1C { get; set; }

        public byte GBA_Byte_14 { get; set; }
        public byte GBA_Byte_15 { get; set; }
        public byte BackgroundIndex { get; set; }
        public byte ParallaxBackgroundIndex { get; set; }

        // 1 << 0: Compress map data
        // 1 << 1: Compress tile palette indices
        public uint CompressionFlags { get; set; }

        #endregion

        #region Parsed from Pointers

        public byte[] TileData { get; set; }

        /// <summary>
        /// The map data
        /// </summary>
        public MapData MapData { get; set; }

        /// <summary>
        /// The 10 available tile palettes (16 colors each)
        /// </summary>
        public RGBA5551Color[] TilePalettes { get; set; }

        public byte[] TilePaletteIndices { get; set; }

        public ushort[] TileBlockIndices { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            var settings = s.GetRequiredSettings<Ray1Settings>();

            if (settings.EngineVersion == Ray1EngineVersion.GBA)
            {
                // Serialize values
                TileDataPointer = s.SerializePointer(TileDataPointer, name: nameof(TileDataPointer));
                MapDataPointer = s.SerializePointer(MapDataPointer, name: nameof(MapDataPointer));
                TilePaletteIndicesPointer = s.SerializePointer(TilePaletteIndicesPointer, name: nameof(TilePaletteIndicesPointer));
                TileBlockIndicesPointer = s.SerializePointer(TileBlockIndicesPointer, name: nameof(TileBlockIndicesPointer));
                TilePalettePointer = s.SerializePointer(TilePalettePointer, name: nameof(TilePalettePointer));
                GBA_Byte_14 = s.Serialize<byte>(GBA_Byte_14, name: nameof(GBA_Byte_14));
                GBA_Byte_15 = s.Serialize<byte>(GBA_Byte_15, name: nameof(GBA_Byte_15));
                BackgroundIndex = s.Serialize<byte>(BackgroundIndex, name: nameof(BackgroundIndex));
                ParallaxBackgroundIndex = s.Serialize<byte>(ParallaxBackgroundIndex, name: nameof(ParallaxBackgroundIndex));
                CompressionFlags = s.Serialize<uint>(CompressionFlags, name: nameof(CompressionFlags));
            }
            else if (settings.EngineVersion == Ray1EngineVersion.DSi)
            {
                // Serialize values
                DSi_Uint_00 = s.Serialize<uint>(DSi_Uint_00, name: nameof(DSi_Uint_00));
                TileDataPointer = s.SerializePointer(TileDataPointer, name: nameof(TileDataPointer));
                DSi_Uint_08 = s.Serialize<uint>(DSi_Uint_08, name: nameof(DSi_Uint_08));
                MapDataPointer = s.SerializePointer(MapDataPointer, name: nameof(MapDataPointer));
                DSi_Uint_10 = s.Serialize<uint>(DSi_Uint_10, name: nameof(DSi_Uint_10));
                TileBlockIndicesPointer = s.SerializePointer(TileBlockIndicesPointer, name: nameof(TileBlockIndicesPointer));
                TilePalettePointer = s.SerializePointer(TilePalettePointer, name: nameof(TilePalettePointer));
                DSi_Uint_1C = s.Serialize<uint>(DSi_Uint_1C, name: nameof(DSi_Uint_1C));
            }
        }

        public void SerializeLevelData(SerializerObject s) 
        {
            var settings = s.GetRequiredSettings<Ray1Settings>();

            if (settings.EngineVersion == Ray1EngineVersion.GBA)
            {
                s.DoAt(MapDataPointer, () => 
                {
                    if ((CompressionFlags & 1) == 1)
                        s.DoEncoded(new LZSSEncoder(), () => MapData = s.SerializeObject<MapData>(MapData, name: nameof(MapData)));
                    else
                        MapData = s.SerializeObject<MapData>(MapData, name: nameof(MapData));
                });
                s.DoAt(TilePaletteIndicesPointer, () => 
                {
                    if ((CompressionFlags & 2) == 2)
                    {
                        s.DoEncoded(new LZSSEncoder(), () => TilePaletteIndices = s.SerializeArray<byte>(TilePaletteIndices, s.CurrentLength, name: nameof(TilePaletteIndices)));
                    }
                    else
                    {
                        var numTileBlocks = (TilePaletteIndicesPointer.AbsoluteOffset - TileBlockIndicesPointer.AbsoluteOffset) / 2;
                        TilePaletteIndices = s.SerializeArray<byte>(TilePaletteIndices, numTileBlocks, name: nameof(TilePaletteIndices));
                    }
                });

                TileBlockIndices = s.DoAt(TileBlockIndicesPointer, () => 
                    s.SerializeArray<ushort>(TileBlockIndices, TilePaletteIndices.Length, name: nameof(TileBlockIndices)));
                TilePalettes = s.DoAt(TilePalettePointer, () => 
                    s.SerializeObjectArray<RGBA5551Color>(TilePalettes, 10 * 16, name: nameof(TilePalettes)));

                ushort maxBlockIndex = TileBlockIndices.Max();
                TileData = s.DoAt(TileDataPointer, () => 
                    s.SerializeArray<byte>(TileData, 0x20 * ((uint)maxBlockIndex + 1), name: nameof(TileData)));
            }
            else if (settings.EngineVersion == Ray1EngineVersion.DSi)
            {
                s.DoAt(MapDataPointer, () => s.DoEncoded(new LZSSEncoder(), () => 
                    MapData = s.SerializeObject<MapData>(MapData, name: nameof(MapData))));
                s.DoAt(TileDataPointer, () => s.DoEncoded(new LZSSEncoder(), () => 
                    TileData = s.SerializeArray<byte>(TileData, s.CurrentLength, name: nameof(TileData))));
                TilePalettes = s.DoAt(TilePalettePointer, () => 
                    s.SerializeObjectArray<RGBA5551Color>(TilePalettes, 256, name: nameof(TilePalettes)));
                TileBlockIndices = s.DoAt(TileBlockIndicesPointer, () => 
                    s.SerializeArray<ushort>(TileBlockIndices, (MapData.Blocks.Max(t => t.TileIndex) + 1) * 4, name: nameof(TileBlockIndices)));
            }
        }

        #endregion
    }
}