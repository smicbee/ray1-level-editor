namespace BinarySerializer.Ray1.Jaguar
{
    /// <summary>
    /// SNES Pointer class. See https://datacrystal.romhacking.net/wiki/Pointer#SNES_Pointers
    /// </summary>
    public class SNES_Pointer : BinarySerializable
    {
        // Set before serializing
        public bool Pre_HasMemoryBankValue { get; set; } = false;
        public byte? Pre_MemoryBankOverride { get; set; }

        public ushort Pointer { get; set; }
        public byte MemoryBank { get; set; }

        private Pointer _cachedPointer;
        public Pointer GetPointer() 
        {
            if (_cachedPointer == null) 
            {
                // The ROM is split into memory banks, with the size 0x8000 which get loaded at 0x8000 in RAM.
                var baseOffset = Offset.File.StartPointer;

                long bank = Pre_MemoryBankOverride ?? MemoryBank;

                // If we don't have a bank value the pointer is in the current bank
                if (!Pre_HasMemoryBankValue && Pre_MemoryBankOverride == null)
                    bank = Offset.FileOffset / MemoryBankSize;

                _cachedPointer = GetPointer(baseOffset, Pointer, bank);
            }
            return _cachedPointer;
        }

        public static Pointer GetPointer(Pointer romBaseOffset, long pointer, long memoryBank) => romBaseOffset + (MemoryBankSize * memoryBank) + (pointer - MemoryBankBaseAddress);

        public const long MemoryBankBaseAddress = 0x8000;
        public const long MemoryBankSize = 0x8000;

        public override void SerializeImpl(SerializerObject s) 
        {
            if (Pre_HasMemoryBankValue) 
                MemoryBank = s.Serialize<byte>(MemoryBank, name: nameof(MemoryBank));
            Pointer = s.Serialize<ushort>(Pointer, name: nameof(Pointer));

            s.Log("Pointer: {0}", GetPointer());
        }
    }
}