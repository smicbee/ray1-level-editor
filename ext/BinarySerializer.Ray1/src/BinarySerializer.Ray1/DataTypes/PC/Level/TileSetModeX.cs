using System;

namespace BinarySerializer.Ray1.PC
{
    /// <summary>
    /// Tile-set data in ModeX for PC
    /// </summary>
    public class TileSetModeX : BinarySerializable
    {
        public byte[][] RoughBlockTexturesData { get; set; } // GrosPatai - appears to be 4 8x8 tiles?
        public Pointer[] RoughBlockTexturesOffsetTable { get; set; }

        public byte[] BlocksCode { get; set; } // Each item here has a different length
        public Pointer[] BlocksCodeOffsetTable { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // NOTE: This data is only parsed by the game if the rough textures should be used. Otherwise it skips to the texture block pointer.

            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            if (settings.EngineVersion is Ray1EngineVersion.PC_Kit or Ray1EngineVersion.PC_Edu or Ray1EngineVersion.PC_Fan)
            {
                s.DoProcessed(new Checksum8Processor(), p =>
                {
                    p.Serialize<byte>(s, "RoughBlockTexturesChecksum");

                    s.DoProcessed(new Xor8Processor(0x7D), () =>
                    {
                        RoughBlockTexturesData = s.SerializeArraySize<byte[], uint>(RoughBlockTexturesData,
                            name: nameof(RoughBlockTexturesData));

                        Pointer roughTexturesAnchor = s.CurrentPointer;

                        s.DoArray(RoughBlockTexturesData, (x, _, name) => s.SerializeArray<byte>(x, 256, name: name),
                            name: nameof(RoughBlockTexturesData));

                        RoughBlockTexturesOffsetTable = s.SerializePointerArray(RoughBlockTexturesOffsetTable, 1200,
                            anchor: roughTexturesAnchor,
                            name: nameof(RoughBlockTexturesOffsetTable));
                    });
                });

                s.DoProcessed(new Checksum8Processor(), p =>
                {
                    p.Serialize<byte>(s, "BlocksCodeChecksum");

                    s.DoProcessed(new Xor8Processor(0xF3), () =>
                    {
                        BlocksCode = s.SerializeArraySize<byte, uint>(BlocksCode, name: nameof(BlocksCode));

                        Pointer blocksCodeAnchor = s.CurrentPointer;

                        BlocksCode = s.SerializeArray<byte>(BlocksCode, BlocksCode.Length, name: nameof(BlocksCode));

                        BlocksCodeOffsetTable = s.SerializePointerArray(BlocksCodeOffsetTable, 1200,
                            anchor: blocksCodeAnchor, nullValue: UInt32.MaxValue, name: nameof(BlocksCodeOffsetTable));
                    });
                });
            }
            else
            {
                RoughBlockTexturesData = s.SerializeArraySize<byte[], uint>(RoughBlockTexturesData, name: nameof(RoughBlockTexturesData));
                BlocksCode = s.SerializeArraySize<byte, uint>(BlocksCode, name: nameof(BlocksCode));

                Pointer roughTexturesAnchor = s.CurrentPointer;
                s.DoProcessed(new Checksum8Processor(), p =>
                {
                    s.DoProcessed(new Xor8Processor(0x7D),
                        () =>
                        {
                            s.DoArray(RoughBlockTexturesData, (x, _, name) => s.SerializeArray<byte>(x, 256, name: name),
                                name: nameof(RoughBlockTexturesData));
                        });

                    p.Serialize<byte>(s, "RoughBlockTexturesDataChecksum");
                });

                RoughBlockTexturesOffsetTable = s.SerializePointerArray(RoughBlockTexturesOffsetTable, 1200,
                    anchor: roughTexturesAnchor,
                    name: nameof(RoughBlockTexturesOffsetTable));

                Pointer blocksCodeAnchor = s.CurrentPointer;
                s.DoProcessed(new Checksum8Processor(), p =>
                {
                    s.DoProcessed(new Xor8Processor(0xF3),
                        () =>
                        {
                            BlocksCode =
                                s.SerializeArray<byte>(BlocksCode, BlocksCode.Length, name: nameof(BlocksCode));
                        });

                    p.Serialize<byte>(s, "BlocksCodeChecksum");
                });

                BlocksCodeOffsetTable = s.SerializePointerArray(BlocksCodeOffsetTable, 1200, anchor: blocksCodeAnchor,
                    nullValue: UInt32.MaxValue, name: nameof(BlocksCodeOffsetTable));
            }
        }
    }
}