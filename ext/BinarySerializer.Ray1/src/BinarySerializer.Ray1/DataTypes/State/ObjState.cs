namespace BinarySerializer.Ray1
{
    /// <summary>
    /// Data for an object state
    /// </summary>
    public class ObjState : BinarySerializable
    {
        public short RightSpeed { get; set; }
        public short LeftSpeed { get; set; }

        public byte AnimationIndex { get; set; }

        // To be set when this state's animation has finished
        public byte NextAnimationIndex { get; set; }
        public byte NextMainEtat { get; set; }
        public byte NextSubEtat { get; set; }

        /// <summary>
        /// The amount of game frames to wait for each animation frame, or 0 for it to not animate
        /// </summary>
        public byte AnimationSpeed { get; set; }

        public short Gravity { get; set; } // SpeedY
        public byte GravityMode { get; set; } // The type of gravity calculation to perform

        public byte Sound { get; set; }

        // These flags might have different meanings for non-Rayman states
        public bool RayCanJump { get; set; }
        public bool RayCanPunch { get; set; }
        public bool RayCanHelico { get; set; }

        public bool FistCollision { get; set; }
        public bool ReverseAnimation { get; set; }
        public bool RayCollision { get; set; }
        
        public bool UnknownFlag1 { get; set; }
        public bool UnknownFlag2 { get; set; }

        public bool ObjInAir { get; set; } // Only in R2, for earlier games it's determined by checking if state is 2
        public bool ConstantGravity { get; set; } // True if gravity is constant and doesn't accelerate

        public override void SerializeImpl(SerializerObject s) 
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            if (settings.EngineVersion == Ray1EngineVersion.R2_PS1)
            {
                // Unsure about padding. Could be unused data, but always 0 anyway.
                RightSpeed = s.Serialize<short>(RightSpeed, name: nameof(RightSpeed));
                LeftSpeed = s.Serialize<short>(LeftSpeed, name: nameof(LeftSpeed));
                AnimationIndex = s.Serialize<byte>(AnimationIndex, name: nameof(AnimationIndex));
                s.SerializePadding(1, logIfNotNull: true);
                Gravity = s.Serialize<short>(Gravity, name: nameof(Gravity));
                s.SerializePadding(2, logIfNotNull: true);
                NextMainEtat = s.Serialize<byte>(NextMainEtat, name: nameof(NextMainEtat));
                NextSubEtat = s.Serialize<byte>(NextSubEtat, name: nameof(NextSubEtat));

                s.DoBits<ushort>(b =>
                {
                    RayCanJump = b.SerializeBits<bool>(RayCanJump, 1, name: nameof(RayCanJump));
                    RayCanPunch = b.SerializeBits<bool>(RayCanPunch, 1, name: nameof(RayCanPunch));
                    RayCanHelico = b.SerializeBits<bool>(RayCanHelico, 1, name: nameof(RayCanHelico));
                    FistCollision = b.SerializeBits<bool>(FistCollision, 1, name: nameof(FistCollision));
                    ReverseAnimation = b.SerializeBits<bool>(ReverseAnimation, 1, name: nameof(ReverseAnimation));
                    RayCollision = b.SerializeBits<bool>(RayCollision, 1, name: nameof(RayCollision));
                    UnknownFlag1 = b.SerializeBits<bool>(UnknownFlag1, 1, name: nameof(UnknownFlag1));
                    UnknownFlag2 = b.SerializeBits<bool>(UnknownFlag2, 1, name: nameof(UnknownFlag2));
                    
                    AnimationSpeed = b.SerializeBits<byte>(AnimationSpeed, 3, name: nameof(AnimationSpeed));
                    ObjInAir = b.SerializeBits<bool>(ObjInAir, 1, name: nameof(ObjInAir));
                    ConstantGravity = b.SerializeBits<bool>(ConstantGravity, 1, name: nameof(ConstantGravity));
                    b.SerializePadding(3, logIfNotNull: true);
                });
                
                s.SerializePadding(2, logIfNotNull: true);
            }
            else
            {
                s.DoWithDefaults(new SerializerDefaults()
                {
                    // When a state is NULL then it contains leftover garbage data
                    DisableFormattingWarnings = true
                }, () =>
                {
                    if (settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol3)
                    {
                        RightSpeed = s.Serialize<short>(RightSpeed, name: nameof(RightSpeed));
                        LeftSpeed = s.Serialize<short>(LeftSpeed, name: nameof(LeftSpeed));

                        AnimationIndex = s.Serialize<byte>(AnimationIndex, name: nameof(AnimationIndex));
                        NextAnimationIndex = s.Serialize<byte>(NextAnimationIndex, name: nameof(NextAnimationIndex));
                        NextMainEtat = s.Serialize<byte>(NextMainEtat, name: nameof(NextMainEtat));
                        NextSubEtat = s.Serialize<byte>(NextSubEtat, name: nameof(NextSubEtat));

                        ReverseAnimation = s.Serialize<bool>(ReverseAnimation, name: nameof(ReverseAnimation));
                        AnimationSpeed = s.Serialize<byte>(AnimationSpeed, name: nameof(AnimationSpeed));
                        GravityMode = s.Serialize<byte>(GravityMode, name: nameof(GravityMode));
                        RayCanJump = s.Serialize<bool>(RayCanJump, name: nameof(RayCanJump));
                        RayCanPunch = s.Serialize<bool>(RayCanPunch, name: nameof(RayCanPunch));
                        Sound = s.Serialize<byte>(Sound, name: nameof(Sound));
                    }
                    else if (settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol6)
                    {
                        RightSpeed = s.Serialize<short>(RightSpeed, name: nameof(RightSpeed));
                        LeftSpeed = s.Serialize<short>(LeftSpeed, name: nameof(LeftSpeed));
                        
                        AnimationIndex = s.Serialize<byte>(AnimationIndex, name: nameof(AnimationIndex));
                        NextAnimationIndex = s.Serialize<byte>(NextAnimationIndex, name: nameof(NextAnimationIndex));
                        NextMainEtat = s.Serialize<byte>(NextMainEtat, name: nameof(NextMainEtat));
                        NextSubEtat = s.Serialize<byte>(NextSubEtat, name: nameof(NextSubEtat));

                        AnimationSpeed = s.Serialize<byte>(AnimationSpeed, name: nameof(AnimationSpeed));
                        GravityMode = s.Serialize<byte>(GravityMode, name: nameof(GravityMode));
                        s.DoBits<byte>(b =>
                        {
                            RayCanJump = b.SerializeBits<bool>(RayCanJump, 1, name: nameof(RayCanJump));
                            RayCanPunch = b.SerializeBits<bool>(RayCanPunch, 1, name: nameof(RayCanPunch));
                            RayCanHelico = b.SerializeBits<bool>(RayCanHelico, 1, name: nameof(RayCanHelico));
                            FistCollision = b.SerializeBits<bool>(FistCollision, 1, name: nameof(FistCollision));
                            ReverseAnimation = b.SerializeBits<bool>(ReverseAnimation, 1, name: nameof(ReverseAnimation));
                            RayCollision = b.SerializeBits<bool>(RayCollision, 1, name: nameof(RayCollision));
                            UnknownFlag1 = b.SerializeBits<bool>(UnknownFlag1, 1, name: nameof(UnknownFlag1));
                            UnknownFlag2 = b.SerializeBits<bool>(UnknownFlag2, 1, name: nameof(UnknownFlag2));
                        });
                        Sound = s.Serialize<byte>(Sound, name: nameof(Sound));
                    }
                    else
                    {
                        RightSpeed = s.Serialize<sbyte>((sbyte)RightSpeed, name: nameof(RightSpeed));
                        LeftSpeed = s.Serialize<sbyte>((sbyte)LeftSpeed, name: nameof(LeftSpeed));
                        
                        AnimationIndex = s.Serialize<byte>(AnimationIndex, name: nameof(AnimationIndex));
                        NextMainEtat = s.Serialize<byte>(NextMainEtat, name: nameof(NextMainEtat));
                        NextSubEtat = s.Serialize<byte>(NextSubEtat, name: nameof(NextSubEtat));

                        s.DoBits<byte>(b =>
                        {
                            if (settings.EngineVersion == Ray1EngineVersion.Saturn)
                            {
                                GravityMode = b.SerializeBits<byte>(GravityMode, 4, name: nameof(GravityMode));
                                AnimationSpeed = b.SerializeBits<byte>(AnimationSpeed, 4, name: nameof(AnimationSpeed));
                            }
                            else
                            {
                                AnimationSpeed = b.SerializeBits<byte>(AnimationSpeed, 4, name: nameof(AnimationSpeed));
                                GravityMode = b.SerializeBits<byte>(GravityMode, 4, name: nameof(GravityMode));
                            }
                        });

                        Sound = s.Serialize<byte>(Sound, name: nameof(Sound));

                        s.DoBits<byte>(b =>
                        {
                            if (settings.EngineVersion == Ray1EngineVersion.Saturn)
                            {
                                UnknownFlag2 = b.SerializeBits<bool>(UnknownFlag2, 1, name: nameof(UnknownFlag2));
                                UnknownFlag1 = b.SerializeBits<bool>(UnknownFlag1, 1, name: nameof(UnknownFlag1));
                                RayCollision = b.SerializeBits<bool>(RayCollision, 1, name: nameof(RayCollision));
                                ReverseAnimation = b.SerializeBits<bool>(ReverseAnimation, 1, name: nameof(ReverseAnimation));
                                FistCollision = b.SerializeBits<bool>(FistCollision, 1, name: nameof(FistCollision));
                                RayCanHelico = b.SerializeBits<bool>(RayCanHelico, 1, name: nameof(RayCanHelico));
                                RayCanPunch = b.SerializeBits<bool>(RayCanPunch, 1, name: nameof(RayCanPunch));
                                RayCanJump = b.SerializeBits<bool>(RayCanJump, 1, name: nameof(RayCanJump));
                            }
                            else
                            {
                                RayCanJump = b.SerializeBits<bool>(RayCanJump, 1, name: nameof(RayCanJump));
                                RayCanPunch = b.SerializeBits<bool>(RayCanPunch, 1, name: nameof(RayCanPunch));
                                RayCanHelico = b.SerializeBits<bool>(RayCanHelico, 1, name: nameof(RayCanHelico));
                                FistCollision = b.SerializeBits<bool>(FistCollision, 1, name: nameof(FistCollision));
                                ReverseAnimation = b.SerializeBits<bool>(ReverseAnimation, 1, name: nameof(ReverseAnimation));
                                RayCollision = b.SerializeBits<bool>(RayCollision, 1, name: nameof(RayCollision));
                                UnknownFlag1 = b.SerializeBits<bool>(UnknownFlag1, 1, name: nameof(UnknownFlag1));
                                UnknownFlag2 = b.SerializeBits<bool>(UnknownFlag2, 1, name: nameof(UnknownFlag2));
                            }
                        });
                    }
                });
            }
        }
    }
}