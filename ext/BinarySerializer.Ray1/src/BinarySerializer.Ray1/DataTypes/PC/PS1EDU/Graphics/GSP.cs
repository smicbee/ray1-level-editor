namespace BinarySerializer.Ray1.PC.PS1EDU
{
    public class GSP : BinarySerializable
    {
        /// <summary>
        /// Indices for the descriptor array in <see cref="TEX"/>
        /// </summary>
        public ushort[] Indices { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Indices = s.SerializeArraySize<ushort, ushort>(Indices, name: nameof(Indices));
            Indices = s.SerializeArray<ushort>(Indices, Indices.Length, name: nameof(Indices));
        }
    }
}