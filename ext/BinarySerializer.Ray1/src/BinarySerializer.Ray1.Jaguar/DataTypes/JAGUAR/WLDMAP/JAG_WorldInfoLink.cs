using System;

namespace BinarySerializer.Ray1.Jaguar
{
    public class JAG_WorldInfoLink : BinarySerializable
    {
        public Direction Directions { get; set; }
        public Pointer<JAG_WorldInfo> EntryPointer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Directions = s.Serialize<Direction>(Directions, name: nameof(Directions));

            if (Directions == Direction.None)
                return;

            // Resolve later for a cleaner log
            EntryPointer = s.SerializePointer<JAG_WorldInfo>(EntryPointer, name: nameof(EntryPointer));
        }

        [Flags]
        public enum Direction : ushort
        {
            None = 0,
            Up = 1 << 0,
            Down = 1 << 1,
            Left = 1 << 2,
            Right = 1 << 3,
        }
    }
}