namespace BinarySerializer.Ray1
{
    public class AnimationSet : BinarySerializable
    {
        public Pointer ETAPointer { get; set; }
        public Pointer AnimationsPointer { get; set; }
        public ushort AnimationsCount { get; set; }
        public ushort Ushort_0A { get; set; } // Usually 0. Appears unused. Padding?

        public Animation[] Animations { get; set; }
        public ETA ETA { get; set; }
        
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize the pointers
            ETAPointer = s.SerializePointer(ETAPointer, name: nameof(ETAPointer));
            AnimationsPointer = s.SerializePointer(AnimationsPointer, name: nameof(AnimationsPointer));

            // Serialize the values
            AnimationsCount = s.Serialize<ushort>(AnimationsCount, name: nameof(AnimationsCount));
            Ushort_0A = s.Serialize<ushort>(Ushort_0A, name: nameof(Ushort_0A));

            // Serialize the animations
            s.DoAt(AnimationsPointer, () => 
                Animations = s.SerializeObjectArray<Animation>(Animations, AnimationsCount, name: nameof(Animations)));

            // Serialize ETA
            s.DoAt(ETAPointer, () => ETA = s.SerializeObject<ETA>(ETA, name: nameof(ETA)));
        }
    }
}