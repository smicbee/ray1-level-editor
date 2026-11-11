namespace BinarySerializer.Ray1
{
    public class Credit : BinarySerializable
    {
        public Pointer TextPointer { get; set; }
        public uint PCPacked_TextPointer { get; set; }

        public short XPos { get; set; }
        public short YPos { get; set; }

        public ushort LocTextId { get; set; } // For localized text, otherwise 0
        public byte Font { get; set; }
        public byte Command { get; set; }
        public byte Color { get; set; }

        public string Text { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            if (settings.IsLoadingPackedPCData)
                PCPacked_TextPointer = s.Serialize<uint>(PCPacked_TextPointer, name: nameof(PCPacked_TextPointer));
            else
                TextPointer = s.SerializePointer(TextPointer, name: nameof(TextPointer));

            XPos = s.Serialize<short>(XPos, name: nameof(XPos));
            YPos = s.Serialize<short>(YPos, name: nameof(YPos));

            if (settings.EngineVersion is 
                Ray1EngineVersion.PC_Edu or 
                Ray1EngineVersion.PS1_Edu or 
                Ray1EngineVersion.PC_Kit or 
                Ray1EngineVersion.PC_Fan)
                LocTextId = s.Serialize<ushort>(LocTextId, name: nameof(LocTextId));
            
            Font = s.Serialize<byte>(Font, name: nameof(Font));
            Command = s.Serialize<byte>(Command, name: nameof(Command));
            Color = s.Serialize<byte>(Color, name: nameof(Color));

            s.Align(baseOffset: Offset);

            if (settings.IsLoadingPackedPCData)
                Text = s.SerializeLengthPrefixedString<byte>(Text, name: nameof(Text));
            else
                s.DoAt(TextPointer, () => Text = s.SerializeString(Text, name: nameof(Text)));
        }
    }
}