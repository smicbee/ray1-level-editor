using System;
using BinarySerializer.Ray1.PS1;

namespace BinarySerializer.Ray1
{
    /// <summary>
    /// Object data
    /// </summary>
    public class R2_ObjData : BinarySerializable 
    {
        #region Options

        // Sometimes we don't want to serialize all referenced data, so this can be used to
        // specify which data we want. Ideally this should be replaced with a more common
        // system in BinarySerializer for determining which data to serialize.
        public RefDataFlags Pre_SerializeRefDataFlags { get; set; } = RefDataFlags.All;

        #endregion

        #region Object data

        public short Rotation { get; set; }
        public short Short_02 { get; set; }
        public short Short_04 { get; set; }
        public short Short_06 { get; set; } // Appears unreferenced
        public byte RotationCollisionIndex { get; set; }
        public bool HasRotationCollision { get; set; }
        public byte Byte_0A { get; set; } // Appears unreferenced
        public byte RotationMode { get; set; } // 0 = No rotation, 1 = rotation. Checked as a bool, but set to 2 in one place...

        public Pointer UserDataPointer { get; set; }
        public Pointer CharacterPointer { get; set; }
        public Pointer AnimSetPointer { get; set; }
        public Pointer HandlersPointer { get; set; }
        
        public short InitialXPosition { get; set; }
        public short InitialYPosition { get; set; }
        public byte InitialMainEtat { get; set; }
        public byte InitialSubEtat { get; set; }
        public byte InitialHitPoints { get; set; }
        public byte InitialDisplayPrio { get; set; }
        public ObjMapLayer InitialMapLayer { get; set; }
        public byte InitialScale { get; set; }
        public bool InitialFlipX { get; set; }
        public byte InitialGendoorExtGroup { get; set; }

        public FixedPointInt32 FixedPointXPosition { get; set; }
        public FixedPointInt32 FixedPointYPosition { get; set; }

        public int TestBlockIndex { get; set; }
        public int RaymanDistance { get; set; }

        public Pointer StatePointer { get; set; }
        public Pointer AnimationPointer { get; set; }

        public short Id { get; set; }
        public R2_ObjType Type { get; set; }

        public short XPosition { get; set; }
        public short YPosition { get; set; }

        public short ScreenXPosition { get; set; }
        public short ScreenYPosition { get; set; }

        public short SpeedX { get; set; }
        public short SpeedY { get; set; }

        public short ActiveTimer { get; set; }
        public byte Scale { get; set; }

        public byte AnimationIndex { get; set; }
        public byte AnimationFrame { get; set; }

        public byte MainEtat { get; set; }
        public byte SubEtat { get; set; }
        
        public byte HitPoints { get; set; }
        public byte ActiveFlag { get; set; }
        public byte DisplayPrio { get; set; }

        public R2_BlockType BlockType { get; set; }

        public byte Byte_5B { get; set; } // Appears unreferenced

        public byte EventIndex { get; set; }
        public byte[] EventTimers { get; set; } // Unsure about length

        public ObjMapLayer MapLayer { get; set; }
        public byte BlockCheckCounter { get; set; }
        public byte BlockCheck { get; set; }

        public bool NewState { get; set; }
        public bool UnknownFlag1 { get; set; }
        public bool IsAlive { get; set; }
        public bool IsLinked { get; set; }
        public bool IsActive { get; set; }
        public bool UnknownFlag2 { get; set; }
        public bool HasHandledDeath { get; set; }
        public bool UnknownFlag3 { get; set; } // Same as UnknownFlag2 from R1

        public bool FlipX { get; set; }
        public bool FollowEnabled { get; set; }
        public byte GendoorExtGroup { get; set; } // Might have other usages for other types?
        public bool SemiTransparent { get; set; }

        #endregion

        #region Parsed from pointers

        public Character Character { get; set; }
        public AnimationSet AnimSet { get; set; }

        public byte[] UserDataBuffer { get; set; }
        public GendoorUserData UserData_Gendoor { get; set; }
        public TriggerUserData UserData_Trigger { get; set; }

        #endregion

        #region Object Creation Methods

        /// <summary>
        /// Gets a new object instance for Rayman
        /// </summary>
        public static R2_ObjData GetRayman(R2_ObjData rayPos, R2_AllfixData data) => new R2_ObjData()
        {
            // Gets loaded at 0x80178DF0 during runtime
            UserDataPointer = data.RaymanUserDataPointer,
            CharacterPointer = data.RaymanCharacterPointer,
            AnimSetPointer = data.RaymanAnimSetPointer,
            XPosition = (short)(rayPos != null ? (rayPos.XPosition + rayPos.Character.OffsetBX - data.RaymanCollisionData.OffsetBX) : 100),
            YPosition = (short)(rayPos != null ? (rayPos.YPosition + rayPos.Character.OffsetBY - data.RaymanCollisionData.OffsetBY) : 10),
            MainEtat = 0, // It's supposed to be Etat 2, SubEtat 2, but the ray pos state has a better looking speed
            SubEtat = 19,
            InitialMapLayer = ObjMapLayer.Front,
            Type = R2_ObjType.TYPE_RAYMAN,
            DisplayPrio = 7,
            EventTimers = new byte[7],
            Character = data.RaymanCollisionData,
            AnimSet = data.RaymanAnimSet
        };

        #endregion

        #region Public Methods

        public override void SerializeImpl(SerializerObject s)
        {
            Rotation = s.Serialize<short>(Rotation, name: nameof(Rotation));
            Short_02 = s.Serialize<short>(Short_02, name: nameof(Short_02));
            Short_04 = s.Serialize<short>(Short_04, name: nameof(Short_04));
            Short_06 = s.Serialize<short>(Short_06, name: nameof(Short_06));
            RotationCollisionIndex = s.Serialize<byte>(RotationCollisionIndex, name: nameof(RotationCollisionIndex));
            HasRotationCollision = s.Serialize<bool>(HasRotationCollision, name: nameof(HasRotationCollision));
            Byte_0A = s.Serialize<byte>(Byte_0A, name: nameof(Byte_0A));
            RotationMode = s.Serialize<byte>(RotationMode, name: nameof(RotationMode));

            UserDataPointer = s.SerializePointer(UserDataPointer, name: nameof(UserDataPointer));
            CharacterPointer = s.SerializePointer(CharacterPointer, name: nameof(CharacterPointer));
            AnimSetPointer = s.SerializePointer(AnimSetPointer, name: nameof(AnimSetPointer));
            HandlersPointer = s.SerializePointer(HandlersPointer, name: nameof(HandlersPointer));

            InitialXPosition = s.Serialize<short>(InitialXPosition, name: nameof(InitialXPosition));
            InitialYPosition = s.Serialize<short>(InitialYPosition, name: nameof(InitialYPosition));
            InitialMainEtat = s.Serialize<byte>(InitialMainEtat, name: nameof(InitialMainEtat));
            InitialSubEtat = s.Serialize<byte>(InitialSubEtat, name: nameof(InitialSubEtat));
            InitialHitPoints = s.Serialize<byte>(InitialHitPoints, name: nameof(InitialHitPoints));
            InitialDisplayPrio = s.Serialize<byte>(InitialDisplayPrio, name: nameof(InitialDisplayPrio));
            InitialMapLayer = s.Serialize<ObjMapLayer>(InitialMapLayer, name: nameof(InitialMapLayer));
            InitialScale = s.Serialize<byte>(InitialScale, name: nameof(InitialScale));
            s.DoBits<ushort>(b =>
            {
                InitialFlipX = b.SerializeBits<bool>(InitialFlipX, 1, name: nameof(InitialFlipX));
                InitialGendoorExtGroup = b.SerializeBits<byte>(InitialGendoorExtGroup, 6, name: nameof(InitialGendoorExtGroup));
                b.SerializePadding(1, logIfNotNull: true);
            });

            FixedPointXPosition = s.SerializeObject<FixedPointInt32>(FixedPointXPosition, x => x.Pre_PointPosition = 8, name: nameof(FixedPointXPosition));
            FixedPointYPosition = s.SerializeObject<FixedPointInt32>(FixedPointYPosition, x => x.Pre_PointPosition = 8, name: nameof(FixedPointYPosition));

            TestBlockIndex = s.Serialize<int>(TestBlockIndex, name: nameof(TestBlockIndex));
            RaymanDistance = s.Serialize<int>(RaymanDistance, name: nameof(RaymanDistance));

            StatePointer = s.SerializePointer(StatePointer, name: nameof(StatePointer));
            AnimationPointer = s.SerializePointer(AnimationPointer, name: nameof(AnimationPointer));

            Id = s.Serialize<short>(Id, name: nameof(Id));
            Type = s.Serialize<R2_ObjType>(Type, name: nameof(Type));

            XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));

            ScreenXPosition = s.Serialize<short>(ScreenXPosition, name: nameof(ScreenXPosition));
            ScreenYPosition = s.Serialize<short>(ScreenYPosition, name: nameof(ScreenYPosition));

            SpeedX = s.Serialize<short>(SpeedX, name: nameof(SpeedX));
            SpeedY = s.Serialize<short>(SpeedY, name: nameof(SpeedY));
            
            ActiveTimer = s.Serialize<short>(ActiveTimer, name: nameof(ActiveTimer));
            Scale = s.Serialize<byte>(Scale, name: nameof(Scale));

            AnimationIndex = s.Serialize<byte>(AnimationIndex, name: nameof(AnimationIndex));
            AnimationFrame = s.Serialize<byte>(AnimationFrame, name: nameof(AnimationFrame));

            MainEtat = s.Serialize<byte>(MainEtat, name: nameof(MainEtat));
            SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));

            HitPoints = s.Serialize<byte>(HitPoints, name: nameof(HitPoints));
            ActiveFlag = s.Serialize<byte>(ActiveFlag, name: nameof(ActiveFlag));
            DisplayPrio = s.Serialize<byte>(DisplayPrio, name: nameof(DisplayPrio));
            BlockType = s.Serialize<R2_BlockType>(BlockType, name: nameof(BlockType));

            Byte_5B = s.Serialize<byte>(Byte_5B, name: nameof(Byte_5B));

            EventIndex = s.Serialize<byte>(EventIndex, name: nameof(EventIndex));
            EventTimers = s.SerializeArray<byte>(EventTimers, 7, name: nameof(EventTimers));

            MapLayer = s.Serialize<ObjMapLayer>(MapLayer, name: nameof(MapLayer));
            BlockCheckCounter = s.Serialize<byte>(BlockCheckCounter, name: nameof(BlockCheckCounter));
            BlockCheck = s.Serialize<byte>(BlockCheck, name: nameof(BlockCheck));

            s.DoBits<byte>(b =>
            {
                NewState = b.SerializeBits<bool>(NewState, 1, name: nameof(NewState));
                UnknownFlag1 = b.SerializeBits<bool>(UnknownFlag1, 1, name: nameof(UnknownFlag1));
                IsAlive = b.SerializeBits<bool>(IsAlive, 1, name: nameof(IsAlive));
                IsLinked = b.SerializeBits<bool>(IsLinked, 1, name: nameof(IsLinked));
                IsActive = b.SerializeBits<bool>(IsActive, 1, name: nameof(IsActive));
                UnknownFlag2 = b.SerializeBits<bool>(UnknownFlag2, 1, name: nameof(UnknownFlag2));
                HasHandledDeath = b.SerializeBits<bool>(HasHandledDeath, 1, name: nameof(HasHandledDeath));
                UnknownFlag3 = b.SerializeBits<bool>(UnknownFlag3, 1, name: nameof(UnknownFlag3));
            });

            s.DoBits<int>(b =>
            {
                FlipX = b.SerializeBits<bool>(FlipX, 1, name: nameof(FlipX));
                FollowEnabled = b.SerializeBits<bool>(FollowEnabled, 1, name: nameof(FollowEnabled));
                GendoorExtGroup = b.SerializeBits<byte>(GendoorExtGroup, 6, name: nameof(GendoorExtGroup));
                SemiTransparent = b.SerializeBits<bool>(SemiTransparent, 1, name: nameof(SemiTransparent));

                // Seems unreferenced, so probably padding
                b.SerializePadding(23, logIfNotNull: true);
            });

            // Parse data from pointers
            if ((Pre_SerializeRefDataFlags & RefDataFlags.Character) != 0)
                s.DoAt(CharacterPointer, () => Character = s.SerializeObject<Character>(Character, name: nameof(Character)));
            
            if ((Pre_SerializeRefDataFlags & RefDataFlags.UserData) != 0)
                s.DoAt(UserDataPointer, () =>
                {
                    switch (Type)
                    {
                        case R2_ObjType.TYPE_GENERATING_DOOR:
                        case R2_ObjType.TYPE_DESTROYING_DOOR:
                            UserData_Gendoor = s.SerializeObject<GendoorUserData>(UserData_Gendoor, name: nameof(UserData_Gendoor));
                            break;
                        
                        case R2_ObjType.TYPE_TRIGGER:
                            UserData_Trigger = s.SerializeObject<TriggerUserData>(UserData_Trigger, name: nameof(UserData_Trigger));
                            break;

                        // TODO: Parse remaining user data
                        
                        default:
                            UserDataBuffer = s.SerializeArray<byte>(UserDataBuffer, Type.GetUserDataLength(), name: nameof(UserDataBuffer));
                            break;
                    }
                });

            if (!s.FullSerialize)
                return;

            if ((Pre_SerializeRefDataFlags & RefDataFlags.AnimSet) != 0)
                s.DoAt(AnimSetPointer, () => AnimSet = s.SerializeObject<AnimationSet>(AnimSet, name: nameof(AnimSet)));
        }

        #endregion

        #region Data Types

        [Flags]
        public enum RefDataFlags
        {
            None = 0,
            UserData = 1 << 0,
            Character = 1 << 1,
            AnimSet = 1 << 2,
            All = UserData | Character | AnimSet,
        }

        public enum ObjMapLayer : byte
        {
            None = 0,
            Front = 1,
            Back = 2,
        }

        #endregion
    }
}