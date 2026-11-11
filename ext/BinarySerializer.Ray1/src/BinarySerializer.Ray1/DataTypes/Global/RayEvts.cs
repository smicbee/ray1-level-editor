namespace BinarySerializer.Ray1
{
    public class RayEvts : BinarySerializable
    {
        public bool Fist { get; set; }
        public bool Hang { get; set; }
        public bool Helico { get; set; }
        public bool SuperHelico { get; set; }

        // Only used in vol. 6 PS1 demo
        public bool HandstandDash { get; set; } // Beat Moskito
        public bool Handstand { get; set; } // Beat Mr Sax

        public bool Seed { get; set; }
        public bool Grab { get; set; }

        public bool Run { get; set; }
        public bool SmallRayman { get; set; }
        public bool Firefly { get; set; }
        public ToggledRayEvt ForceRun { get; set; }
        public ToggledRayEvt ReverseControls { get; set; }
        public bool Squished { get; set; } // On PS1 this is an unused death. On PC and later it's the mallet squish.

        public bool SuperSeed { get; set; }

        // Rayman 2
        public bool FistDownSlam { get; set; }
        public bool FistControl { get; set; }
        public bool FistPlatform { get; set; }
        public bool R2_Flag_08 { get; set; } // Unused

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            if (settings.EngineVersion == Ray1EngineVersion.R2_PS1)
            {
                s.DoBits<byte>(b =>
                {
                    Fist = b.SerializeBits<bool>(Fist, 1, name: nameof(Fist));
                    FistDownSlam = b.SerializeBits<bool>(FistDownSlam, 1, name: nameof(FistDownSlam));
                    FistControl = b.SerializeBits<bool>(FistControl, 1, name: nameof(FistControl));
                    FistPlatform = b.SerializeBits<bool>(FistPlatform, 1, name: nameof(FistPlatform));
                    Hang = b.SerializeBits<bool>(Hang, 1, name: nameof(Hang));
                    Helico = b.SerializeBits<bool>(Helico, 1, name: nameof(Helico));
                    SuperHelico = b.SerializeBits<bool>(SuperHelico, 1, name: nameof(SuperHelico));
                    Seed = b.SerializeBits<bool>(Seed, 1, name: nameof(Seed));
                });
                s.DoBits<byte>(b =>
                {
                    R2_Flag_08 = b.SerializeBits<bool>(R2_Flag_08, 1, name: nameof(R2_Flag_08));
                    Grab = b.SerializeBits<bool>(Grab, 1, name: nameof(Grab));
                    Run = b.SerializeBits<bool>(Run, 1, name: nameof(Run));
                    b.SerializePadding(5, logIfNotNull: true);
                });
                s.SerializePadding(2, logIfNotNull: true);
            }
            else
            {
                s.DoBits<byte>(b =>
                {
                    Fist = b.SerializeBits<bool>(Fist, 1, name: nameof(Fist));
                    Hang = b.SerializeBits<bool>(Hang, 1, name: nameof(Hang));
                    Helico = b.SerializeBits<bool>(Helico, 1, name: nameof(Helico));
                    SuperHelico = b.SerializeBits<bool>(SuperHelico, 1, name: nameof(SuperHelico));
                    HandstandDash = b.SerializeBits<bool>(HandstandDash, 1, name: nameof(HandstandDash));
                    Handstand = b.SerializeBits<bool>(Handstand, 1, name: nameof(Handstand));
                    Seed = b.SerializeBits<bool>(Seed, 1, name: nameof(Seed));
                    Grab = b.SerializeBits<bool>(Grab, 1, name: nameof(Grab));
                });
                s.DoBits<byte>(b =>
                {
                    Run = b.SerializeBits<bool>(Run, 1, name: nameof(Run));
                    SmallRayman = b.SerializeBits<bool>(SmallRayman, 1, name: nameof(SmallRayman));
                    Firefly = b.SerializeBits<bool>(Firefly, 1, name: nameof(Firefly));
                    ForceRun = b.SerializeBits<ToggledRayEvt>(ForceRun, 2, name: nameof(ForceRun));
                    ReverseControls = b.SerializeBits<ToggledRayEvt>(ReverseControls, 2, name: nameof(ReverseControls));
                    Squished = b.SerializeBits<bool>(Squished, 1, name: nameof(Squished));
                });

                if (settings.EngineVersion is
                    Ray1EngineVersion.PC_Edu or
                    Ray1EngineVersion.PS1_Edu or
                    Ray1EngineVersion.PC_Kit or
                    Ray1EngineVersion.PC_Fan)
                {
                    s.DoBits<byte>(b =>
                    {
                        SuperSeed = b.SerializeBits<bool>(SuperSeed, 1, name: nameof(SuperSeed));
                        b.SerializePadding(7, logIfNotNull: true);
                    });
                }
            }
        }
    }
}