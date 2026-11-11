namespace BinarySerializer.Ray1
{
    // Since Mapper levels are raw non-compiled levels it doesn't use label offsets. Any command utilizing these is replaced by
    // its counterpart which doesn't. Then when the level is compiled it is "optimized" by having the label offsets be calculated
    // and used instead.

    /// <summary>
    /// The object command types
    /// </summary>
    public enum CommandType : byte
    {
        /// <summary>
        /// Self-handled
        /// </summary>
        GO_LEFT = 0x0,

        /// <summary>
        /// Self-handled
        /// </summary>
        GO_RIGHT = 0x1,

        /// <summary>
        /// Self-handled
        /// </summary>
        GO_WAIT = 0x2,

        /// <summary>
        /// Self-handled
        /// </summary>
        GO_UP = 0x3,

        /// <summary>
        /// Self-handled
        /// </summary>
        GO_DOWN = 0x4,

        /// <summary>
        /// Set sub etat to {arg1}
        /// </summary>
        GO_SUBSTATE = 0x5,

        /// <summary>
        /// Skips {arg1} arguments
        /// </summary>
        GO_SKIP = 0x6,

        /// <summary>
        /// Unused, does nothing
        /// </summary>
        GO_ADD = 0x7,

        /// <summary>
        /// Set main etat to {arg1}
        /// </summary>
        GO_STATE = 0x8,

        /// <summary>
        /// Saves the current command position and prepares a loop which loops {arg1} times
        /// </summary>
        GO_PREPARELOOP = 0x9,

        /// <summary>
        /// Perform the loop
        /// </summary>
        GO_DOLOOP = 0xA,
        
        /// <summary>
        /// Defines label {arg1}
        /// </summary>
        GO_LABEL = 0xB,

        /// <summary>
        /// Skips to label {arg1}
        /// </summary>
        GO_GOTO = 0xC,

        /// <summary>
        /// Saves the current command position and starts executing from label {arg1} until <see cref="GO_RETURN"/>
        /// </summary>
        GO_GOSUB = 0xD,

        /// <summary>
        /// Returns from a <see cref="GO_GOSUB"/> command execution
        /// </summary>
        GO_RETURN = 0xE,

        /// <summary>
        /// Skips to label {arg1} if TEST flag is true
        /// </summary>
        GO_BRANCHTRUE = 0xF,

        /// <summary>
        /// Skips to label {arg1} if TEST flag is false
        /// </summary>
        GO_BRANCHFALSE = 0x10,

        /// <summary>
        /// Sets the TEST flag depending on {arg1}. If {arg1} is less than 5 then {arg2} is used in the validation, otherwise only one argument is expected.
        /// </summary>
        GO_TEST = 0x11,

        /// <summary>
        /// Sets the TEST flag to {arg1}
        /// </summary>
        GO_SETTEST = 0x12,
        
        /// <summary>
        /// Waits for the animation to play {arg1} times. The command gets set to <see cref="GO_WAIT"/> while waiting.
        /// </summary>
        GO_WAITSTATE = 0x13,

        /// <summary>
        /// Self-handled. Usually {arg2} is speed x and {arg3} is speed y
        /// </summary>
        GO_SPEED = 0x14,
        
        /// <summary>
        /// Sets the x position to {arg1}{arg2}
        /// </summary>
        GO_X = 0x15,

        /// <summary>
        /// Sets the y position to {arg1}{arg2}
        /// </summary>
        GO_Y = 0x16,

        /// <summary>
        /// Skips to label offset index {arg1}
        /// </summary>
        RESERVED_GO_SKIP = 0x17,

        /// <summary>
        /// Same as <see cref="RESERVED_GO_SKIP"/>
        /// </summary>
        RESERVED_GO_GOTO = 0x18,

        /// <summary>
        /// Saves the current command position and starts executing from label offset index {arg1} until <see cref="GO_RETURN"/>
        /// </summary>
        RESERVED_GO_GOSUB = 0x19,

        /// <summary>
        /// Skips to label offset index {arg1} if TEST is true
        /// </summary>
        RESERVED_GO_GOTOT = 0x1A,

        /// <summary>
        /// Skips to label offset index {arg1} if TEST is false
        /// </summary>
        RESERVED_GO_GOTOF = 0x1B,

        /// <summary>
        /// Same as <see cref="RESERVED_GO_GOTOT"/>
        /// </summary>
        RESERVED_GO_SKIPT = 0x1C,

        /// <summary>
        /// Same as <see cref="RESERVED_GO_GOTOF"/>
        /// </summary>
        RESERVED_GO_SKIPF = 0x1D,

        /// <summary>
        /// Self-handled
        /// </summary>
        GO_NOP = 0x1E,

        /// <summary>
        /// Skips {arg1} commands if TEST is true
        /// </summary>
        GO_SKIPTRUE = 0x1F,

        /// <summary>
        /// Skips {arg1} commands if TEST is false
        /// </summary>
        GO_SKIPFALSE = 0x20,

        /// <summary>
        /// Terminates the commands and loops back, {arg1} is always 0xFF which is used as the command array terminator
        /// </summary>
        INVALID_CMD = 0x21,

        /// <summary>
        /// Same as <see cref="INVALID_CMD"/>. Used in the earlier PS1 demos.
        /// </summary>
        INVALID_CMD_DEMO = 0x42
    }
}