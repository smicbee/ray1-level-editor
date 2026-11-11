namespace BinarySerializer.Ray1.PC
{
    /// <summary>
    /// Tile-set data in normal mode for PC
    /// </summary>
    public class TileSetNormal : BinarySerializable
    {
        /// <summary>
        /// The offset table for the blocks in <see cref="MapBlocks"/>
        /// </summary>
        public uint[] BlocksOffsetTable { get; set; }

        /// <summary>
        /// The total amount of <see cref="TileSetNormalMapBlocks.OpaqueBlocks"/> and <see cref="TileSetNormalMapBlocks.TransparentBlocks"/>
        /// </summary>
        public uint TotalBlocksCount { get; set; }

        /// <summary>
        /// The amount of <see cref="TileSetNormalMapBlocks.OpaqueBlocks"/>
        /// </summary>
        public uint OpaqueBlocksCount { get; set; }

        /// <summary>
        /// The byte size of <see cref="MapBlocks"/>
        /// </summary>
        public uint MapBlocksSize { get; set; }

        public TileSetNormalMapBlocks MapBlocks { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            if (settings.EngineVersion is Ray1EngineVersion.PC_Kit or Ray1EngineVersion.PC_Edu or Ray1EngineVersion.PC_Fan)
            {
                s.DoProcessed(new Checksum8Processor(), p =>
                {
                    p.Serialize<byte>(s, "TileSetNormalChecksum");

                    s.DoProcessed(new Xor8Processor(0xFF), () =>
                    {
                        BlocksOffsetTable = s.SerializeArray<uint>(BlocksOffsetTable, 1200, name: nameof(BlocksOffsetTable));
                        TotalBlocksCount = s.Serialize<uint>(TotalBlocksCount, name: nameof(TotalBlocksCount));
                        OpaqueBlocksCount = s.Serialize<uint>(OpaqueBlocksCount, name: nameof(OpaqueBlocksCount));
                        MapBlocksSize = s.Serialize<uint>(MapBlocksSize, name: nameof(MapBlocksSize));
                    });

                    MapBlocks = s.SerializeObject<TileSetNormalMapBlocks>(MapBlocks, name: nameof(MapBlocks));
                });
            }
            else
            {
                BlocksOffsetTable = s.SerializeArray<uint>(BlocksOffsetTable, 1200, name: nameof(BlocksOffsetTable));
                TotalBlocksCount = s.Serialize<uint>(TotalBlocksCount, name: nameof(TotalBlocksCount));
                OpaqueBlocksCount = s.Serialize<uint>(OpaqueBlocksCount, name: nameof(OpaqueBlocksCount));
                MapBlocksSize = s.Serialize<uint>(MapBlocksSize, name: nameof(MapBlocksSize));

                s.DoProcessed(new Checksum8Processor(), p =>
                {
                    MapBlocks = s.SerializeObject<TileSetNormalMapBlocks>(MapBlocks, x =>
                    {
                        x.Pre_OpaqueBlocksCount = OpaqueBlocksCount;
                        x.Pre_TransparentBlocksCount = TotalBlocksCount - OpaqueBlocksCount;
                    }, name: nameof(MapBlocks));
                    p.Serialize<byte>(s, "MapBlockChecksum");
                });
            }
        }
    }
}