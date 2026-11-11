using System.Collections.Generic;
using System.Linq;

namespace BinarySerializer.Ray1.Jaguar
{
    /// <summary>
    /// Level load commands for Rayman 1 (Jaguar)
    /// </summary>
    public class JAG_LevelLoadCommandCollection : BinarySerializable
    {
        /// <summary>
        /// The commands
        /// </summary>
        public JAG_LevelLoadCommand[] Commands { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            if (Commands == null) {
                // Create a temporary list
                var cmd = new List<JAG_LevelLoadCommand>();

                int index = 0;

                // Loop until we reach the end command
                while (cmd.LastOrDefault()?.Type != JAG_LevelLoadCommand.LevelLoadCommandType.End) 
                {
                    cmd.Add(s.SerializeObject<JAG_LevelLoadCommand>(null, name: $"{nameof(Commands)}[{index}]"));
                    index++;
                }

                // Set the commands
                Commands = cmd.ToArray();
            } 
            else 
            {
                // Serialize the commands
                s.SerializeObjectArray(Commands, Commands.Length, name: nameof(Commands));
            }
        }
    }
}