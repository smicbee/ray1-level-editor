namespace BinarySerializer.Ray1.Jaguar
{
    /// <summary>
    /// Event state
    /// </summary>
    public class JAG_EventState : BinarySerializable
    {
        public byte Byte_00 { get; set; }
        public byte AnimationSpeed { get; set; }

        public Pointer AnimationPointer { get; set; }
        public Pointer LinkedStatePointer { get; set; }
        public Pointer CodePointer { get; set; }

        public byte Byte_0A { get; set; }
        public byte Byte_0B { get; set; }

        // Parsed
        public Animation Animation { get; set; }
        public JAG_EventState LinkedState { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            AnimationSpeed = s.Serialize<byte>(AnimationSpeed, name: nameof(AnimationSpeed));
            
            if (Byte_00 != 0) 
                AnimationPointer = s.SerializePointer(AnimationPointer, name: nameof(AnimationPointer));
            else 
                LinkedStatePointer = s.SerializePointer(LinkedStatePointer, name: nameof(LinkedStatePointer));
            
            CodePointer = s.SerializePointer(CodePointer, name: nameof(CodePointer));
            Byte_0A = s.Serialize<byte>(Byte_0A, name: nameof(Byte_0A));
            Byte_0B = s.Serialize<byte>(Byte_0B, name: nameof(Byte_0B));

            Animation = s.DoAt(AnimationPointer, () => s.SerializeObject<Animation>(Animation, name: nameof(Animation)));
            LinkedState = s.DoAt(LinkedStatePointer, () => s.SerializeObject<JAG_EventState>(LinkedState, name: nameof(LinkedState)));
        }
    }
}