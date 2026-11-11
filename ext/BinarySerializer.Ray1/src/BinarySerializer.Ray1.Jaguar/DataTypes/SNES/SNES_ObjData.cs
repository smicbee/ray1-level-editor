using System.Linq;

namespace BinarySerializer.Ray1.Jaguar
{
    public class SNES_ObjData : BinarySerializable
    {
        public SNES_Pointer UnknownStatesPointer { get; set; }
        public SNES_Pointer SpritesPointer { get; set; }
        public ushort Ushort_04 { get; set; } // 0
        public short XPosition { get; set; }
        public short YPosition { get; set; }
        public byte[] Bytes_0A { get; set; }
        public SNES_Pointer StatesPointer { get; set; }

        public SNES_State[] States { get; set; }
        public SNES_Sprite[] Sprites { get; set; }
        public SNES_Pointer[] UnknownStatesPointers { get; set; }
        public SNES_State[] UnknownStates { get; set; } // References to some of the states in the normal state array

        public override void SerializeImpl(SerializerObject s)
        {
            UnknownStatesPointer = s.SerializeObject<SNES_Pointer>(UnknownStatesPointer, onPreSerialize: x => x.Pre_MemoryBankOverride = 4, name: nameof(UnknownStatesPointer));
            SpritesPointer = s.SerializeObject<SNES_Pointer>(SpritesPointer, onPreSerialize: x => x.Pre_MemoryBankOverride = 4, name: nameof(SpritesPointer));

            Ushort_04 = s.Serialize<ushort>(Ushort_04, name: nameof(Ushort_04));

            XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));

            Bytes_0A = s.SerializeArray<byte>(Bytes_0A, 6, name: nameof(Bytes_0A));

            StatesPointer = s.SerializeObject<SNES_Pointer>(StatesPointer, onPreSerialize: x => x.Pre_MemoryBankOverride = 4, name: nameof(StatesPointer));

            if (!s.FullSerialize)
                return;

            // Serialize data from pointers
            States = s.DoAt(StatesPointer.GetPointer(), () => s.SerializeObjectArray<SNES_State>(States, 5 * 0x15, name: nameof(States)));
            Sprites = s.DoAt(SpritesPointer.GetPointer(), () => s.SerializeObjectArray<SNES_Sprite>(Sprites, States.Max(state => state.Animation?.Layers.Max(layer => layer.SpriteIndex + 1) ?? 0), name: nameof(Sprites)));
            
            UnknownStatesPointers = s.DoAt(UnknownStatesPointer.GetPointer(), () => s.SerializeObjectArray<SNES_Pointer>(UnknownStatesPointers, 16, onPreSerialize: x => x.Pre_MemoryBankOverride = 4, name: nameof(UnknownStatesPointers)));

            UnknownStates ??= new SNES_State[UnknownStatesPointers.Length];

            for (int i = 0; i < UnknownStates.Length; i++)
                UnknownStates[i] = s.DoAt(UnknownStatesPointers[i].GetPointer(), () => s.SerializeObject<SNES_State>(UnknownStates[i], name: $"{nameof(UnknownStates)}[{i}]"));
        }
    }
}