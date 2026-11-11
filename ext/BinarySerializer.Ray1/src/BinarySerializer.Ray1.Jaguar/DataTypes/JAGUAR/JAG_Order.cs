namespace BinarySerializer.Ray1.Jaguar
{
    /// <summary>
    /// An order. Each frame the game executes the verb for every order. Used for map objects and other functionality.
    /// </summary>
    public class JAG_Order : BinarySerializable
    {
        public Pointer PrevPointer { get; set; }
        public ushort Verbe { get; set; }
        public Pointer DisplayPointer { get; set; }
        public byte[] Params { get; set; } // Same as the data in multi-sprite after the verbe

        public byte[] Unknown { get; set; } // Added after prototype - what is it?
        public short AbsoluteX { get; set; } // Is this correct? Game does x + unknown value + scroll x
        public short AbsoluteY { get; set; } // Same as above
        public Pointer OriginalParamsPointer { get; set; } // Is this correct?

        public Pointer NextPointer { get; set; }

        // Serialized from pointers
        public JAG_Display Display { get; set; } // Optional for orders which display something

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            PrevPointer = s.SerializePointer(PrevPointer, size: PointerSize.Pointer16, name: nameof(PrevPointer));
            Verbe = s.Serialize<ushort>(Verbe, name: nameof(Verbe));
            DisplayPointer = s.SerializePointer(DisplayPointer, size: PointerSize.Pointer16, name: nameof(DisplayPointer));
            Params = s.SerializeArray<byte>(Params, 32, name: nameof(Params));

            if (settings.EngineVersion != Ray1EngineVersion.Jaguar_Proto)
            {
                Unknown = s.SerializeArray<byte>(Unknown, 4, name: nameof(Unknown));

                if (settings.EngineVersion == Ray1EngineVersion.Jaguar)
                {
                    AbsoluteX = s.Serialize<short>(AbsoluteX, name: nameof(AbsoluteX));
                    AbsoluteY = s.Serialize<short>(AbsoluteY, name: nameof(AbsoluteY));
                    OriginalParamsPointer = s.SerializePointer(OriginalParamsPointer, name: nameof(OriginalParamsPointer));
                }
            }

            NextPointer = s.SerializePointer(NextPointer, size: PointerSize.Pointer16, name: nameof(NextPointer));

            s.DoAt(DisplayPointer, () => Display = s.SerializeObject<JAG_Display>(Display, name: nameof(Display)));
        }
    }
}