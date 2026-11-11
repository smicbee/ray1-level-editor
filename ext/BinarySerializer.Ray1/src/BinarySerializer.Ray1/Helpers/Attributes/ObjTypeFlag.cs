namespace BinarySerializer.Ray1
{
    /// <summary>
    /// The object type
    /// </summary>
    public enum ObjTypeFlag
    {
        /// <summary>
        /// Normal object - appears in-game
        /// </summary>
        Normal,

        /// <summary>
        /// An always object - works together with a normal object
        /// </summary>
        Always,

        /// <summary>
        /// Editor object - does not appear in-game
        /// </summary>
        Editor
    }
}