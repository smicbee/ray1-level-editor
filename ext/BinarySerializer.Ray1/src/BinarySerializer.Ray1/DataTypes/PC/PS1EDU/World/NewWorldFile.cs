using System;
using System.Linq;

namespace BinarySerializer.Ray1.PC.PS1EDU
{
    /// <summary>
    /// World data for EDU on PS1
    /// </summary>
    public class NewWorldFile : BinarySerializable
    {
        #region Public Properties

        public Type Pre_FileType { get; set; }

        public ushort BG1 { get; set; }

        public ushort BG2 { get; set; }

        public byte Plan0NumPcxCount { get; set; }

        public string[] Plan0NumPcxFiles { get; set; }

        public ushort DESCount { get; set; }
        
        public byte ETACount { get; set; }

        public uint DESBlockLength { get; set; }

        public DESTemplate[] DESData { get; set; }

        public WorldDefine WorldDefine { get; set; }

        public uint MainDataBlockLength { get; set; }

        public Pointer MainDataBlockPointer { get; set; }

        public Sprite[][] SpriteCollections { get; set; }

        public Animation[][] Animations { get; set; }

        /// <summary>
        /// The obj states for every ETA
        /// </summary>
        public ObjState[][][] ETA { get; set; }

        // Index table for DES. not sure what for yet
        public uint[] DESDataIndices { get; set; }

        public byte ETAStateCountTableCount { get; set; }

        public byte[] ETAStateCountTable { get; set; }

        public byte ETASubStateCountTableCount { get; set; }

        public byte[] ETASubStateCountTable { get; set; }

        public uint AnimationLayersBlockSizeTableCount { get; set; }

        public ushort[] AnimationLayersBlockSizeTable { get; set; }

        public AnimationLayer[] AnimationLayers { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            if (Pre_FileType == Type.World)
            {
                // Serialize header
                BG1 = s.Serialize<ushort>(BG1, name: nameof(BG1));
                BG2 = s.Serialize<ushort>(BG2, name: nameof(BG2));
                Plan0NumPcxCount = s.Serialize<byte>(Plan0NumPcxCount, name: nameof(Plan0NumPcxCount));
                s.DoProcessed(new Xor8Processor(0x19), () => Plan0NumPcxFiles = s.SerializeStringArray(Plan0NumPcxFiles, Plan0NumPcxCount, 8, name: nameof(Plan0NumPcxFiles)));

                // Serialize counts
                DESCount = s.Serialize<ushort>(DESCount, name: nameof(DESCount));
                ETACount = s.Serialize<byte>(ETACount, name: nameof(ETACount));
                DESBlockLength = s.Serialize<uint>(DESBlockLength, name: nameof(DESBlockLength));
            }
            else
            {
                // Serialize header
                ETACount = s.Serialize<byte>(ETACount, name: nameof(ETACount));
                DESCount = s.Serialize<ushort>(DESCount, name: nameof(DESCount));
            }

            // Serialize DES data
            DESData = s.SerializeObjectArray<DESTemplate>(DESData, DESCount, name: nameof(DESData));

            if (Pre_FileType == Type.World)
                WorldDefine = WorldDefine = s.SerializeObject<WorldDefine>(WorldDefine, name: nameof(WorldDefine));

            // Serialize main data block length
            MainDataBlockLength = s.Serialize<uint>(MainDataBlockLength, name: nameof(MainDataBlockLength));

            // We parse the main data block later...
            MainDataBlockPointer = s.CurrentPointer;
            s.Goto(MainDataBlockPointer + MainDataBlockLength);

            if (Pre_FileType == Type.Allfix)
                DESDataIndices = s.SerializeArray<uint>(DESDataIndices, 8, name: nameof(DESDataIndices));

            // Serialize ETA tables
            if (Pre_FileType == Type.World)
            {
                ETAStateCountTableCount = s.Serialize<byte>(ETAStateCountTableCount, name: nameof(ETAStateCountTableCount));
                ETAStateCountTable = s.SerializeArray<byte>(ETAStateCountTable, ETAStateCountTableCount, name: nameof(ETAStateCountTable));
            }
            else
            {
                ETAStateCountTable = s.SerializeArray<byte>(ETAStateCountTable, ETACount, name: nameof(ETAStateCountTable));
            }
            ETASubStateCountTableCount = s.Serialize<byte>(ETASubStateCountTableCount, name: nameof(ETASubStateCountTableCount));
            ETASubStateCountTable = s.SerializeArray<byte>(ETASubStateCountTable, ETASubStateCountTableCount, name: nameof(ETASubStateCountTable));

            // Serialize animation layer table
            AnimationLayersBlockSizeTableCount = s.Serialize<uint>(AnimationLayersBlockSizeTableCount, name: nameof(AnimationLayersBlockSizeTableCount));
            AnimationLayersBlockSizeTable = s.SerializeArray<ushort>(AnimationLayersBlockSizeTable, AnimationLayersBlockSizeTableCount, name: nameof(AnimationLayersBlockSizeTable));

            // Serialize animation layers
            AnimationLayers = s.SerializeObjectArray<AnimationLayer>(AnimationLayers, 0xFE, name: nameof(AnimationLayers));

            // Serialize the main data block
            s.DoAt(MainDataBlockPointer, () =>
            {
                if (Pre_FileType == Type.World)
                {
                    SerializeDES();
                    SerializeETA();
                }
                else
                {
                    SerializeETA();
                    SerializeDES();
                }

                // Helper method for serializing the DES
                void SerializeDES()
                {
                    SpriteCollections ??= new Sprite[DESCount][];
                    Animations ??= new Animation[DESCount][];

                    int curAnimDesc = 0;

                    // Serialize data for every DES
                    for (int i = 0; i < DESCount; i++)
                    {
                        // Serialize sprites
                        if (DESData[i].SpritesCount > 0)
                            SpriteCollections[i] = s.SerializeObjectArray<Sprite>(SpriteCollections[i], DESData[i].SpritesCount, name: $"{nameof(SpriteCollections)}[{i}]");
                        else
                            SpriteCollections[i] = Array.Empty<Sprite>();

                        // Serialize animations
                        Animations[i] = s.SerializeObjectArray<Animation>(Animations[i], DESData[i].AnimationsCount, name: $"{nameof(Animations)}[{i}]");

                        // Serialize animation data
                        for (int j = 0; j < Animations[i].Length; j++)
                        {
                            var anim = Animations[i][j];

                            if (anim.FramesCount <= 0)
                            {
                                curAnimDesc++;
                                continue;
                            }

                            // Serialize layer data
                            anim.PS1EDU_CompressedLayers = s.SerializeArray<byte>(anim.PS1EDU_CompressedLayers, AnimationLayersBlockSizeTable[curAnimDesc], name: nameof(anim.PS1EDU_CompressedLayers));

                            // Padding...
                            if (AnimationLayersBlockSizeTable[curAnimDesc] % 4 != 0)
                            {
                                // Padding seems to contain garbage data in this case instead of 0xCD?
                                int paddingLength = 4 - AnimationLayersBlockSizeTable[curAnimDesc] % 4;
                                s.SerializeArray<byte>(Enumerable.Repeat((byte)0xCD, paddingLength).ToArray(), paddingLength, name: "Padding");
                            }

                            // Serialize frames
                            if (anim.PCPacked_FramesPointer != 0xFFFFFFFF)
                                anim.Frames = s.SerializeObjectArray<AnimationFrame>(anim.Frames, anim.FramesCount, name: nameof(anim.Frames));

                            // Parse layers
                            if (anim.Layers == null)
                                anim.PS1EDU_UncompressLayers(AnimationLayers);

                            curAnimDesc++;
                        }
                    }
                }

                // Helper method for serializing the ETA
                void SerializeETA()
                {
                    if (ETA == null)
                        ETA = new ObjState[ETACount][][];

                    var stateIndex = 0;

                    // Serialize every ETA
                    for (int i = 0; i < ETA.Length; i++)
                    {
                        if (ETA[i] == null)
                            ETA[i] = new ObjState[ETAStateCountTable[i]][];

                        // EDU serializes the pointer structs, but the pointers are invalid. They can be anything as they're overwritten with valid memory pointers upon load
                        uint[] pointerStructs = Enumerable.Repeat((uint)1, ETA[i].Length).ToArray();
                        _ = s.SerializeArray<uint>(pointerStructs, pointerStructs.Length, name: $"ETAPointers[{i}]");

                        // Serialize every state
                        for (int j = 0; j < ETA[i].Length; j++)
                        {
                            // Serialize sub-states
                            ETA[i][j] = s.SerializeObjectArray<ObjState>(ETA[i][j], ETASubStateCountTable[stateIndex], name: $"{nameof(ETA)}[{i}][{j}]");

                            stateIndex++;
                        }
                    }
                }
            });
        }

        #endregion

        #region Enums

        public enum Type
        {
            Allfix,
            World,
        }

        #endregion
    }
}