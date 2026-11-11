using System;
using System.IO;
using System.Linq;

namespace BinarySerializer.Ray1
{
    /// <summary>
    /// An object command collection
    /// </summary>
    public class ObjCommands : BinarySerializable
    {
        /// <summary>
        /// The commands
        /// </summary>
        public Command[] Commands { get; set; }

        /// <summary>
        /// Gets a command from the raw bytes
        /// </summary>
        /// <param name="bytes">The command bytes</param>
        /// <param name="contextFunc">The func used to create a new context</param>
        /// <returns>The command</returns>
        public static ObjCommands FromBytes(byte[] bytes, Func<Context> contextFunc)
        {
            // Make sure there are bytes
            if (!bytes.Any())
                return new ObjCommands() { Commands = Array.Empty<Command>() };

            // Create a new context
            using var context = contextFunc();

            // Create a memory stream
            using var memStream = new MemoryStream(bytes);

            // Stream key
            const string key = "CMDS";

            // Add the stream
            var file = new StreamFile(context, key, memStream);

            context.AddFile(file);

            // Deserialize the bytes
            return FileFactory.Read<ObjCommands>(context, key);
        }

        /// <summary>
        /// Gets the byte representation of the command
        /// </summary>
        /// <param name="contextFunc">The func used to create a new context</param>
        /// <returns>The command bytes</returns>
        public byte[] ToBytes(Func<Context> contextFunc)
        {
            using var context = contextFunc();
            
            // Create a memory stream
            using var memStream = new MemoryStream();
            
            // Stream key
            const string key = "CMDS";

            // Add the stream
            var file = new StreamFile(context, key, memStream);

            context.AddFile(file);

            // Serialize the command
            FileFactory.Write<ObjCommands>(context, key, this);

            // Return the bytes
            return memStream.ToArray();
        }

        public override void SerializeImpl(SerializerObject s)
        {
            Commands = s.SerializeObjectArrayUntil<Command>(Commands, 
                x => x.CommandType is CommandType.INVALID_CMD or CommandType.INVALID_CMD_DEMO, name: nameof(Commands));
        }

        public string[] ToTranslatedStrings(ushort[] labelOffsets, int lineStartIndex = 0) 
        {
            int[] lineNumbers;
            
            if (Commands == null || Commands.Length == 0) 
                return null;

            if (labelOffsets != null && labelOffsets.Length > 0) 
            {
                int[] commandOffsets = new int[Commands.Length + 1];
                int curOff = 0;
                for (int i = 0; i < commandOffsets.Length; i++) 
                {
                    commandOffsets[i] = curOff;

                    if (i < Commands.Length) 
                        curOff += Commands[i].Length;
                }
                lineNumbers = labelOffsets.Select(l => Array.IndexOf(commandOffsets, l + 1) + lineStartIndex).ToArray();
            } 
            else 
            {
                lineNumbers = Array.Empty<int>();
            }

            return Commands.Select((c, i) => 
                c.ToTranslatedString(
                    labelOffsetsLineNumbers: lineNumbers, 
                    prevCmd: Commands.ElementAtOrDefault(i - 1), 
                    nextCmd: Commands.ElementAtOrDefault(i + 1))).ToArray();
        }
    }
}