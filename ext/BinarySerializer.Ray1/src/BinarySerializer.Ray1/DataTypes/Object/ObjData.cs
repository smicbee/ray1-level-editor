using System;
using System.Linq;

namespace BinarySerializer.Ray1
{
    /// <summary>
    /// An object instance
    /// </summary>
    public class ObjData : BinarySerializable
    {
        #region Options

        // Sometimes we don't want to serialize all referenced data, so this can be used to
        // specify which data we want. Ideally this should be replaced with a more common
        // system in BinarySerializer for determining which data to serialize.
        public RefDataFlags Pre_SerializeRefDataFlags { get; set; } = RefDataFlags.All;

        #endregion

        #region Object data

        // These are indexes in the files and get replaced with pointers during runtime
        public uint PCPacked_SpritesIndex { get; set; }
        public uint PCPacked_AnimationsIndex { get; set; }
        public uint PCPacked_ImageBufferIndex { get; set; }
        public uint PCPacked_ETAIndex { get; set; }

        // Keep separate values for these to avoid invalid pointers when reading from the files
        public uint PCPacked_CommandsPointer { get; set; }
        public uint PCPacked_LabelOffsetsPointer { get; set; }

        public Pointer SpritesPointer { get; set; }
        public Pointer AnimationsPointer { get; set; }
        public Pointer ImageBufferPointer { get; set; } // Only valid for vol3 PS1 demo and PC
        public Pointer ETAPointer { get; set; }

        public Pointer CommandsPointer { get; set; }
        public Pointer LabelOffsetsPointer { get; set; }

        public CommandContext[] CommandContexts { get; set; }

        public int XPosition { get; set; }
        public int YPosition { get; set; }

        public byte GBA_Byte_1D { get; set; }

        public short Id { get; set; } // The object id/index

        public short ScreenXPosition { get; set; }
        public short ScreenYPosition { get; set; }
        public short Short_3A { get; set; }

        public short InitialXPosition { get; set; }
        public short InitialYPosition { get; set; }

        public short SpeedX { get; set; }
        public short SpeedY { get; set; }

        public ushort SpritesCount { get; set; }

        public short CommandOffset { get; set; }
        public short CommandsCount { get; set; }
        public short Short_4A { get; set; }
        public short FollowY { get; set; }
        public short FollowX { get; set; }

        // This value is used for voice lines as a replacement of the normal HitPoints value in order to have a
        // sample index higher than 255. When this is used HitPoints is always EDU_ExtHitPoints % 256.
        public uint EDU_ExtHitPoints { get; set; }
        
        public short Short_50 { get; set; }
        public short Short_52 { get; set; }
        public short RaymanDistance { get; set; }
        public short IFramesTimer { get; set; }
        public short TestBlockIndex { get; set; }
        public short Scale { get; set; }

        public ZDCReference TypeZDC { get; set; }
        public short ActiveTimer { get; set; }

        public ObjType Type { get; set; }

        public BlockType[] BlockTypes { get; set; }

        public byte OffsetBX { get; set; }
        public byte OffsetBY { get; set; }

        public byte AnimationIndex { get; set; }
        public byte AnimationFrame { get; set; }

        public byte SubEtat { get; set; }
        public byte MainEtat { get; set; }

        public byte InitialSubEtat { get; set; }
        public byte InitialMainEtat { get; set; }

        public CommandType Command { get; set; }
        public byte GravityValue1 { get; set; }
        public byte GravityValue2 { get; set; }
        public ChangeAnimationMode ChangeAnimationMode { get; set; }

        public byte OffsetHY { get; set; }

        /// <summary>
        /// The sprite index which uses the obj collision
        /// </summary>
        public byte FollowSprite { get; set; }

        public uint ActualHitPoints
        {
            get => Type == ObjType.EDU_VoiceLine ? EDU_ExtHitPoints : HitPoints;
            set
            {
                if (Type == ObjType.EDU_VoiceLine)
                    EDU_ExtHitPoints = value;

                HitPoints = (byte)(value % 256);
            }
        }

        public byte HitPoints { get; set; }
        public byte InitialHitPoints { get; set; }

        // Appears to be unused
        public byte InitFlag { get; set; }

        public byte HitSprite { get; set; }

        public ObjectActiveFlag ActiveFlag { get; set; }

        public byte DetectZone { get; set; }
        public byte DetectZoneFlag { get; set; }
        public byte CommandContextDepth { get; set; }
        public byte Byte_7D { get; set; }

        /// <summary>
        /// The layer the obj sprites gets drawn to, between 1 and 7.
        /// 0 means it doesn't get drawn.
        /// </summary>
        public byte DisplayPrio { get; set; }

        public byte Timer { get; set; }

        public byte AnimationsCount { get; set; }

        // Flags
        public bool FlipX { get; set; }
        public bool ReadCommands { get; set; }
        public bool UnknownFlag1 { get; set; } // Appears to indicate if object has been interacted with
        public bool IsLinked { get; set; }
        public bool UnknownFlag2 { get; set; }
        public bool UnknownFlag3 { get; set; }
        public bool UnknownFlag4 { get; set; }
        public bool UnknownFlag5 { get; set; }
        public bool CommandTest { get; set; }
        public bool IsAlive { get; set; }
        public bool IsActive { get; set; }
        public bool FollowEnabled { get; set; }

        // Unknown data
        public int UnknownFlags { get; set; }
        public short UnknownDemoValue { get; set; }
        public byte[] UnknownBytes { get; set; }

        #endregion

        #region Parsed from pointers

        public Sprite[] Sprites { get; set; }
        public Animation[] Animations { get; set; }
        public byte[] ImageBuffer { get; set; }
        public ETA ETA { get; set; }
        public ObjCommands Commands { get; set; }
        public ushort[] LabelOffsets { get; set; }

        #endregion

        #region Object Creation Methods

        public static ObjData CreateRayman(Context context, ObjData rayPos) =>
            new ObjData().InitRayman(context, rayPos);
        public static ObjData CreateMapObj(Context context, short x, short y, int index, WorldInfo.PelletType type = WorldInfo.PelletType.Normal) =>
            new ObjData().InitMapObj(context, x, y, index, type);

        public ObjData InitRayman(Context context, ObjData rayPos)
        {
            Ray1Settings settings = context.GetRequiredSettings<Ray1Settings>();

            OffsetBX = 80;
            OffsetBY = (byte)(settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol3 ||
                              settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol6 ? 80 : 78);

            if (rayPos != null)
            {
                XPosition = rayPos.XPosition + rayPos.OffsetBX - OffsetBX;
                YPosition = rayPos.YPosition + rayPos.OffsetBY - OffsetBY;
            }
            else
            {
                XPosition = 100;
                YPosition = 10;
            }

            Type = ObjType.TYPE_RAYMAN;
            SubEtat = 2;
            MainEtat = 2;
            OffsetHY = 20;
            FollowSprite = 0;
            HitPoints = 0;
            DisplayPrio = 7;
            HitSprite = 0;

            PCPacked_SpritesIndex = 1;
            PCPacked_AnimationsIndex = 1;
            PCPacked_ImageBufferIndex = 1;
            PCPacked_ETAIndex = 0;

            CommandContexts = new[] { new CommandContext() };
            BlockTypes = new BlockType[5];
            Commands = new ObjCommands() { Commands = new Command[0] };
            LabelOffsets = new ushort[0];
            TypeZDC = new ZDCReference();

            return this;
        }

        // Copied from INIT_CHEMIN
        public ObjData InitMapObj(Context context, short x, short y, int index, WorldInfo.PelletType type)
        {
            Ray1Settings settings = context.GetRequiredSettings<Ray1Settings>();

            Type = ObjType.TYPE_MEDAILLON;
            MainEtat = 5;

            // Set correct sub-etat and position
            if (settings.EngineVersion is Ray1EngineVersion.PC_Kit or Ray1EngineVersion.PC_Fan)
            {
                SubEtat = 69;

                // ?
                XPosition = x - 34;
                YPosition = y - 39;

                // ?
                //OffsetBX = 80;
                //OffsetBY = 64;
            }
            else if (settings.EngineVersion is Ray1EngineVersion.PC_Edu or Ray1EngineVersion.PS1_Edu)
            {
                if (type == WorldInfo.PelletType.Normal) // Normal
                    SubEtat = 39;
                else if (type == WorldInfo.PelletType.Final) // End
                    SubEtat = 55;
                else if (type == WorldInfo.PelletType.Demo) // Demo
                    SubEtat = 54;
                else if (type == WorldInfo.PelletType.Start) // Start point
                    SubEtat = 45;

                // ?
                XPosition = x - 34 - 36;
                YPosition = y - 39 - 25;

                // ?
                //OffsetBX = 80;
                //OffsetBY = 64;
            }
            else
            {
                XPosition = x - 70; // In the code it's 78 - why do we have to offset it differently here?
                YPosition = y - 64;

                OffsetBX = 80;
                OffsetBY = 64;

                // Mr Dark
                if (index == 17)
                    SubEtat = 59;
                else if (index > 17)
                    SubEtat = 58;
                else
                    SubEtat = 39;
            }

            PCPacked_SpritesIndex = 4;
            PCPacked_AnimationsIndex = 4;
            PCPacked_ImageBufferIndex = 4;
            PCPacked_ETAIndex = 2;

            CommandContexts = new[] { new CommandContext() };
            BlockTypes = new BlockType[5];
            Commands = new ObjCommands() { Commands = new Command[0] };
            LabelOffsets = new ushort[0];
            TypeZDC = new ZDCReference();

            return this;
        }

        #endregion

        #region Serialization Methods

        private void SerializeFromPointers(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            // Serialize the sprites
            if ((Pre_SerializeRefDataFlags & RefDataFlags.Sprites) != 0)
                s.DoAt(SpritesPointer, () => 
                    Sprites = s.SerializeObjectArray<Sprite>(Sprites, SpritesCount, name: nameof(Sprites)));

            // Serialize the animations
            if ((Pre_SerializeRefDataFlags & RefDataFlags.Animations) != 0)
                s.DoAt(AnimationsPointer, () =>
                    Animations = s.SerializeObjectArray<Animation>(Animations, AnimationsCount, name: nameof(Animations)));

            // Serialize image buffer
            if ((Pre_SerializeRefDataFlags & RefDataFlags.ImageBuffer) != 0)
                s.DoAt(ImageBufferPointer, () => 
                    ImageBuffer = s.SerializeArray<byte>(ImageBuffer, InternalHelpers.GetImageBufferLength(Sprites, settings), name: nameof(ImageBuffer)));

            // Serialize the commands
            if ((Pre_SerializeRefDataFlags & RefDataFlags.Commands) != 0)
            {
                // Serialize the commands
                s.DoAt(CommandsPointer, () =>
                    Commands = s.SerializeObject<ObjCommands>(Commands, name: nameof(Commands)));

                // Serialize the label offsets
                if (Commands != null && Commands.Commands.Length > 0)
                {
                    s.DoAt(LabelOffsetsPointer, () =>
                    {
                        if (LabelOffsets == null)
                        {
                            int length = Commands.Commands.Max(c => c.UsesLabelOffsets ? (int)c.Arguments[0] : -1) + 1;
                            LabelOffsets = new ushort[length];
                        }

                        // Serialize the label offsets
                        LabelOffsets = s.SerializeArray(LabelOffsets, LabelOffsets.Length, name: nameof(LabelOffsets));
                    });
                }
            }

            // Serialize ETA
            if ((Pre_SerializeRefDataFlags & RefDataFlags.States) != 0)
            {
                if (ETAPointer != null)
                    s.DoAt(ETAPointer, () => ETA = s.SerializeObject<ETA>(ETA, name: nameof(ETA)));

                if (ETA?.States?.ElementAtOrDefault(MainEtat)?.ElementAtOrDefault(SubEtat) == null)
                    s.Context.SystemLogger?.LogWarning($"Matching obj state not found for obj {Type} at {XPosition}x{YPosition} with E{MainEtat},SE{SubEtat} for {settings.EngineVersion} in {settings.World}{settings.Level}");
            }
        }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            if (settings.EngineBranch is Ray1EngineBranch.PC or Ray1EngineBranch.GBA)
            {
                if (settings.IsLoadingPackedPCData)
                {
                    PCPacked_SpritesIndex = s.Serialize<uint>(PCPacked_SpritesIndex, name: nameof(PCPacked_SpritesIndex));
                    PCPacked_AnimationsIndex = s.Serialize<uint>(PCPacked_AnimationsIndex, name: nameof(PCPacked_AnimationsIndex));
                    PCPacked_ImageBufferIndex = s.Serialize<uint>(PCPacked_ImageBufferIndex, name: nameof(PCPacked_ImageBufferIndex));
                    PCPacked_ETAIndex = s.Serialize<uint>(PCPacked_ETAIndex, name: nameof(PCPacked_ETAIndex));

                    PCPacked_CommandsPointer = s.Serialize<uint>(PCPacked_CommandsPointer, name: nameof(PCPacked_CommandsPointer));
                    PCPacked_LabelOffsetsPointer = s.Serialize<uint>(PCPacked_LabelOffsetsPointer, name: nameof(PCPacked_LabelOffsetsPointer));
                }
                else
                {
                    SpritesPointer = s.SerializePointer(SpritesPointer, name: nameof(SpritesPointer));
                    AnimationsPointer = s.SerializePointer(AnimationsPointer, name: nameof(AnimationsPointer));
                    ImageBufferPointer = s.SerializePointer(ImageBufferPointer, name: nameof(ImageBufferPointer));
                    ETAPointer = s.SerializePointer(ETAPointer, name: nameof(ETAPointer));

                    CommandsPointer = s.SerializePointer(CommandsPointer, name: nameof(CommandsPointer));
                    LabelOffsetsPointer = s.SerializePointer(LabelOffsetsPointer, name: nameof(LabelOffsetsPointer));
                }

                CommandContexts = s.SerializeObjectArray<CommandContext>(CommandContexts, 1, name: nameof(CommandContexts));

                if (settings.EngineVersion is Ray1EngineVersion.GBA or Ray1EngineVersion.DSi)
                {
                    ActiveFlag = s.Serialize<ObjectActiveFlag>(ActiveFlag, name: nameof(ActiveFlag));
                    GBA_Byte_1D = s.Serialize<byte>(GBA_Byte_1D, name: nameof(GBA_Byte_1D));
                    XPosition = s.Serialize<short>((short)XPosition, name: nameof(XPosition));
                    YPosition = s.Serialize<short>((short)YPosition, name: nameof(YPosition));
                }
                else
                {
                    UnknownFlag2 = s.SerializeBoolean<int>(UnknownFlag2, name: nameof(UnknownFlag2));
                    IsLinked = s.SerializeBoolean<int>(IsLinked, name: nameof(IsLinked));
                    IsActive = s.SerializeBoolean<int>(IsActive, name: nameof(IsActive));
                    XPosition = s.Serialize<int>(XPosition, name: nameof(XPosition));
                    YPosition = s.Serialize<int>(YPosition, name: nameof(YPosition));
                    ActiveFlag = s.Serialize<ObjectActiveFlag>(ActiveFlag, name: nameof(ActiveFlag));
                    s.SerializePadding(3);
                }

                Id = s.Serialize<short>(Id, name: nameof(Id));
                ScreenXPosition = s.Serialize<short>(ScreenXPosition, name: nameof(ScreenXPosition));
                ScreenYPosition = s.Serialize<short>(ScreenYPosition, name: nameof(ScreenYPosition));
                Short_3A = s.Serialize<short>(Short_3A, name: nameof(Short_3A));
                InitialXPosition = s.Serialize<short>(InitialXPosition, name: nameof(InitialXPosition));
                InitialYPosition = s.Serialize<short>(InitialYPosition, name: nameof(InitialYPosition));

                SpeedX = s.Serialize<short>(SpeedX, name: nameof(SpeedX));
                SpeedY = s.Serialize<short>(SpeedY, name: nameof(SpeedY));

                SpritesCount = s.Serialize<ushort>(SpritesCount, name: nameof(SpritesCount));

                CommandOffset = s.Serialize<short>(CommandOffset, name: nameof(CommandOffset));
                CommandsCount = s.Serialize<short>(CommandsCount, name: nameof(CommandsCount));

                Short_4A = s.Serialize<short>(Short_4A, name: nameof(Short_4A));
                FollowY = s.Serialize<short>(FollowY, name: nameof(FollowY));
                FollowX = s.Serialize<short>(FollowX, name: nameof(FollowX));

                if (settings.EngineVersion is
                    Ray1EngineVersion.PC_Kit or
                    Ray1EngineVersion.PC_Fan or
                    Ray1EngineVersion.PC_Edu or
                    Ray1EngineVersion.PS1_Edu)
                    EDU_ExtHitPoints = s.Serialize<uint>(EDU_ExtHitPoints, name: nameof(EDU_ExtHitPoints));

                Short_50 = s.Serialize<short>(Short_50, name: nameof(Short_50));
                Short_52 = s.Serialize<short>(Short_52, name: nameof(Short_52));
                RaymanDistance = s.Serialize<short>(RaymanDistance, name: nameof(RaymanDistance));
                IFramesTimer = s.Serialize<short>(IFramesTimer, name: nameof(IFramesTimer));

                TestBlockIndex = s.Serialize<short>(TestBlockIndex, name: nameof(TestBlockIndex));
                Scale = s.Serialize<short>(Scale, name: nameof(Scale));
                TypeZDC = s.SerializeObject<ZDCReference>(TypeZDC, name: nameof(TypeZDC));
                ActiveTimer = s.Serialize<short>(ActiveTimer, name: nameof(ActiveTimer));
                Type = s.Serialize<ObjType>(Type, name: nameof(Type));

                BlockTypes = s.SerializeArray<BlockType>(BlockTypes, 5, name: nameof(BlockTypes));
                s.SerializePadding(1, logIfNotNull: true);

                OffsetBX = s.Serialize<byte>(OffsetBX, name: nameof(OffsetBX));
                OffsetBY = s.Serialize<byte>(OffsetBY, name: nameof(OffsetBY));

                AnimationIndex = s.Serialize<byte>(AnimationIndex, name: nameof(AnimationIndex));
                AnimationFrame = s.Serialize<byte>(AnimationFrame, name: nameof(AnimationFrame));

                SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));
                MainEtat = s.Serialize<byte>(MainEtat, name: nameof(MainEtat));
                InitialSubEtat = s.Serialize<byte>(InitialSubEtat, name: nameof(InitialSubEtat));
                InitialMainEtat = s.Serialize<byte>(InitialMainEtat, name: nameof(InitialMainEtat));

                Command = s.Serialize<CommandType>(Command, name: nameof(Command));
                GravityValue1 = s.Serialize<byte>(GravityValue1, name: nameof(GravityValue1));
                GravityValue2 = s.Serialize<byte>(GravityValue2, name: nameof(GravityValue2));
                ChangeAnimationMode = s.Serialize<ChangeAnimationMode>(ChangeAnimationMode, name: nameof(ChangeAnimationMode));

                OffsetHY = s.Serialize<byte>(OffsetHY, name: nameof(OffsetHY));

                FollowSprite = s.Serialize<byte>(FollowSprite, name: nameof(FollowSprite));
                HitPoints = s.Serialize<byte>(HitPoints, name: nameof(HitPoints));
                InitialHitPoints = s.Serialize<byte>(InitialHitPoints, name: nameof(InitialHitPoints));
                InitFlag = s.Serialize<byte>(InitFlag, name: nameof(InitFlag));

                HitSprite = s.Serialize<byte>(HitSprite, name: nameof(HitSprite));
                DetectZone = s.Serialize<byte>(DetectZone, name: nameof(DetectZone));
                DetectZoneFlag = s.Serialize<byte>(DetectZoneFlag, name: nameof(DetectZoneFlag));
                CommandContextDepth = s.Serialize<byte>(CommandContextDepth, name: nameof(CommandContextDepth));
                Byte_7D = s.Serialize<byte>(Byte_7D, name: nameof(Byte_7D));
                DisplayPrio = s.Serialize<byte>(DisplayPrio, name: nameof(DisplayPrio));
                Timer = s.Serialize<byte>(Timer, name: nameof(Timer));

                AnimationsCount = s.Serialize<byte>(AnimationsCount, name: nameof(AnimationsCount));

                if (settings.EngineVersion is Ray1EngineVersion.GBA or Ray1EngineVersion.DSi)
                {
                    s.DoBits<byte>(b =>
                    {
                        UnknownFlag1 = b.SerializeBits<bool>(UnknownFlag1, 1, name: nameof(UnknownFlag1));
                        CommandTest = b.SerializeBits<bool>(CommandTest, 1, name: nameof(CommandTest));
                        IsAlive = b.SerializeBits<bool>(IsAlive, 1, name: nameof(IsAlive));
                        UnknownFlag2 = b.SerializeBits<bool>(UnknownFlag2, 1, name: nameof(UnknownFlag2));
                        IsLinked = b.SerializeBits<bool>(IsLinked, 1, name: nameof(IsLinked));
                        IsActive = b.SerializeBits<bool>(IsActive, 1, name: nameof(IsActive));
                        FlipX = b.SerializeBits<bool>(FlipX, 1, name: nameof(FlipX));
                        ReadCommands = b.SerializeBits<bool>(ReadCommands, 1, name: nameof(ReadCommands));
                    });
                    s.DoBits<byte>(b =>
                    {
                        FollowEnabled = b.SerializeBits<bool>(FollowEnabled, 1, name: nameof(FollowEnabled));
                        UnknownFlag3 = b.SerializeBits<bool>(UnknownFlag3, 1, name: nameof(UnknownFlag3));
                        // Padding?
                        UnknownFlags = b.SerializeBits<int>(UnknownFlags, 6, name: nameof(UnknownFlags));
                    });
                    // Probably padding
                    UnknownBytes = s.SerializeArray<byte>(UnknownBytes, 3, name: nameof(UnknownBytes));
                }
                else
                {
                    s.DoBits<byte>(b =>
                    {
                        UnknownFlag1 = b.SerializeBits<bool>(UnknownFlag1, 1, name: nameof(UnknownFlag1));
                        CommandTest = b.SerializeBits<bool>(CommandTest, 1, name: nameof(CommandTest));
                        IsAlive = b.SerializeBits<bool>(IsAlive, 1, name: nameof(IsAlive));
                        FlipX = b.SerializeBits<bool>(FlipX, 1, name: nameof(FlipX));
                        ReadCommands = b.SerializeBits<bool>(ReadCommands, 1, name: nameof(ReadCommands));
                        FollowEnabled = b.SerializeBits<bool>(FollowEnabled, 1, name: nameof(FollowEnabled));
                        UnknownFlag3 = b.SerializeBits<bool>(UnknownFlag3, 1, name: nameof(UnknownFlag3));
                        UnknownFlag4 = b.SerializeBits<bool>(UnknownFlag4, 1, name: nameof(UnknownFlag4));
                    });
                    s.DoBits<byte>(b =>
                    {
                        UnknownFlag5 = b.SerializeBits<bool>(UnknownFlag5, 1, name: nameof(UnknownFlag5));
                        b.SerializePadding(7, logIfNotNull: true);
                    });
                    s.SerializePadding(1, logIfNotNull: true);
                }
            }
            else if (settings.EngineBranch == Ray1EngineBranch.PS1)
            {
                SpritesPointer = s.SerializePointer(SpritesPointer, name: nameof(SpritesPointer));
                AnimationsPointer = s.SerializePointer(AnimationsPointer, name: nameof(AnimationsPointer));
                ImageBufferPointer = s.SerializePointer(ImageBufferPointer, allowInvalid: true, name: nameof(ImageBufferPointer));
                ETAPointer = s.SerializePointer(ETAPointer, name: nameof(ETAPointer));

                CommandsPointer = s.SerializePointer(CommandsPointer, name: nameof(CommandsPointer));

                if (settings.EngineVersion is Ray1EngineVersion.PS1_JPDemoVol3 or Ray1EngineVersion.PS1_JPDemoVol6)
                {
                    CommandContexts = s.SerializeObjectArray<CommandContext>(CommandContexts, 10, name: nameof(CommandContexts));

                    Id = s.Serialize<short>(Id, name: nameof(Id));
                    ScreenXPosition = s.Serialize<short>(ScreenXPosition, name: nameof(ScreenXPosition));
                    ScreenYPosition = s.Serialize<short>(ScreenYPosition, name: nameof(ScreenYPosition));
                    XPosition = s.Serialize<short>((short)XPosition, name: nameof(XPosition));
                    YPosition = s.Serialize<short>((short)YPosition, name: nameof(YPosition));
                }
                else
                {
                    LabelOffsetsPointer = s.SerializePointer(LabelOffsetsPointer, name: nameof(LabelOffsetsPointer));
                    CommandContexts = s.SerializeObjectArray<CommandContext>(CommandContexts, 1, name: nameof(CommandContexts));

                    XPosition = s.Serialize<short>((short)XPosition, name: nameof(XPosition));
                    YPosition = s.Serialize<short>((short)YPosition, name: nameof(YPosition));
                    Id = s.Serialize<short>(Id, name: nameof(Id));
                    ScreenXPosition = s.Serialize<short>(ScreenXPosition, name: nameof(ScreenXPosition));
                    ScreenYPosition = s.Serialize<short>(ScreenYPosition, name: nameof(ScreenYPosition));
                }

                Short_3A = s.Serialize<short>(Short_3A, name: nameof(Short_3A));
                InitialXPosition = s.Serialize<short>(InitialXPosition, name: nameof(InitialXPosition));
                InitialYPosition = s.Serialize<short>(InitialYPosition, name: nameof(InitialYPosition));

                if (settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol3)
                    FlipX = s.SerializeBoolean<short>(FlipX, name: nameof(FlipX));

                SpeedX = s.Serialize<short>(SpeedX, name: nameof(SpeedX));
                SpeedY = s.Serialize<short>(SpeedY, name: nameof(SpeedY));

                SpritesCount = s.Serialize<ushort>(SpritesCount, name: nameof(SpritesCount));

                CommandOffset = s.Serialize<short>(CommandOffset, name: nameof(CommandOffset));
                CommandsCount = s.Serialize<short>(CommandsCount, name: nameof(CommandsCount));

                Short_4A = s.Serialize<short>(Short_4A, name: nameof(Short_4A));
                FollowY = s.Serialize<short>(FollowY, name: nameof(FollowY));
                FollowX = s.Serialize<short>(FollowX, name: nameof(FollowX));

                Short_50 = s.Serialize<short>(Short_50, name: nameof(Short_50));
                Short_52 = s.Serialize<short>(Short_52, name: nameof(Short_52));
                RaymanDistance = s.Serialize<short>(RaymanDistance, name: nameof(RaymanDistance));
                IFramesTimer = s.Serialize<short>(IFramesTimer, name: nameof(IFramesTimer));

                if (settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol3)
                {
                    ReadCommands = s.SerializeBoolean<short>(ReadCommands, name: nameof(ReadCommands));
                    UnknownDemoValue = s.Serialize<short>(UnknownDemoValue, name: nameof(UnknownDemoValue));
                }
                else
                {
                    TestBlockIndex = s.Serialize<short>(TestBlockIndex, name: nameof(TestBlockIndex));
                    Scale = s.Serialize<short>(Scale, name: nameof(Scale));
                    TypeZDC = s.SerializeObject<ZDCReference>(TypeZDC, name: nameof(TypeZDC));
                    ActiveTimer = s.Serialize<short>(ActiveTimer, name: nameof(ActiveTimer));
                }

                BlockTypes = s.SerializeArray<BlockType>(BlockTypes, 5, name: nameof(BlockTypes));
                s.SerializePadding(1, logIfNotNull: true);

                OffsetBX = s.Serialize<byte>(OffsetBX, name: nameof(OffsetBX));
                OffsetBY = s.Serialize<byte>(OffsetBY, name: nameof(OffsetBY));

                AnimationIndex = s.Serialize<byte>(AnimationIndex, name: nameof(AnimationIndex));
                AnimationFrame = s.Serialize<byte>(AnimationFrame, name: nameof(AnimationFrame));

                MainEtat = s.Serialize<byte>(MainEtat, name: nameof(MainEtat));
                InitialMainEtat = s.Serialize<byte>(InitialMainEtat, name: nameof(InitialMainEtat));
                SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));
                InitialSubEtat = s.Serialize<byte>(InitialSubEtat, name: nameof(InitialSubEtat));

                Command = s.Serialize<CommandType>(Command, name: nameof(Command));
                GravityValue1 = s.Serialize<byte>(GravityValue1, name: nameof(GravityValue1));
                GravityValue2 = s.Serialize<byte>(GravityValue2, name: nameof(GravityValue2));
                ChangeAnimationMode = s.Serialize<ChangeAnimationMode>(ChangeAnimationMode, name: nameof(ChangeAnimationMode));

                OffsetHY = s.Serialize<byte>(OffsetHY, name: nameof(OffsetHY));

                if (settings.EngineVersion is Ray1EngineVersion.PS1_JPDemoVol3 or Ray1EngineVersion.PS1_JPDemoVol6)
                    FollowEnabled = s.Serialize<bool>(FollowEnabled, name: nameof(FollowEnabled));

                FollowSprite = s.Serialize<byte>(FollowSprite, name: nameof(FollowSprite));
                HitPoints = s.Serialize<byte>(HitPoints, name: nameof(HitPoints));
                InitialHitPoints = s.Serialize<byte>(InitialHitPoints, name: nameof(InitialHitPoints));
                InitFlag = s.Serialize<byte>(InitFlag, name: nameof(InitFlag));

                Type = (ObjType)s.Serialize<byte>((byte)Type, name: nameof(Type));
                HitSprite = s.Serialize<byte>(HitSprite, name: nameof(HitSprite));
                ActiveFlag = s.Serialize<ObjectActiveFlag>(ActiveFlag, name: nameof(ActiveFlag));
                DetectZone = s.Serialize<byte>(DetectZone, name: nameof(DetectZone));
                DetectZoneFlag = s.Serialize<byte>(DetectZoneFlag, name: nameof(DetectZoneFlag));
                CommandContextDepth = s.Serialize<byte>(CommandContextDepth, name: nameof(CommandContextDepth));
                Byte_7D = s.Serialize<byte>(Byte_7D, name: nameof(Byte_7D));

                if (settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol3)
                {
                    UnknownFlag1 = s.Serialize<bool>(UnknownFlag1, name: nameof(UnknownFlag1));
                    CommandTest = s.Serialize<bool>(CommandTest, name: nameof(CommandTest));
                    IsAlive = s.Serialize<bool>(IsAlive, name: nameof(IsAlive));
                    IsActive = s.Serialize<bool>(IsActive, name: nameof(IsActive));

                    DisplayPrio = s.Serialize<byte>(DisplayPrio, name: nameof(DisplayPrio));
                    Timer = s.Serialize<byte>(Timer, name: nameof(Timer));

                    AnimationsCount = s.Serialize<byte>(AnimationsCount, name: nameof(AnimationsCount));
                }
                else if (settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol6)
                {
                    UnknownFlag1 = s.Serialize<bool>(UnknownFlag1, name: nameof(UnknownFlag1));
                    DisplayPrio = s.Serialize<byte>(DisplayPrio, name: nameof(DisplayPrio));
                    Timer = s.Serialize<byte>(Timer, name: nameof(Timer));

                    AnimationsCount = s.Serialize<byte>(AnimationsCount, name: nameof(AnimationsCount));
                    s.SerializePadding(1, logIfNotNull: true);
                }
                else
                {
                    DisplayPrio = s.Serialize<byte>(DisplayPrio, name: nameof(DisplayPrio));
                    Timer = s.Serialize<byte>(Timer, name: nameof(Timer));
                    AnimationsCount = s.Serialize<byte>(AnimationsCount, name: nameof(AnimationsCount));

                    s.DoBits<byte>(b =>
                    {
                        if (settings.EngineVersion == Ray1EngineVersion.Saturn)
                        {
                            ReadCommands = b.SerializeBits<bool>(ReadCommands, 1, name: nameof(ReadCommands));
                            FlipX = b.SerializeBits<bool>(FlipX, 1, name: nameof(FlipX));
                            UnknownFlag2 = b.SerializeBits<bool>(UnknownFlag2, 1, name: nameof(UnknownFlag2));
                            IsLinked = b.SerializeBits<bool>(IsLinked, 1, name: nameof(IsLinked));
                            IsActive = b.SerializeBits<bool>(IsActive, 1, name: nameof(IsActive));
                            IsAlive = b.SerializeBits<bool>(IsAlive, 1, name: nameof(IsAlive));
                            CommandTest = b.SerializeBits<bool>(CommandTest, 1, name: nameof(CommandTest));
                            UnknownFlag1 = b.SerializeBits<bool>(UnknownFlag1, 1, name: nameof(UnknownFlag1));
                        }
                        else
                        {
                            UnknownFlag1 = b.SerializeBits<bool>(UnknownFlag1, 1, name: nameof(UnknownFlag1));
                            CommandTest = b.SerializeBits<bool>(CommandTest, 1, name: nameof(CommandTest));
                            IsAlive = b.SerializeBits<bool>(IsAlive, 1, name: nameof(IsAlive));
                            IsActive = b.SerializeBits<bool>(IsActive, 1, name: nameof(IsActive));
                            IsLinked = b.SerializeBits<bool>(IsLinked, 1, name: nameof(IsLinked));
                            UnknownFlag2 = b.SerializeBits<bool>(UnknownFlag2, 1, name: nameof(UnknownFlag2));
                            FlipX = b.SerializeBits<bool>(FlipX, 1, name: nameof(FlipX));
                            ReadCommands = b.SerializeBits<bool>(ReadCommands, 1, name: nameof(ReadCommands));
                        }
                    });
                    s.DoBits<byte>(b =>
                    {
                        if (settings.EngineVersion == Ray1EngineVersion.Saturn)
                        {
                            UnknownFlags = b.SerializeBits<int>(UnknownFlags, 4, name: nameof(UnknownFlags));
                            UnknownFlag5 = b.SerializeBits<bool>(UnknownFlag5, 1, name: nameof(UnknownFlag5));
                            UnknownFlag4 = b.SerializeBits<bool>(UnknownFlag4, 1, name: nameof(UnknownFlag4));
                            UnknownFlag3 = b.SerializeBits<bool>(UnknownFlag3, 1, name: nameof(UnknownFlag3));
                            FollowEnabled = b.SerializeBits<bool>(FollowEnabled, 1, name: nameof(FollowEnabled));
                        }
                        else
                        {
                            FollowEnabled = b.SerializeBits<bool>(FollowEnabled, 1, name: nameof(FollowEnabled));
                            UnknownFlag3 = b.SerializeBits<bool>(UnknownFlag3, 1, name: nameof(UnknownFlag3));
                            UnknownFlag4 = b.SerializeBits<bool>(UnknownFlag4, 1, name: nameof(UnknownFlag4));
                            UnknownFlag5 = b.SerializeBits<bool>(UnknownFlag5, 1, name: nameof(UnknownFlag5));

                            // Probably padding
                            UnknownFlags = b.SerializeBits<int>(UnknownFlags, 4, name: nameof(UnknownFlags));
                        }
                    });
                    // Probably padding
                    UnknownBytes = s.SerializeArray<byte>(UnknownBytes, 1, name: nameof(UnknownBytes));
                }
            }
            else
            {
                throw new BinarySerializableException(this, $"Unsupported engine branch {settings.EngineBranch}");
            }

            if (!settings.IsLoadingPackedPCData && s.FullSerialize)
                SerializeFromPointers(s);
        }

        #endregion

        #region Data Types

        [Flags]
        public enum RefDataFlags
        {
            None = 0,
            Sprites = 1 << 0,
            Animations = 1 << 1,
            ImageBuffer = 1 << 2,
            States = 1 << 3,
            Commands = 1 << 4,
            All = Sprites | Animations | ImageBuffer | States | Commands,
        }

        #endregion
    }
}