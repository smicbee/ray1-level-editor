namespace BinarySerializer.Ray1
{
    public class StatusBar : BinarySerializable
    {
        public short LivesCount { get; set; }
        public byte[] LivesDigits { get; set; }

        public byte[] HitPointSprites { get; set; }

        public byte TingsCount { get; set; }
        public byte[] TingsDigits { get; set; }

        /// <summary>
        /// The current maximum health count (always one less)
        /// </summary>
        public byte MaxHealth { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            if (settings.EngineVersion == Ray1EngineVersion.R2_PS1)
            {
                LivesCount = s.Serialize<short>(LivesCount, name: nameof(LivesCount));
                LivesDigits = s.SerializeArray<byte>(LivesDigits, 2, name: nameof(LivesDigits));
                MaxHealth = s.Serialize<byte>(MaxHealth, name: nameof(MaxHealth));
                TingsCount = s.Serialize<byte>(TingsCount, name: nameof(TingsCount));
                TingsDigits = s.SerializeArray<byte>(TingsDigits, 2, name: nameof(TingsDigits));
            }
            else
            {
                LivesCount = s.Serialize<short>(LivesCount, name: nameof(LivesCount));
                LivesDigits = s.SerializeArray<byte>(LivesDigits, 2, name: nameof(LivesDigits));
                HitPointSprites = s.SerializeArray<byte>(HitPointSprites, 2, name: nameof(HitPointSprites));
                TingsCount = s.Serialize<byte>(TingsCount, name: nameof(TingsCount));
                TingsDigits = s.SerializeArray<byte>(TingsDigits, 2, name: nameof(TingsDigits));
                MaxHealth = s.Serialize<byte>(MaxHealth, name: nameof(MaxHealth));
            }
        }
    }
}