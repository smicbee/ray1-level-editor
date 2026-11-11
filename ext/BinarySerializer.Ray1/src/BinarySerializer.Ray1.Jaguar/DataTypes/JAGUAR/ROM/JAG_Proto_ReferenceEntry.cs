namespace BinarySerializer.Ray1.Jaguar
{
    public class JAG_Proto_ReferenceEntry : BinarySerializable, ISerializerShortLog
    {
        public Pointer Pre_StringBasePointer { get; set; }

        public Pointer StringPointer { get; set; }
        public EntryType Type { get; set; }
        public Pointer DataPointer { get; set; }
        public uint DataValue { get; set; }

        public string String { get; set; }

        public string ShortLog => ToString();
        public override string ToString() => $"Ref_{Type}('{String}' = {DataPointer?.ToString() ?? $"0x{DataValue:X8}"})";

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            StringPointer = s.SerializePointer(StringPointer, anchor: Pre_StringBasePointer, name: nameof(StringPointer));
            Type = s.Serialize<EntryType>(Type, name: nameof(Type));
            s.SerializePadding(3, logIfNotNull: true);

            if (Type == EntryType.ROM)
                DataPointer = s.SerializePointer(DataPointer, name: nameof(DataPointer));
            else
                DataValue = s.Serialize<uint>(DataValue, name: nameof(DataValue));

            s.DoAt(StringPointer, () => String = s.SerializeString(String, name: nameof(String)));

            if (Type == EntryType.ROM)
                Offset.File.AddLabel(DataPointer.FileOffset, String);
        }

        public enum EntryType : byte
        {
            /// <summary>
            /// A constant value
            /// </summary>
            Const = 2,

            /// <summary>
            /// Pointer to a data block or function in the ROM
            /// </summary>
            ROM = 6,

            /// <summary>
            /// Pointer to data in RAM
            /// </summary>
            RAM = 8
        }
    }
}