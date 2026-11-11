namespace BinarySerializer.Ray1
{
    public class ObjTypeFlags : BinarySerializable
    {
        public bool Always { get; set; } // If true the game sets the pos to (-32000, -32000) on init
        public bool Balle { get; set; } // Indicates if the object is TYPE_BALLE1 or TYPE_BALLE2
        public bool NoCollision { get; set; } // Indicates if the object has no collision - does not include follow
        public bool HitRay { get; set; } // Indicates if the object damages Rayman
        public bool KeepActive { get; set; }
        public bool DetectZone { get; set; } // Indicates if the detect zone should be set
        public bool Flag_06 { get; set; }
        public bool Boss { get; set; } // Indicates if the boss bar should show

        public bool KeepLinkedObjectsActive { get; set; }
        public bool Bonus { get; set; } // Indicates if the object can be collected and thus not respawn again
        public bool BigRayHitKnockback { get; set; }
        public bool RayDistMultisprCantchange { get; set; }
        public bool InstantSpeedX { get; set; } // Indicates if the object x position should be changed by SpeedX in MOVE_OBJECT
        public bool InstantSpeedY { get; set; } // Indicates if the object y position should be changed by SpeedY in MOVE_OBJECT
        public bool SpecialPlatform { get; set; } // Indicates if DO_SPECIAL_PLATFORM should be called
        public bool ReadCmds { get; set; } // Indicates if commands should be read for the object, otherwise the command is set to 30 (NOP)

        public bool MoveOnBlock { get; set; } // Indicates if the object reacts to block types (tile collision), thus calling calc_btyp
        public bool FallInWater { get; set; }
        public bool BlocksRay { get; set; }
        public bool JumpOnBlock { get; set; } // Indicates if obj_jump gets called when on a ressort (spring) block
        public bool NoRayCollision { get; set; }
        public bool KillIfOutsideActiveZone { get; set; }
        public bool UturnOnBlock { get; set; }
        public bool IncreaseSpeedX { get; set; }

        public bool PoingCollisionSound { get; set; }
        public bool DieInWater { get; set; }
        public bool StopMovingUpWhenHitBlock { get; set; }
        public bool SwitchOff { get; set; }
        public bool Flag_1C { get; set; }
        public bool LinkRequiresGendoor { get; set; } // Indicates if the object requires a gendoor in the link group to be valid
        public bool NoLink { get; set; } // Indicates that the object can't be linked
        public bool Flag_1F { get; set; }

        // Rayman 2
        public bool R2_Flag_03 { get; set; }
        public bool R2_Flag_04 { get; set; }
        public bool R2_Flag_08 { get; set; } // Collision related
        public bool R2_Flag_0E { get; set; }
        public bool R2_Flag_11 { get; set; }
        public bool RandomAnimFrame { get; set; }
        public bool R2_Flag_13 { get; set; } // Prevents scaling and effects ZDC
        public bool R2_Flag_14 { get; set; } // Prevents scaling and effects ZDC
        public bool R2_Flag_15 { get; set; }
        public bool R2_Flag_16 { get; set; } // Determines how hit knockback is calculated
        public bool DoNotDoObject { get; set; }
        public bool R2_Flag_18 { get; set; }
        public bool R2_Flag_19 { get; set; }
        public bool R2_Flag_1A { get; set; }
        public bool R2_Flag_1B { get; set; }
        public bool R2_Flag_1C { get; set; }
        public bool R2_Flag_1D { get; set; }
        public bool R2_Flag_1E { get; set; }
        public bool R2_Flag_1F { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            if (settings.EngineVersion == Ray1EngineVersion.R2_PS1)
            {
                s.DoBits<ushort>(b =>
                {
                    HitRay = b.SerializeBits<bool>(HitRay, 1, name: nameof(HitRay));
                    KeepActive = b.SerializeBits<bool>(KeepActive, 1, name: nameof(KeepActive));
                    DetectZone = b.SerializeBits<bool>(DetectZone, 1, name: nameof(DetectZone));
                    R2_Flag_03 = b.SerializeBits<bool>(R2_Flag_03, 1, name: nameof(R2_Flag_03));
                    R2_Flag_04 = b.SerializeBits<bool>(R2_Flag_04, 1, name: nameof(R2_Flag_04));
                    BigRayHitKnockback = b.SerializeBits<bool>(BigRayHitKnockback, 1, name: nameof(BigRayHitKnockback));
                    MoveOnBlock = b.SerializeBits<bool>(MoveOnBlock, 1, name: nameof(MoveOnBlock));
                    FallInWater = b.SerializeBits<bool>(FallInWater, 1, name: nameof(FallInWater));

                    R2_Flag_08 = b.SerializeBits<bool>(R2_Flag_08, 1, name: nameof(R2_Flag_08));
                    JumpOnBlock = b.SerializeBits<bool>(JumpOnBlock, 1, name: nameof(JumpOnBlock));
                    NoCollision = b.SerializeBits<bool>(NoCollision, 1, name: nameof(NoCollision));
                    UturnOnBlock = b.SerializeBits<bool>(UturnOnBlock, 1, name: nameof(UturnOnBlock));
                    DieInWater = b.SerializeBits<bool>(DieInWater, 1, name: nameof(DieInWater));
                    StopMovingUpWhenHitBlock = b.SerializeBits<bool>(StopMovingUpWhenHitBlock, 1, name: nameof(StopMovingUpWhenHitBlock));
                    R2_Flag_0E = b.SerializeBits<bool>(R2_Flag_0E, 1, name: nameof(R2_Flag_0E));
                    LinkRequiresGendoor = b.SerializeBits<bool>(LinkRequiresGendoor, 1, name: nameof(LinkRequiresGendoor));
                });
                s.DoBits<ushort>(b =>
                {
                    NoLink = b.SerializeBits<bool>(NoLink, 1, name: nameof(NoLink));
                    R2_Flag_11 = b.SerializeBits<bool>(R2_Flag_11, 1, name: nameof(R2_Flag_11));
                    RandomAnimFrame = b.SerializeBits<bool>(RandomAnimFrame, 1, name: nameof(RandomAnimFrame));
                    R2_Flag_13 = b.SerializeBits<bool>(R2_Flag_13, 1, name: nameof(R2_Flag_13));
                    R2_Flag_14 = b.SerializeBits<bool>(R2_Flag_14, 1, name: nameof(R2_Flag_14));
                    R2_Flag_15 = b.SerializeBits<bool>(R2_Flag_15, 1, name: nameof(R2_Flag_15));
                    R2_Flag_16 = b.SerializeBits<bool>(R2_Flag_16, 1, name: nameof(R2_Flag_16));
                    DoNotDoObject = b.SerializeBits<bool>(DoNotDoObject, 1, name: nameof(DoNotDoObject));

                    // Might just be padding here - they all appear unreferenced
                    R2_Flag_18 = b.SerializeBits<bool>(R2_Flag_18, 1, name: nameof(R2_Flag_18));
                    R2_Flag_19 = b.SerializeBits<bool>(R2_Flag_19, 1, name: nameof(R2_Flag_19));
                    R2_Flag_1A = b.SerializeBits<bool>(R2_Flag_1A, 1, name: nameof(R2_Flag_1A));
                    R2_Flag_1B = b.SerializeBits<bool>(R2_Flag_1B, 1, name: nameof(R2_Flag_1B));
                    R2_Flag_1C = b.SerializeBits<bool>(R2_Flag_1C, 1, name: nameof(R2_Flag_1C));
                    R2_Flag_1D = b.SerializeBits<bool>(R2_Flag_1D, 1, name: nameof(R2_Flag_1D));
                    R2_Flag_1E = b.SerializeBits<bool>(R2_Flag_1E, 1, name: nameof(R2_Flag_1E));
                    R2_Flag_1F = b.SerializeBits<bool>(R2_Flag_1F, 1, name: nameof(R2_Flag_1F));
                });
            }
            else
            {
                // TODO: Add support for demos. They use fewer bytes. PS1 version also doesn't use
                //       some of the last few flags as they weren't added until the PC version.

                s.DoBits<int>(b =>
                {
                    if (settings.EngineVersion == Ray1EngineVersion.Saturn)
                    {
                        Flag_1F = b.SerializeBits<bool>(Flag_1F, 1, name: nameof(Flag_1F));
                        NoLink = b.SerializeBits<bool>(NoLink, 1, name: nameof(NoLink));
                        LinkRequiresGendoor = b.SerializeBits<bool>(LinkRequiresGendoor, 1, name: nameof(LinkRequiresGendoor));
                        Flag_1C = b.SerializeBits<bool>(Flag_1C, 1, name: nameof(Flag_1C));
                        SwitchOff = b.SerializeBits<bool>(SwitchOff, 1, name: nameof(SwitchOff));
                        StopMovingUpWhenHitBlock = b.SerializeBits<bool>(StopMovingUpWhenHitBlock, 1, name: nameof(StopMovingUpWhenHitBlock));
                        DieInWater = b.SerializeBits<bool>(DieInWater, 1, name: nameof(DieInWater));
                        PoingCollisionSound = b.SerializeBits<bool>(PoingCollisionSound, 1, name: nameof(PoingCollisionSound));

                        IncreaseSpeedX = b.SerializeBits<bool>(IncreaseSpeedX, 1, name: nameof(IncreaseSpeedX));
                        UturnOnBlock = b.SerializeBits<bool>(UturnOnBlock, 1, name: nameof(UturnOnBlock));
                        KillIfOutsideActiveZone = b.SerializeBits<bool>(KillIfOutsideActiveZone, 1, name: nameof(KillIfOutsideActiveZone));
                        NoRayCollision = b.SerializeBits<bool>(NoRayCollision, 1, name: nameof(NoRayCollision));
                        JumpOnBlock = b.SerializeBits<bool>(JumpOnBlock, 1, name: nameof(JumpOnBlock));
                        BlocksRay = b.SerializeBits<bool>(BlocksRay, 1, name: nameof(BlocksRay));
                        FallInWater = b.SerializeBits<bool>(FallInWater, 1, name: nameof(FallInWater));
                        MoveOnBlock = b.SerializeBits<bool>(MoveOnBlock, 1, name: nameof(MoveOnBlock));

                        ReadCmds = b.SerializeBits<bool>(ReadCmds, 1, name: nameof(ReadCmds));
                        SpecialPlatform = b.SerializeBits<bool>(SpecialPlatform, 1, name: nameof(SpecialPlatform));
                        InstantSpeedY = b.SerializeBits<bool>(InstantSpeedY, 1, name: nameof(InstantSpeedY));
                        InstantSpeedX = b.SerializeBits<bool>(InstantSpeedX, 1, name: nameof(InstantSpeedX));
                        RayDistMultisprCantchange = b.SerializeBits<bool>(RayDistMultisprCantchange, 1, name: nameof(RayDistMultisprCantchange));
                        BigRayHitKnockback = b.SerializeBits<bool>(BigRayHitKnockback, 1, name: nameof(BigRayHitKnockback));
                        Bonus = b.SerializeBits<bool>(Bonus, 1, name: nameof(Bonus));
                        KeepLinkedObjectsActive = b.SerializeBits<bool>(KeepLinkedObjectsActive, 1, name: nameof(KeepLinkedObjectsActive));

                        Boss = b.SerializeBits<bool>(Boss, 1, name: nameof(Boss));
                        Flag_06 = b.SerializeBits<bool>(Flag_06, 1, name: nameof(Flag_06));
                        DetectZone = b.SerializeBits<bool>(DetectZone, 1, name: nameof(DetectZone));
                        KeepActive = b.SerializeBits<bool>(KeepActive, 1, name: nameof(KeepActive));
                        HitRay = b.SerializeBits<bool>(HitRay, 1, name: nameof(HitRay));
                        NoCollision = b.SerializeBits<bool>(NoCollision, 1, name: nameof(NoCollision));
                        Balle = b.SerializeBits<bool>(Balle, 1, name: nameof(Balle));
                        Always = b.SerializeBits<bool>(Always, 1, name: nameof(Always));
                    }
                    else
                    {
                        Always = b.SerializeBits<bool>(Always, 1, name: nameof(Always));
                        Balle = b.SerializeBits<bool>(Balle, 1, name: nameof(Balle));
                        NoCollision = b.SerializeBits<bool>(NoCollision, 1, name: nameof(NoCollision));
                        HitRay = b.SerializeBits<bool>(HitRay, 1, name: nameof(HitRay));
                        KeepActive = b.SerializeBits<bool>(KeepActive, 1, name: nameof(KeepActive));
                        DetectZone = b.SerializeBits<bool>(DetectZone, 1, name: nameof(DetectZone));
                        Flag_06 = b.SerializeBits<bool>(Flag_06, 1, name: nameof(Flag_06));
                        Boss = b.SerializeBits<bool>(Boss, 1, name: nameof(Boss));

                        KeepLinkedObjectsActive = b.SerializeBits<bool>(KeepLinkedObjectsActive, 1, name: nameof(KeepLinkedObjectsActive));
                        Bonus = b.SerializeBits<bool>(Bonus, 1, name: nameof(Bonus));
                        BigRayHitKnockback = b.SerializeBits<bool>(BigRayHitKnockback, 1, name: nameof(BigRayHitKnockback));
                        RayDistMultisprCantchange = b.SerializeBits<bool>(RayDistMultisprCantchange, 1, name: nameof(RayDistMultisprCantchange));
                        InstantSpeedX = b.SerializeBits<bool>(InstantSpeedX, 1, name: nameof(InstantSpeedX));
                        InstantSpeedY = b.SerializeBits<bool>(InstantSpeedY, 1, name: nameof(InstantSpeedY));
                        SpecialPlatform = b.SerializeBits<bool>(SpecialPlatform, 1, name: nameof(SpecialPlatform));
                        ReadCmds = b.SerializeBits<bool>(ReadCmds, 1, name: nameof(ReadCmds));

                        MoveOnBlock = b.SerializeBits<bool>(MoveOnBlock, 1, name: nameof(MoveOnBlock));
                        FallInWater = b.SerializeBits<bool>(FallInWater, 1, name: nameof(FallInWater));
                        BlocksRay = b.SerializeBits<bool>(BlocksRay, 1, name: nameof(BlocksRay));
                        JumpOnBlock = b.SerializeBits<bool>(JumpOnBlock, 1, name: nameof(JumpOnBlock));
                        NoRayCollision = b.SerializeBits<bool>(NoRayCollision, 1, name: nameof(NoRayCollision));
                        KillIfOutsideActiveZone = b.SerializeBits<bool>(KillIfOutsideActiveZone, 1, name: nameof(KillIfOutsideActiveZone));
                        UturnOnBlock = b.SerializeBits<bool>(UturnOnBlock, 1, name: nameof(UturnOnBlock));
                        IncreaseSpeedX = b.SerializeBits<bool>(IncreaseSpeedX, 1, name: nameof(IncreaseSpeedX));

                        PoingCollisionSound = b.SerializeBits<bool>(PoingCollisionSound, 1, name: nameof(PoingCollisionSound));
                        DieInWater = b.SerializeBits<bool>(DieInWater, 1, name: nameof(DieInWater));
                        StopMovingUpWhenHitBlock = b.SerializeBits<bool>(StopMovingUpWhenHitBlock, 1, name: nameof(StopMovingUpWhenHitBlock));
                        SwitchOff = b.SerializeBits<bool>(SwitchOff, 1, name: nameof(SwitchOff));
                        Flag_1C = b.SerializeBits<bool>(Flag_1C, 1, name: nameof(Flag_1C));
                        LinkRequiresGendoor = b.SerializeBits<bool>(LinkRequiresGendoor, 1, name: nameof(LinkRequiresGendoor));
                        NoLink = b.SerializeBits<bool>(NoLink, 1, name: nameof(NoLink));
                        Flag_1F = b.SerializeBits<bool>(Flag_1F, 1, name: nameof(Flag_1F));
                    }
                });
            }
        }
    }
}