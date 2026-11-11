namespace BinarySerializer.Ray1.PC
{
    /// <summary>
    /// Sound sample file names for voice sounds
    /// </summary>
    public class ChatSamples : BinarySerializable
    {
        public string Lost { get; set; } // Perdu
        public string BonusPerfect { get; set; }
        public string Quit { get; set; }
        public string Erase { get; set; }
        public string Level { get; set; } // Niveau
        public string QuitDos { get; set; }
        public string Encourage { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Lost = s.SerializeString(Lost, length: 9, name: nameof(Lost));
            BonusPerfect = s.SerializeString(BonusPerfect, length: 9, name: nameof(BonusPerfect));
            Quit = s.SerializeString(Quit, length: 9, name: nameof(Quit));
            Erase = s.SerializeString(Erase, length: 9, name: nameof(Erase));
            Level = s.SerializeString(Level, length: 9, name: nameof(Level));
            QuitDos = s.SerializeString(QuitDos, length: 9, name: nameof(QuitDos));
            Encourage = s.SerializeString(Encourage, length: 9, name: nameof(Encourage));
        }
    }
}