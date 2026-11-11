namespace BinarySerializer.Ray1
{
    public class Level : BinarySerializable
    {
        public Pointer ObjectsPointer { get; set; }
        public Pointer AlwaysObjectsPointer { get; set; }
        public Pointer SpritesPointer { get; set; }
        public uint Uint_0C { get; set; }
        public int[] MapLengths { get; set; }
        public Pointer[] MapBlocks { get; set; }
        public Pointer ModifyTileIndices { get; set; }
        public int AllowedTime { get; set; }
        public short Short_28 { get; set; }
        public short ObjectsCount { get; set; }
        public short AlwaysObjectsCount { get; set; }
        public short AllocatedObjectsCount { get; set; }
        public short AllocatableObjectsCount { get; set; }
        public short SpritesCount { get; set; }
        public short FixSpritesCount { get; set; }
        public MapDimensions[] MapDimensions { get; set; }
        public byte Byte_3E { get; set; }
        public LevelEffects Effects { get; set; }

        // Serialized from pointers
        public ObjData[] Objects { get; set; }
        public R2_ObjData[] R2_Objects { get; set; } // Allocated and allocatable objects
        public R2_ObjData[] AlwaysObjects { get; set; } // Templates used to allocate always objects
        public Sprite[] FixSprites { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            if (settings.EngineVersion == Ray1EngineVersion.R2_PS1)
            {
                ObjectsPointer = s.SerializePointer(ObjectsPointer, name: nameof(ObjectsPointer));
                AlwaysObjectsPointer = s.SerializePointer(AlwaysObjectsPointer, name: nameof(AlwaysObjectsPointer));
                SpritesPointer = s.SerializePointer(SpritesPointer, name: nameof(SpritesPointer));
                Uint_0C = s.Serialize<uint>(Uint_0C, name: nameof(Uint_0C));
                MapLengths = s.SerializeArray<int>(MapLengths, 2, name: nameof(MapLengths));
                MapBlocks = s.SerializePointerArray(MapBlocks, 2, name: nameof(MapBlocks));
                ModifyTileIndices = s.SerializePointer(ModifyTileIndices, name: nameof(ModifyTileIndices));
                AllowedTime = s.Serialize<int>(AllowedTime, name: nameof(AllowedTime));
                Short_28 = s.Serialize<short>(Short_28, name: nameof(Short_28));
                ObjectsCount = s.Serialize<short>(ObjectsCount, name: nameof(ObjectsCount));
                AlwaysObjectsCount = s.Serialize<short>(AlwaysObjectsCount, name: nameof(AlwaysObjectsCount));
                AllocatedObjectsCount = s.Serialize<short>(AllocatedObjectsCount, name: nameof(AllocatedObjectsCount));
                AllocatableObjectsCount = s.Serialize<short>(AllocatableObjectsCount, name: nameof(AllocatableObjectsCount));
                SpritesCount = s.Serialize<short>(SpritesCount, name: nameof(SpritesCount));
                FixSpritesCount = s.Serialize<short>(FixSpritesCount, name: nameof(FixSpritesCount));
                MapDimensions = s.SerializeObjectArray<MapDimensions>(MapDimensions, 2, name: nameof(MapDimensions));
                Byte_3E = s.Serialize<byte>(Byte_3E, name: nameof(Byte_3E));
                Effects = s.Serialize<LevelEffects>(Effects, name: nameof(Effects));
                s.SerializePadding(3);

                // Serialize data from pointers
                s.DoAt(ObjectsPointer, () =>
                    R2_Objects = s.SerializeObjectArray<R2_ObjData>(R2_Objects, ObjectsCount, name: nameof(R2_Objects)));
                s.DoAt(AlwaysObjectsPointer, () =>
                    AlwaysObjects = s.SerializeObjectArray<R2_ObjData>(AlwaysObjects, AlwaysObjectsCount, name: nameof(AlwaysObjects)));

                // In memory the level sprites get allocated to the end of the fix array so they're all
                // read as the same array. But since we serialize on a per file basis we can only
                // serialize the fix sprites here, unless serializing from memory.
                s.DoAt(SpritesPointer, () =>
                    FixSprites = s.SerializeObjectArray<Sprite>(FixSprites, FixSpritesCount, name: nameof(FixSprites)));
            }
            else
            {
                ObjectsPointer = s.SerializePointer(ObjectsPointer, name: nameof(ObjectsPointer));
                ObjectsCount = s.Serialize<short>(ObjectsCount, name: nameof(ObjectsCount));

                // Serialize data from pointers
                s.DoAt(ObjectsPointer, () =>
                    Objects = s.SerializeObjectArray<ObjData>(Objects, ObjectsCount, name: nameof(Objects)));
            }
        }
    }
}