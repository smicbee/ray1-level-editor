namespace BinarySerializer.Ray1.Jaguar
{
    /// <summary>
    /// An event instance in the map. This is used to spawn a multi-sprite character.
    /// </summary>
    public class JAG_Event : BinarySerializable
    {
        // This value is 0 if the event is not valid
        public ushort Ushort_00 { get; set; }

        // Offsets for the position
        public short OffsetX { get; set; }
        public short OffsetY { get; set; }

        public Pointer EventDefinitionPointer { get; set; }

        // Always 0x05? Probably the same as InitFlag in multi-platform versions, but what is that?
        public ushort Unk_0A { get; set; }

        public ushort EventIndex { get; set; }

        // Parsed
        public JAG_MultiSprite MultiSprite { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));

            if (Ushort_00 == 0)
                return;

            OffsetX = s.Serialize<short>(OffsetX, name: nameof(OffsetX));
            OffsetY = s.Serialize<short>(OffsetY, name: nameof(OffsetY));
            EventDefinitionPointer = s.SerializePointer(EventDefinitionPointer, name: nameof(EventDefinitionPointer));
            Unk_0A = s.Serialize<ushort>(Unk_0A, name: nameof(Unk_0A));
            EventIndex = s.Serialize<ushort>(EventIndex, name: nameof(EventIndex));

            MultiSprite = s.DoAt(EventDefinitionPointer, () => s.SerializeObject<JAG_MultiSprite>(MultiSprite, name: nameof(MultiSprite)));
        }
    }
}