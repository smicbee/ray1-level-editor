namespace BinarySerializer.Ray1.Jaguar
{
    // The collide in memory
    public class JAG_Collide : BinarySerializable
    {
        public short XPos { get; set; }
        public short YPos { get; set; }
        public short Width { get; set; }
        public short Height { get; set; }
        public JAG_CollideType Type { get; set; }
        public Pointer DisplayPointer { get; set; }

        // Serialized from pointers
        public JAG_Display Display { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            XPos = s.Serialize<short>(XPos, name: nameof(XPos));
            YPos = s.Serialize<short>(YPos, name: nameof(YPos));
            Width = s.Serialize<short>(Width, name: nameof(Width));
            Height = s.Serialize<short>(Height, name: nameof(Height));
            Type = s.Serialize<JAG_CollideType>(Type, name: nameof(Type));
            DisplayPointer = s.SerializePointer(DisplayPointer, name: nameof(DisplayPointer));

            s.DoAt(DisplayPointer, () => Display = s.SerializeObject<JAG_Display>(Display, name: nameof(Display)));
        }
    }
}