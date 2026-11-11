using System;
using System.Collections.Generic;
using System.Linq;

namespace BinarySerializer.Ray1
{
    /// <summary>
    /// Compiles and decompiles object commands
    /// </summary>
    public static class ObjCommandCompiler
    {
        /// <summary>
        /// Compiles the commands
        /// </summary>
        /// <param name="commands">The commands</param>
        /// <returns>The compiled the command data</returns>
        public static CompiledObjCommandData Compile(ObjCommands commands)
        {
            // Create the lists to compile to
            var compiledCommands = new List<Command>();
            var labelOffsets = new List<ushort>();
            
            // Keep track of the command byte index
            var commandByteIndex = 0;
            var commandIndex = 0;

            foreach (var command in commands.Commands)
            {
                // Increment the index for the command declaration
                commandByteIndex++;

                CommandType GetCompiledVersion() =>
                    command.CommandType switch
                    {
                        CommandType.GO_GOSUB => CommandType.RESERVED_GO_GOSUB,
                        CommandType.GO_GOTO => CommandType.RESERVED_GO_GOTO,
                        CommandType.GO_BRANCHTRUE => CommandType.RESERVED_GO_GOTOT,
                        CommandType.GO_BRANCHFALSE => CommandType.RESERVED_GO_GOTOF,
                        CommandType.GO_SKIP => CommandType.RESERVED_GO_SKIP,
                        CommandType.GO_SKIPTRUE => CommandType.RESERVED_GO_SKIPT,
                        CommandType.GO_SKIPFALSE => CommandType.RESERVED_GO_SKIPF,
                        _ => throw new ArgumentOutOfRangeException(nameof(command.CommandType), command.CommandType,
                            null)
                    };

                switch (command.CommandType)
                {
                    // Handle commands with offsets
                    case CommandType.GO_GOSUB:
                    case CommandType.GO_GOTO:
                    case CommandType.GO_BRANCHTRUE:
                    case CommandType.GO_BRANCHFALSE:

                        // Get the label
                        var label = command.Arguments[0];

                        // Find the matching commands
                        var labelCmdIndex = Array.FindIndex(commands.Commands, x => x.CommandType == CommandType.GO_LABEL && x.Arguments.FirstOrDefault() == label);

                        // Get the offset 
                        var offset = commands.Commands.Take(labelCmdIndex).Sum(x => x.Length) + 1;

                        // Add the offset to the label offsets
                        labelOffsets.Add((ushort)offset);

                        // Add the command
                        compiledCommands.Add(new Command()
                        {
                            CommandType = GetCompiledVersion(),
                            Arguments = new byte[]
                            {
                                (byte)(labelOffsets.Count - 1)
                            }
                        });

                        break;

                    // Handle commands with offsets based on current position
                    case CommandType.GO_SKIP:
                    case CommandType.GO_SKIPTRUE:
                    case CommandType.GO_SKIPFALSE:

                        // Get the number of cmds to skip
                        var toSkip = command.Arguments[0];

                        // Get the offset to skip to, starting at the next command index
                        var skipToOffset = commandByteIndex + 1;

                        int curCmd = commandIndex + 1;
                        for (int i = 0; i < toSkip; i++)
                        {
                            var cmd = commands.Commands[curCmd % commands.Commands.Length];
                            skipToOffset += cmd.Length;
                            if (cmd.CommandType == CommandType.INVALID_CMD)
                            {
                                curCmd = 0;
                                skipToOffset = commands.Commands[curCmd].Length;
                            }
                            curCmd++;
                        }

                        if (commands.Commands[curCmd % commands.Commands.Length].CommandType == CommandType.GO_LABEL)
                        {
                            skipToOffset += 2;
                            curCmd++;
                        }

                        skipToOffset -= 1;

                        // Overflow
                        skipToOffset %= commands.Commands.Sum(x => x.Length);

                        // Add the offset to the label offsets
                        labelOffsets.Add((byte)skipToOffset);

                        // Add the command
                        compiledCommands.Add(new Command()
                        {
                            CommandType = GetCompiledVersion(),
                            Arguments = new byte[]
                            {
                                (byte)(labelOffsets.Count - 1)
                            }
                        });

                        break;

                    // Handle others
                    default:

                        // Copy the command
                        compiledCommands.Add(new Command()
                        {
                            CommandType = command.CommandType,
                            Arguments = command.Arguments.ToArray()
                        });

                    break;
                }

                // Increment the index for every argument
                commandByteIndex += command.Arguments.Length;
                commandIndex++;
            }

            // Return the compiled commands
            return new CompiledObjCommandData(new ObjCommands()
            {
                Commands = compiledCommands.ToArray()
            }, labelOffsets.ToArray());
        }

        /// <summary>
        /// Decompiles the commands
        /// </summary>
        /// <param name="commands">The commands</param>
        /// <param name="commandBytes">The command bytes</param>
        /// <returns>The decompiled commands</returns>
        public static ObjCommands Decompile(CompiledObjCommandData commands, byte[] commandBytes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Data for compiled obj commands
        /// </summary>
        public class CompiledObjCommandData
        {
            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="commands">The object commands</param>
            /// <param name="labelOffsets">The label offsets</param>
            public CompiledObjCommandData(ObjCommands commands, ushort[] labelOffsets)
            {
                Commands = commands;
                LabelOffsets = labelOffsets;
            }

            /// <summary>
            /// The object commands
            /// </summary>
            public ObjCommands Commands { get; }

            /// <summary>
            /// The label offsets
            /// </summary>
            public ushort[] LabelOffsets { get; }
        }
    }
}