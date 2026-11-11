using System;

namespace BinarySerializer.Ray1.PC
{
    /// <summary>
    /// Obj commands and their label offsets for PC
    /// </summary>
    public class ObjCommandsData : BinarySerializable
    {
        public ObjCommands Commands { get; set; }
        public ushort[] LabelOffsetTable { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            DataLengthProcessor commandsDataLengthProcessor = new();

            // Serialize the lengths
            commandsDataLengthProcessor.Serialize<ushort>(s, name: "CommandsLength");
            LabelOffsetTable = s.SerializeArraySize<ushort, ushort>(LabelOffsetTable, name: nameof(LabelOffsetTable));

            // Serialize the commands
            s.DoProcessed(commandsDataLengthProcessor, () =>
            {
                if (commandsDataLengthProcessor.SerializedValue > 0 || Commands?.Commands?.Length > 0)
                    Commands = s.SerializeObject<ObjCommands>(Commands, name: nameof(Commands));
                else
                    Commands = new ObjCommands() { Commands = Array.Empty<Command>() };
            });

            // Serialize the label offsets
            LabelOffsetTable = s.SerializeArray<ushort>(LabelOffsetTable, LabelOffsetTable.Length, name: nameof(LabelOffsetTable));
        }
    }
}