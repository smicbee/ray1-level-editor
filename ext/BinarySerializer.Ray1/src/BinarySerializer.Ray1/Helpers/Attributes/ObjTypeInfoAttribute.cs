using System;

namespace BinarySerializer.Ray1
{
    /// <summary>
    /// Attribute for object types
    /// </summary>
    public sealed class ObjTypeInfoAttribute : Attribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="flag">The object flag</param>
        public ObjTypeInfoAttribute(ObjTypeFlag flag)
        {
            Flag = flag;
        }

        /// <summary>
        /// The object type flag
        /// </summary>
        public ObjTypeFlag Flag { get; }
    }
}