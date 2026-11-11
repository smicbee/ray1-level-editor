namespace BinarySerializer.Ray1.Jaguar
{
    public class SNES_AnimatedTileEntry : BinarySerializable
    {
        public SNES_Pointer GraphicsPointer { get; set; }
        public ushort VRAMAddress { get; set; }
        public ushort BytesToCopy { get; set; }
        public byte[] Unknown { get; set; }

        public byte[] GraphicsBuffer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            GraphicsPointer = s.SerializeObject<SNES_Pointer>(GraphicsPointer, onPreSerialize: p => p.Pre_HasMemoryBankValue = true, name: nameof(GraphicsPointer));
            VRAMAddress = s.Serialize<ushort>(VRAMAddress, name: nameof(VRAMAddress));
            BytesToCopy = s.Serialize<ushort>(BytesToCopy, name: nameof(BytesToCopy));
            Unknown = s.SerializeArray<byte>(Unknown, BytesToCopy >> 8, name: nameof(Unknown));

            GraphicsBuffer = s.DoAt(GraphicsPointer.GetPointer(), () => s.SerializeArray<byte>(GraphicsBuffer, BytesToCopy, name: nameof(GraphicsBuffer)));
        }
    }
}