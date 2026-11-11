namespace BinarySerializer.Ray1
{
    /// <summary>
    /// World map info
    /// </summary>
    public class WorldInfo : BinarySerializable
    {
        public short XPosition { get; set; }
        public short YPosition { get; set; }
        public short UpIndex { get; set; }
        public short DownIndex { get; set; }
        public short LeftIndex { get; set; }
        public short RightIndex { get; set; }
        public bool IsUnlocked { get; set; }
        public bool HasDrawnPath { get; set; }
        public bool IsUnlocking { get; set; }
        public byte CollectedCages { get; set; }
        public World World { get; set; }
        public byte Level { get; set; }
        public byte WorldNameColor { get; set; }
        public byte LevelNameColor { get; set; }
        public Pointer LevelNamePointer { get; set; }

        // EDU/KIT
        public bool EDU_Flag_03 { get; set; } // For when level is completed?
        public bool EDU_Byte_09 { get; set; } // 0-3, used for game progress
        public sbyte LivesCount { get; set; }
        public PelletType Type { get; set; }
        public short WorldName { get; set; }
        public short LevelName { get; set; }
        public string LoadingVignette { get; set; }
        public LevelLinkEntry[][] LevelLinks { get; set; } // Difficulty, level
        public byte CurrentLinkedLevel { get; set; }
        public byte?[][] LevelVariants { get; set; } // Each level can have several variants, randomly set when starting a new game
        public Pointer RunningDemoPointer { get; set; }
        public uint PCPacked_RunningDemoPointer { get; set; }
        public Record RunningDemo { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            if (settings.EngineVersion == Ray1EngineVersion.PC_Edu || 
                settings.EngineVersion == Ray1EngineVersion.PS1_Edu ||
                settings.EngineVersion == Ray1EngineVersion.PC_Kit ||
                settings.EngineVersion == Ray1EngineVersion.PC_Fan)
            {
                XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
                YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
                UpIndex = s.Serialize<byte>((byte)UpIndex, name: nameof(UpIndex));
                DownIndex = s.Serialize<byte>((byte)DownIndex, name: nameof(DownIndex));
                LeftIndex = s.Serialize<byte>((byte)LeftIndex, name: nameof(LeftIndex));
                RightIndex = s.Serialize<byte>((byte)RightIndex, name: nameof(RightIndex));
                
                s.DoBits<byte>(b =>
                {
                    IsUnlocked = b.SerializeBits<bool>(IsUnlocked, 1, name: nameof(IsUnlocked));
                    HasDrawnPath = b.SerializeBits<bool>(HasDrawnPath, 1, name: nameof(HasDrawnPath));
                    IsUnlocking = b.SerializeBits<bool>(IsUnlocking, 1, name: nameof(IsUnlocking));
                    EDU_Flag_03 = b.SerializeBits<bool>(EDU_Flag_03, 1, name: nameof(EDU_Flag_03));
                    b.SerializePadding(4, logIfNotNull: true);
                });
                EDU_Byte_09 = s.Serialize<bool>(EDU_Byte_09, name: nameof(EDU_Byte_09));
                LivesCount = s.Serialize<sbyte>(LivesCount, name: nameof(LivesCount));
                Type = s.Serialize<PelletType>(Type, name: nameof(Type));
                WorldNameColor = s.Serialize<byte>(WorldNameColor, name: nameof(WorldNameColor));
                LevelNameColor = s.Serialize<byte>(LevelNameColor, name: nameof(LevelNameColor));
                WorldName = s.Serialize<short>(WorldName, name: nameof(WorldName));
                LevelName = s.Serialize<short>(LevelName, name: nameof(LevelName));
                LoadingVignette = s.SerializeString(LoadingVignette, 9, name: nameof(LoadingVignette));
                World = s.Serialize<World>(World, name: nameof(World));

                LevelLinks = s.InitializeArray(LevelLinks, 5);
                s.DoArray(LevelLinks, (x, _, name) => s.SerializeObjectArray<LevelLinkEntry>(x, 6, name: name), name: nameof(LevelLinks));

                CurrentLinkedLevel = s.Serialize<byte>(CurrentLinkedLevel, name: nameof(CurrentLinkedLevel));

                LevelVariants = s.InitializeArray(LevelVariants, 5);
                s.DoArray(LevelVariants, (x, _, name) => s.SerializeNullableArray<byte>(x, 6, name: name), name: nameof(LevelVariants));

                if (settings.IsLoadingPackedPCData)
                {
                    PCPacked_RunningDemoPointer = s.Serialize<uint>(PCPacked_RunningDemoPointer, name: nameof(PCPacked_RunningDemoPointer));

                    // The actual demo data is stored after the map define when packed
                }
                else
                {
                    if (settings.EngineVersion == Ray1EngineVersion.PS1_Edu)
                        s.Align();

                    RunningDemoPointer = s.SerializePointer(RunningDemoPointer, name: nameof(RunningDemoPointer));

                    s.DoAt(RunningDemoPointer, () => RunningDemo = s.SerializeObject<Record>(RunningDemo, name: nameof(RunningDemo)));
                }
            }
            else if (settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol6)
            {
                XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
                YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
                UpIndex = s.Serialize<short>(UpIndex, name: nameof(UpIndex));
                DownIndex = s.Serialize<short>(DownIndex, name: nameof(DownIndex));
                LeftIndex = s.Serialize<short>(LeftIndex, name: nameof(LeftIndex));
                RightIndex = s.Serialize<short>(RightIndex, name: nameof(RightIndex));
                IsUnlocked = s.Serialize<bool>(IsUnlocked, name: nameof(IsUnlocked));
                IsUnlocking = s.Serialize<bool>(IsUnlocking, name: nameof(IsUnlocking)); // Unsure about this

                // Game stores these as shorts
                World = s.Serialize<World>(World, name: nameof(World));
                s.SerializePadding(1, logIfNotNull: true);
                Level = s.Serialize<byte>(Level, name: nameof(Level));
                s.SerializePadding(1, logIfNotNull: true);

                HasDrawnPath = s.Serialize<bool>(HasDrawnPath, name: nameof(HasDrawnPath));
                CollectedCages = s.Serialize<byte>(CollectedCages, name: nameof(CollectedCages));
            }
            else
            {
                XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
                YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
                UpIndex = s.Serialize<byte>((byte)UpIndex, name: nameof(UpIndex));
                DownIndex = s.Serialize<byte>((byte)DownIndex, name: nameof(DownIndex));
                LeftIndex = s.Serialize<byte>((byte)LeftIndex, name: nameof(LeftIndex));
                RightIndex = s.Serialize<byte>((byte)RightIndex, name: nameof(RightIndex));
                s.DoBits<byte>(b =>
                {
                    IsUnlocked = b.SerializeBits<bool>(IsUnlocked, 1, name: nameof(IsUnlocked));
                    HasDrawnPath = b.SerializeBits<bool>(HasDrawnPath, 1, name: nameof(HasDrawnPath));
                    IsUnlocking = b.SerializeBits<bool>(IsUnlocking, 1, name: nameof(IsUnlocking));
                    b.SerializePadding(5, logIfNotNull: true);
                });
                CollectedCages = s.Serialize<byte>(CollectedCages, name: nameof(CollectedCages));
                World = s.Serialize<World>(World, name: nameof(World));
                Level = s.Serialize<byte>(Level, name: nameof(Level));
                LevelNameColor = s.Serialize<byte>(LevelNameColor, name: nameof(LevelNameColor));
                s.SerializePadding(3, logIfNotNull: true);
                LevelNamePointer = s.SerializePointer(LevelNamePointer, allowInvalid: true, name: nameof(LevelNamePointer));
            }
        }

        public enum PelletType : byte
        {
            Normal = 0,
            Save = 1,
            Final = 2,
            Start = 3,
            Demo = 4,
        }
    }
}