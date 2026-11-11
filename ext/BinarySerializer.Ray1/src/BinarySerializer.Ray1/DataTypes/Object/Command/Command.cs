using System;
using System.Collections.Generic;

namespace BinarySerializer.Ray1
{
    /// <summary>
    /// An object command
    /// </summary>
    public class Command : BinarySerializable
    {
        /// <summary>
        /// The command type
        /// </summary>
        public CommandType CommandType { get; set; }

        /// <summary>
        /// The command arguments
        /// </summary>
        public byte[] Arguments { get; set; }

        /// <summary>
        /// The length of the command in bytes
        /// </summary>
        public int Length => Arguments.Length + 1;

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            CommandType = s.Serialize<CommandType>(CommandType, name: nameof(CommandType));

            if (Arguments == null)
            {
                switch (CommandType)
                {
                    case CommandType.GO_LEFT:
                    case CommandType.GO_RIGHT:
                    case CommandType.GO_WAIT:
                    case CommandType.GO_UP:
                    case CommandType.GO_DOWN:
                    case CommandType.GO_SUBSTATE:
                    case CommandType.GO_SKIP:
                    case CommandType.GO_ADD:
                    case CommandType.GO_STATE:
                    case CommandType.GO_PREPARELOOP:
                    case CommandType.GO_LABEL:
                    case CommandType.GO_GOTO:
                    case CommandType.GO_GOSUB:
                    case CommandType.GO_BRANCHTRUE:
                    case CommandType.GO_BRANCHFALSE:
                    case CommandType.GO_SETTEST:
                    case CommandType.GO_WAITSTATE:
                    case CommandType.RESERVED_GO_SKIP:
                    case CommandType.RESERVED_GO_GOTO:
                    case CommandType.RESERVED_GO_GOSUB:
                    case CommandType.RESERVED_GO_GOTOT:
                    case CommandType.RESERVED_GO_GOTOF:
                    case CommandType.RESERVED_GO_SKIPT:
                    case CommandType.RESERVED_GO_SKIPF:
                    case CommandType.GO_NOP:
                    case CommandType.GO_SKIPTRUE:
                    case CommandType.GO_SKIPFALSE:
                    case CommandType.INVALID_CMD:
                    case CommandType.INVALID_CMD_DEMO:
                        Arguments = s.SerializeArray<byte>(Arguments, 1, name: nameof(Arguments));
                        break;

                    case CommandType.GO_DOLOOP:
                    case CommandType.GO_RETURN:
                        Arguments = s.SerializeArray<byte>(Arguments, settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol3 ? 1 : 0, name: nameof(Arguments));
                        break;

                    case CommandType.GO_TEST:
                        var tempList = new List<byte>();

                        tempList.Add(s.Serialize<byte>((byte)0, name: $"{nameof(Arguments)}[0]"));

                        if (tempList[0] <= 4)
                            tempList.Add(s.Serialize<byte>((byte)0, name: $"{nameof(Arguments)}[1]"));

                        Arguments = tempList.ToArray();

                        break;

                    case CommandType.GO_SPEED:
                        Arguments = s.SerializeArray<byte>(Arguments, 3, name: nameof(Arguments));
                        break;

                    case CommandType.GO_X:
                    case CommandType.GO_Y:
                        Arguments = s.SerializeArray<byte>(Arguments, 2, name: nameof(Arguments));
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(CommandType), CommandType, null);
                }
            }
            else
            {
                s.SerializeArray<byte>(Arguments, Arguments.Length, name: nameof(Arguments));
            }
        }

        public string ToTranslatedString(int[] labelOffsetsLineNumbers, Command prevCmd, Command nextCmd) 
        {
            string cmd = $"{CommandType}";

            var prepend = "\t";

            if (RequiresTestTrue() || RequiresTestFalse())
            {
                if (prevCmd?.CommandType is CommandType.GO_TEST or CommandType.GO_SETTEST)
                    prepend = $"{prepend}\t";
                else
                    prepend += $"IF ({(RequiresTestFalse() ? "!" : "")}TEST) ";
            }

            cmd = cmd.Replace("FALSE", "");
            cmd = cmd.Replace("TRUE", "");

            switch (CommandType) 
            {
                case CommandType.RESERVED_GO_GOTO:
                case CommandType.RESERVED_GO_GOTOF:
                case CommandType.RESERVED_GO_GOTOT:
                    cmd = "GOTO";
                    return $"{prepend}{cmd} LINE {labelOffsetsLineNumbers[Arguments[0]]};";
                            
                case CommandType.RESERVED_GO_GOSUB:
                case CommandType.RESERVED_GO_SKIP:
                case CommandType.RESERVED_GO_SKIPF:
                case CommandType.RESERVED_GO_SKIPT:
                    cmd = cmd.Replace("RESERVED_GO_", "");
                    if (cmd is "SKIPT" or "SKIPF") 
                        cmd = "SKIP";
                    return $"{prepend}{cmd} TO LINE {labelOffsetsLineNumbers[Arguments[0]]};";

                case CommandType.GO_LABEL:
                    return "LABEL " + Arguments[0] + ":";

                case CommandType.GO_RETURN:
                    if (Arguments.Length == 0) {
                        return $"{prepend}RETURN;";
                    } else {
                        cmd = cmd.Replace("GO_", "");
                        return $"{prepend}{cmd} {String.Join(" ", Arguments)};";
                    }

                case CommandType.GO_SPEED:
                    cmd = cmd.Replace("GO_", "");
                    return $"{prepend}{cmd} {Arguments[0]} {(sbyte)Arguments[1]} {(sbyte)Arguments[2]}";

                case CommandType.GO_X:
                case CommandType.GO_Y:
                    cmd = cmd.Replace("GO_", "");
                    return $"{prepend}{cmd} {Arguments[0] * 100 + Arguments[1]}";

                case CommandType.GO_TEST:
                case CommandType.GO_SETTEST:
                    var append = String.Empty;

                    if (nextCmd?.RequiresTestFalse() == true || nextCmd?.RequiresTestTrue() == true)
                    {
                        prepend = $"{prepend}IF (";
                        append = ")";
                    }
                    else
                    {
                        prepend = $"{prepend}TEST = ";
                    }

                    if (CommandType == CommandType.GO_SETTEST)
                        return $"{prepend}{(Arguments[0] == 1).ToString().ToUpper()}{append}";
                    else
                        switch (Arguments[0])
                        {
                            case 0:
                                return $"{prepend}{(Arguments[1] == 1 ? "": "!")}FLIPX{append}";
                            case 1:
                                return $"{prepend}RANDOM({Arguments[1]}){append}"; // myRand. RandArray[RandomIndex] % (Argument1 + 1);
                            case 2:
                                return $"{prepend}RAYMAN.X {(Arguments[1] == 1 ? ">" : "<=")} X{append}";
                            case 3:
                                return $"{prepend}STATE == {Arguments[1]}{append}";
                            case 4:
                                return $"{prepend}SUBSTATE == {Arguments[1]}{append}";
                            case 70:
                                return $"{prepend}OBJ_IN_ZONE{append}";
                            case 71:
                                return $"{prepend}HASFLAG(0){append}"; // TODO: What is this flag?
                            case 72:
                                return $"{prepend}!HASFLAG(4){append}"; // TODO: What is this flag?
                            default:
                                return $"{prepend}<UNKNOWN TEST ({Arguments[0]})>{append}";
                        }

                default:
                    cmd = cmd.Replace("GO_", "");
                    return $"{prepend}{cmd}{(Arguments.Length > 0 ? (" " + String.Join(" ", Arguments)) : "")};";
            }
        }

        public bool RequiresTestTrue() => CommandType is 
            CommandType.RESERVED_GO_SKIPT or 
            CommandType.RESERVED_GO_GOTOT or 
            CommandType.GO_BRANCHTRUE or 
            CommandType.GO_SKIPTRUE;
        public bool RequiresTestFalse() => CommandType is 
            CommandType.RESERVED_GO_SKIPF or 
            CommandType.RESERVED_GO_GOTOF or 
            CommandType.GO_BRANCHFALSE or 
            CommandType.GO_SKIPFALSE;

        public bool UsesLabelOffsets 
        {
            get 
            {
                switch (CommandType) 
                {
                    case CommandType.RESERVED_GO_GOTO:
                    case CommandType.RESERVED_GO_GOTOF:
                    case CommandType.RESERVED_GO_GOTOT:
                    case CommandType.RESERVED_GO_GOSUB:
                    case CommandType.RESERVED_GO_SKIP:
                    case CommandType.RESERVED_GO_SKIPF:
                    case CommandType.RESERVED_GO_SKIPT:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public static int GetArgumentsCount(Ray1EngineVersion engineVersion, CommandType type, byte[] args)
        {
            switch (type)
            {
                case CommandType.GO_LEFT:
                case CommandType.GO_RIGHT:
                case CommandType.GO_WAIT:
                case CommandType.GO_UP:
                case CommandType.GO_DOWN:
                case CommandType.GO_SUBSTATE:
                case CommandType.GO_SKIP:
                case CommandType.GO_ADD:
                case CommandType.GO_STATE:
                case CommandType.GO_PREPARELOOP:
                case CommandType.GO_LABEL:
                case CommandType.GO_GOTO:
                case CommandType.GO_GOSUB:
                case CommandType.GO_BRANCHTRUE:
                case CommandType.GO_BRANCHFALSE:
                case CommandType.GO_SETTEST:
                case CommandType.GO_WAITSTATE:
                case CommandType.RESERVED_GO_SKIP:
                case CommandType.RESERVED_GO_GOTO:
                case CommandType.RESERVED_GO_GOSUB:
                case CommandType.RESERVED_GO_GOTOT:
                case CommandType.RESERVED_GO_GOTOF:
                case CommandType.RESERVED_GO_SKIPT:
                case CommandType.RESERVED_GO_SKIPF:
                case CommandType.GO_NOP:
                case CommandType.GO_SKIPTRUE:
                case CommandType.GO_SKIPFALSE:
                case CommandType.INVALID_CMD:
                case CommandType.INVALID_CMD_DEMO:
                    return 1;

                case CommandType.GO_DOLOOP:
                case CommandType.GO_RETURN:
                    return engineVersion == Ray1EngineVersion.PS1_JPDemoVol3 ? 1 : 0;

                case CommandType.GO_TEST:
                    if (args == null || args.Length == 0)
                        return 2;

                    if (args[0] <= 4)
                        return 2;
                    else
                        return 1;

                case CommandType.GO_SPEED:
                    return 3;

                case CommandType.GO_X:
                case CommandType.GO_Y:
                    return 2;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}