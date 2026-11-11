namespace BinarySerializer.Ray1.PC
{
    /// <summary>
    /// Base world data for PC
    /// </summary>
    public abstract class BaseWorldFile : BinarySerializable
    {
        public GameVersion GameVersion { get; set; }

        /// <summary>
        /// The amount of DES items
        /// </summary>
        public ushort DesItemCount { get; set; }

        /// <summary>
        /// The DES items
        /// </summary>
        public Design[] DesItems { get; set; }

        /// <summary>
        /// The ETA items
        /// </summary>
        public States[] Eta { get; set; }
    }
}