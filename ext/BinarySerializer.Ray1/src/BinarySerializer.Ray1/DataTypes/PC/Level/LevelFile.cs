namespace BinarySerializer.Ray1.PC
{
    /// <summary>
    /// Level data for PC
    /// </summary>
    public class LevelFile : BinarySerializable
    {
        public GameVersion GameVersion { get; set; }

        /// <summary>
        /// The pointer to the object block. The game uses this to skip the texture block if the game should use the rough textures.
        /// </summary>
        public Pointer ObjectsPointer { get; set; }

        /// <summary>
        /// The pointer to the texture block. The game uses this to skip the rough the textures if the game should use the normal textures.
        /// </summary>
        public Pointer TileSetNormalPointer { get; set; }

        public LevelDefine LevelDefine { get; set; }
        public BackgroundDefine BackgroundDefineNormal { get; set; }
        public BackgroundDefine BackgroundDefineDiff { get; set; }

        public MapInfo MapInfo { get; set; }

        public byte FondIndex { get; set; }
        public byte ScrollDiffFondIndex { get; set; }
        public int ScrollDiffSprites { get; set; } // DES index

        public TileSetModeX TileSetModeX { get; set; }
        public byte[] TileSetModeXLeftoverData { get; set; }
        public TileSetNormal TileSetNormal { get; set; }

        public LevelObjects ObjData { get; set; }

        public ProfileDefine ProfileDefine { get; set; }

        public byte[][] Alpha { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            // Serialize the header
            if (settings.IsVersioned)
                GameVersion = s.SerializeObject<GameVersion>(GameVersion, name: nameof(GameVersion));

            Pointer pointersOffset = s.CurrentPointer;

            // Serialize the pointers
            bool allowInvalid = settings.PCVersion is Ray1PCVersion.PocketPC or Ray1PCVersion.Android or Ray1PCVersion.iOS;
            ObjectsPointer = s.SerializePointer(ObjectsPointer, allowInvalid: allowInvalid, name: nameof(ObjectsPointer));
            TileSetNormalPointer = s.SerializePointer(TileSetNormalPointer, allowInvalid: allowInvalid, name: nameof(TileSetNormalPointer));

            // Serialize the level defines
            if (settings.EngineVersion is Ray1EngineVersion.PC_Kit or Ray1EngineVersion.PC_Edu or Ray1EngineVersion.PC_Fan)
            {
                LevelDefine = s.SerializeObject<LevelDefine>(LevelDefine, name: nameof(LevelDefine));
                BackgroundDefineNormal = s.SerializeObject<BackgroundDefine>(BackgroundDefineNormal, name: nameof(BackgroundDefineNormal));
                BackgroundDefineDiff = s.SerializeObject<BackgroundDefine>(BackgroundDefineDiff, name: nameof(BackgroundDefineDiff));
            }

            // Serialize the map data
            MapInfo = s.SerializeObject<MapInfo>(MapInfo, name: nameof(MapInfo));

            // Serialize the background data
            if (settings.EngineVersion is Ray1EngineVersion.PC or Ray1EngineVersion.PocketPC)
            {
                FondIndex = s.Serialize<byte>(FondIndex, name: nameof(FondIndex));
                ScrollDiffFondIndex = s.Serialize<byte>(ScrollDiffFondIndex, name: nameof(ScrollDiffFondIndex));
            }
            
            ScrollDiffSprites = s.Serialize<int>(ScrollDiffSprites, name: nameof(ScrollDiffSprites));

            // Serialize the rough block textures
            if (settings.EngineVersion == Ray1EngineVersion.PocketPC)
            {
                // Leftover data. Usually (always?) just the first 12 bytes.
                long length = TileSetNormalPointer.FileOffset - s.CurrentPointer.FileOffset;
                TileSetModeXLeftoverData = s.SerializeArray<byte>(TileSetModeXLeftoverData, length, name: nameof(TileSetModeXLeftoverData));
            }
            else
            {
                TileSetModeX = s.SerializeObject<TileSetModeX>(TileSetModeX, name: nameof(TileSetModeX));
            }

            // At this point the stream position should match the texture block offset
            if (s.CurrentPointer != TileSetNormalPointer)
                s.Context.SystemLogger?.LogWarning("Normal block textures offset is incorrect");

            // Serialize the normal block textures
            TileSetNormal = s.SerializeObject<TileSetNormal>(TileSetNormal, name: nameof(TileSetNormal));

            // At this point the stream position should match the obj block offset (ignore the Pocket PC version here since it uses leftover pointers from PC version)
            if (settings.EngineVersion != Ray1EngineVersion.PocketPC && s.CurrentPointer != ObjectsPointer)
                s.Context.SystemLogger?.LogWarning("Object offset is incorrect");

            // Serialize the object data
            ObjData = s.SerializeObject<LevelObjects>(ObjData, name: nameof(ObjData));

            // Serialize the profile define data (only on By his Fans and 60 Levels)
            if (settings.EngineVersion == Ray1EngineVersion.PC_Fan)
                ProfileDefine = s.SerializeObject<ProfileDefine>(ProfileDefine, name: nameof(ProfileDefine));

            // Serialize alpha data (only on EDU)
            if (settings.EngineVersion == Ray1EngineVersion.PC_Edu)
            {
                s.DoProcessed(new Checksum8Processor(), p =>
                {
                    p.Serialize<byte>(s, "AlphaChecksum");

                    Alpha = s.InitializeArray(Alpha, 480);
                    s.DoArray(Alpha, (x, _, name) => s.SerializeArray<byte>(x, 256, name: name), name: nameof(Alpha));
                });
            }

            // Correct pointers
            s.DoAt(pointersOffset, () =>
            {
                s.SerializePointer(ObjData.Offset, allowInvalid: allowInvalid, name: nameof(ObjectsPointer));
                s.SerializePointer(TileSetNormal.Offset, allowInvalid: allowInvalid, name: nameof(TileSetNormalPointer));
            });
        }
    }
}