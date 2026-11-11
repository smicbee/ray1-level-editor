namespace BinarySerializer.Ray1
{
    /// <summary>
    /// The save data for a <see cref="WorldInfo"/> entry
    /// </summary>
    public class WorldInfoSave : BinarySerializable
    {
        /// <summary>
        /// Indicates if the level has been unlocked on the world map
        /// </summary>
        public bool IsUnlocked { get; set; }

        /// <summary>
        /// Indicates if the level is currently unlocking
        /// </summary>
        public byte IsUnlocking { get; set; }

        /// <summary>
        /// The amount of cages in the level (0-6)
        /// </summary>
        public byte Cages { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoBits<byte>(b =>
            {
                IsUnlocked = b.SerializeBits<bool>(IsUnlocked, 1, name: nameof(IsUnlocked));
                IsUnlocking = b.SerializeBits<byte>(IsUnlocking, 1, name: nameof(IsUnlocking));
                Cages = b.SerializeBits<byte>(Cages, 3, name: nameof(Cages));
                b.SerializePadding(3, logIfNotNull: true);
            });
        }
    }
}