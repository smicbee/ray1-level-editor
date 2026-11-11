namespace BinarySerializer.Ray1.PC
{
    public class GameInfo : BinarySerializable
    {
        public string LanguageFile { get; set; }
        public byte LanguageUsed { get; set; }
        public string TitleBackground { get; set; }
        public string LogoBackground { get; set; }
        public string BigRayBackground { get; set; }
        public string GeneralMenuBackground { get; set; }
        public string SaveMenuBackground { get; set; }
        public string OptionsMenuBackground { get; set; }
        public string CreditsMenuBackground { get; set; }
        public string ContinueBackground { get; set; }
        public string PerfectBonusBackground { get; set; }
        public string PerfectTimeBonusBackground { get; set; }
        public string FixSndBank { get; set; }
        public string EndGameVignette { get; set; }

        public int CreditsVignettesCount { get; set; }
        public string[] CreditsVignettes { get; set; }

        public byte DifficultyLevelsCount { get; set; }
        public string[] DifficultyLevelNames { get; set; }
        public byte[] DifficultyNumbers { get; set; }

        public ObjStateChange[] ObjStateChanges { get; set; } // For regional object graphics

        public override void SerializeImpl(SerializerObject s)
        {
            LanguageFile = s.SerializeString(LanguageFile, length: 9, name: nameof(LanguageFile));
            LanguageUsed = s.Serialize<byte>(LanguageUsed, name: nameof(LanguageUsed));
            TitleBackground = s.SerializeString(TitleBackground, length: 9, name: nameof(TitleBackground));
            LogoBackground = s.SerializeString(LogoBackground, length: 9, name: nameof(LogoBackground));
            BigRayBackground = s.SerializeString(BigRayBackground, length: 9, name: nameof(BigRayBackground));
            GeneralMenuBackground = s.SerializeString(GeneralMenuBackground, length: 9, name: nameof(GeneralMenuBackground));
            SaveMenuBackground = s.SerializeString(SaveMenuBackground, length: 9, name: nameof(SaveMenuBackground));
            OptionsMenuBackground = s.SerializeString(OptionsMenuBackground, length: 9, name: nameof(OptionsMenuBackground));
            CreditsMenuBackground = s.SerializeString(CreditsMenuBackground, length: 9, name: nameof(CreditsMenuBackground));
            ContinueBackground = s.SerializeString(ContinueBackground, length: 9, name: nameof(ContinueBackground));
            PerfectBonusBackground = s.SerializeString(PerfectBonusBackground, length: 9, name: nameof(PerfectBonusBackground));
            PerfectTimeBonusBackground = s.SerializeString(PerfectTimeBonusBackground, length: 9, name: nameof(PerfectTimeBonusBackground));
            FixSndBank = s.SerializeString(FixSndBank, length: 9, name: nameof(FixSndBank));
            EndGameVignette = s.SerializeString(EndGameVignette, length: 9, name: nameof(EndGameVignette));

            CreditsVignettesCount = s.Serialize<int>(CreditsVignettesCount, name: nameof(CreditsVignettesCount));
            CreditsVignettes = s.SerializeStringArray(CreditsVignettes, 8, length: 9, name: nameof(CreditsVignettes));

            DifficultyLevelsCount = s.Serialize<byte>(DifficultyLevelsCount, name: nameof(DifficultyLevelsCount));
            DifficultyLevelNames = s.SerializeStringArray(DifficultyLevelNames, 5, length: 10, name: nameof(DifficultyLevelNames));
            DifficultyNumbers = s.SerializeArray<byte>(DifficultyNumbers, 5, name: nameof(DifficultyNumbers));

            ObjStateChanges = s.SerializeObjectArray<ObjStateChange>(ObjStateChanges, 30, name: nameof(ObjStateChanges));
        }

        public class ObjStateChange : BinarySerializable
        {
            public ObjStateChangeEntry[] Entries { get; set; }
            public ObjType ObjType { get; set; }
            public short Count { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Entries = s.SerializeObjectArray<ObjStateChangeEntry>(Entries, 15, name: nameof(Entries));
                ObjType = s.Serialize<ObjType>(ObjType, name: nameof(ObjType));
                Count = s.Serialize<short>(Count, name: nameof(Count));
                s.SerializePadding(2, logIfNotNull: true);
            }
        }

        public class ObjStateChangeEntry : BinarySerializable
        {
            public short HitPoints { get; set; }
            public byte MainEtat { get; set; }
            public byte SubEtat { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                HitPoints = s.Serialize<short>(HitPoints, name: nameof(HitPoints));
                MainEtat = s.Serialize<byte>(MainEtat, name: nameof(MainEtat));
                s.SerializePadding(1);
                SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));
                s.SerializePadding(1);
            }
        }
    }
}