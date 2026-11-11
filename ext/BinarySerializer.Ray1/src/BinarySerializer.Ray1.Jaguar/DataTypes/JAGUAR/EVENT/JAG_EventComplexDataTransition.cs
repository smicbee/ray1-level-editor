namespace BinarySerializer.Ray1.Jaguar
{
    /// <summary>
    /// Event graphics block for some special events in Rayman 1 (Jaguar)
    /// </summary>
    public class JAG_EventComplexDataTransition : BinarySerializable
    {
        public ushort Pre_Verbe { get; set; } // Read from MultiSprite
        public ushort Pre_NumLayers { get; set; } // Read from MultiSprite

        public Pointer ComplexDataPointer { get; set; }
        public Pointer InitFunctionPointer { get; set; }
        public Pointer MainFunctionPointer { get; set; }
        public byte[] Bytes_0C { get; set; } // TODO: At least 2 more code pointers

        // Parsed
        public JAG_EventComplexData ComplexData { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            ComplexDataPointer = s.SerializePointer(ComplexDataPointer, name: nameof(ComplexDataPointer));
            InitFunctionPointer = s.SerializePointer(InitFunctionPointer, name: nameof(InitFunctionPointer));
            MainFunctionPointer = s.SerializePointer(MainFunctionPointer, name: nameof(MainFunctionPointer));
            Bytes_0C = s.SerializeArray<byte>(Bytes_0C, 0x14, name: nameof(Bytes_0C));

            s.DoAt(ComplexDataPointer, () => 
            {
                ComplexData = s.SerializeObject<JAG_EventComplexData>(ComplexData, onPreSerialize: complexData => {
                    complexData.Pre_Verbe = Pre_Verbe;
                    complexData.Pre_SpritesCount = Pre_NumLayers;
                }, name: nameof(ComplexData));
            });
        }
    }
}