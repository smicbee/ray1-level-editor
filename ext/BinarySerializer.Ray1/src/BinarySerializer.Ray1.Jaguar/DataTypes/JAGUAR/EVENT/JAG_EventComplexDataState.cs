namespace BinarySerializer.Ray1.Jaguar
{
    /// <summary>
    /// Event state for Rayman 1 (Jaguar)
    /// </summary>
    public class JAG_EventComplexDataState : BinarySerializable
    {
        public Pointer AnimationPointer { get; set; }
        public byte Deplacement { get; set; } // Some offset?
        public byte FlipInt { get; set; } // 6 bits, 1 bit, 1 bit
        public byte LoopsCount { get; set; } // Number of times to play animation before switching state
        public byte Chain { get; set; } // Linked state index
        public byte FramesCount { get; set; }
        public byte Speed { get; set; } // Speed
        public byte FlipAnim { get; set; }
        public byte ZddId { get; set; } // ZDD index
        public byte ZddValue { get; set; }
        public byte Insertion { get; set; } // Index to object to spawn from table. 0 = no object.

        // Passed from other structs
        public ushort LayersPerFrame { get; set; }

        // Parsed
        public AnimationLayer[] Layers { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            AnimationPointer = s.SerializePointer(AnimationPointer, name: nameof(AnimationPointer));
            Deplacement = s.Serialize<byte>(Deplacement, name: nameof(Deplacement));
            FlipInt = s.Serialize<byte>(FlipInt, name: nameof(FlipInt));
            LoopsCount = s.Serialize<byte>(LoopsCount, name: nameof(LoopsCount));
            Chain = s.Serialize<byte>(Chain, name: nameof(Chain));
            FramesCount = s.Serialize<byte>(FramesCount, name: nameof(FramesCount));
            Speed = s.Serialize<byte>(Speed, name: nameof(Speed));
            FlipAnim = s.Serialize<byte>(FlipAnim, name: nameof(FlipAnim));
            ZddId = s.Serialize<byte>(ZddId, name: nameof(ZddId));
            ZddValue = s.Serialize<byte>(ZddValue, name: nameof(ZddValue));
            Insertion = s.Serialize<byte>(Insertion, name: nameof(Insertion));
            s.SerializePadding(2, logIfNotNull: false); // TODO: Enable this once we don't serialize invalid states

            if (AnimationPointer != null) 
            {
                // AnimationPointer points to first layer. So, go back 4 bytes to get header
                /*s.DoAt(AnimationPointer - 0x4, () => {
                    Animation = s.SerializeObject<Jaguar_R1_AnimationDescriptor>(Animation, name: nameof(Animation));
                });*/
                Layers = s.DoAt(AnimationPointer, () => s.SerializeObjectArray<AnimationLayer>(Layers, LayersPerFrame * FramesCount, name: nameof(Layers)));
            }
        }
    }
}