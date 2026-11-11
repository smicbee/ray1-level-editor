namespace BinarySerializer.Ray1.PC
{
    /// <summary>
    /// The config data for Rayman 1 on PC
    /// </summary>
    public class ConfigFile : BinarySerializable
    {
        public Language Language { get; set; }

        public uint Port { get; set; }
        public uint Irq { get; set; }
        public uint Dma { get; set; }
        public uint Param { get; set; }
        public uint DeviceID { get; set; }
        public byte NumCard { get; set; }

        // Keys are indexed as 0-3
        public ushort JumpKey { get; set; }
        public ushort FistKey { get; set; }
        public ushort UnusedKey { get; set; }
        public ushort ActionKey { get; set; }

        public ushort MusicVolume { get; set; }
        public ushort SoundVolume { get; set; } // Set as (127 * value / 20), 0-20
        public ushort SteroEnabled { get; set; } // 0 = Mono, 1 = Stereo
        public ushort VoicesVolume { get; set; }
        
        public bool Mode_Pad { get; set; } // Indicates if the controller setup screen has been shown
        public byte Port_Pad { get; set; }

        public short XPadMax { get; set; }
        public short XPadMin { get; set; }
        public short YPadMax { get; set; }
        public short YPadMin { get; set; }
        public short XPadCentre { get; set; }
        public short YPadCentre { get; set; }

        public byte[] NotBut { get; set; }
        public byte[] Tab_Key { get; set; } // Left, up, right, down, jump, fist, action

        public byte GameModeVideo { get; set; } // pci1 = 0, pci2 = 1
        public byte P486 { get; set; } // pci1 or pci2 = 0, vesa = 1
        public byte SizeScreen { get; set; } // 4/4 = 0, 3/4 = 1, 2/4 = 2, 1/4 = 3
        public Freq Frequence { get; set; }

        public bool FixOn { get; set; } // Scores enabled
        public bool BackgroundOptionOn { get; set; }
        public bool ScrollDiffOn { get; set; }

        public ushort[] RefRam2VramNormalFix { get; set; }
        public ushort[] RefRam2VramNormal { get; set; }
        public ushort[] RefTransFondNormal { get; set; }
        public ushort[] RefSpriteNormal { get; set; }
        public ushort[] RefRam2VramX { get; set; }
        public ushort[] RefVram2VramX { get; set; }
        public ushort[] RefSpriteX { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Get the settings
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            if (settings.EngineVersion is Ray1EngineVersion.PC or Ray1EngineVersion.PocketPC)
                Language = s.Serialize<Language>(Language, name: nameof(Language));

            Port = s.Serialize<uint>(Port, name: nameof(Port));
            Irq = s.Serialize<uint>(Irq, name: nameof(Irq));
            Dma = s.Serialize<uint>(Dma, name: nameof(Dma));
            Param = s.Serialize<uint>(Param, name: nameof(Param));
            DeviceID = s.Serialize<uint>(DeviceID, name: nameof(DeviceID));
            NumCard = s.Serialize<byte>(NumCard, name: nameof(NumCard));
            JumpKey = s.Serialize<ushort>(JumpKey, name: nameof(JumpKey));
            FistKey = s.Serialize<ushort>(FistKey, name: nameof(FistKey));
            UnusedKey = s.Serialize<ushort>(UnusedKey, name: nameof(UnusedKey));
            ActionKey = s.Serialize<ushort>(ActionKey, name: nameof(ActionKey));

            MusicVolume = s.Serialize<ushort>(MusicVolume, name: nameof(MusicVolume));
            SoundVolume = s.Serialize<ushort>(SoundVolume, name: nameof(SoundVolume));
            SteroEnabled = s.Serialize<ushort>(SteroEnabled, name: nameof(SteroEnabled));
            if (settings.EngineVersion is Ray1EngineVersion.PC_Edu or Ray1EngineVersion.PC_Kit or Ray1EngineVersion.PC_Fan)
                VoicesVolume = s.Serialize<ushort>(VoicesVolume, name: nameof(VoicesVolume));

            Mode_Pad = s.Serialize<bool>(Mode_Pad, name: nameof(Mode_Pad));
            Port_Pad = s.Serialize<byte>(Port_Pad, name: nameof(Port_Pad));

            XPadMax = s.Serialize<short>(XPadMax, name: nameof(XPadMax));
            XPadMin = s.Serialize<short>(XPadMin, name: nameof(XPadMin));
            YPadMax = s.Serialize<short>(YPadMax, name: nameof(YPadMax));
            YPadMin = s.Serialize<short>(YPadMin, name: nameof(YPadMin));
            XPadCentre = s.Serialize<short>(XPadCentre, name: nameof(XPadCentre));
            YPadCentre = s.Serialize<short>(YPadCentre, name: nameof(YPadCentre));

            NotBut = s.SerializeArray<byte>(NotBut, 4, name: nameof(NotBut));
            Tab_Key = s.SerializeArray<byte>(Tab_Key, 7, name: nameof(Tab_Key));

            GameModeVideo = s.Serialize<byte>(GameModeVideo, name: nameof(GameModeVideo));
            P486 = s.Serialize<byte>(P486, name: nameof(P486));
            SizeScreen = s.Serialize<byte>(SizeScreen, name: nameof(SizeScreen));
            Frequence = s.Serialize<Freq>(Frequence, name: nameof(Frequence));
            FixOn = s.Serialize<bool>(FixOn, name: nameof(FixOn));
            BackgroundOptionOn = s.Serialize<bool>(BackgroundOptionOn, name: nameof(BackgroundOptionOn));
            ScrollDiffOn = s.Serialize<bool>(ScrollDiffOn, name: nameof(ScrollDiffOn));

            RefRam2VramNormalFix = s.SerializeArray<ushort>(RefRam2VramNormalFix, 8, name: nameof(RefRam2VramNormalFix));
            RefRam2VramNormal = s.SerializeArray<ushort>(RefRam2VramNormal, 8, name: nameof(RefRam2VramNormal));
            RefTransFondNormal = s.SerializeArray<ushort>(RefTransFondNormal, 8, name: nameof(RefTransFondNormal));

            RefSpriteNormal = s.SerializeArray<ushort>(RefSpriteNormal, 2, name: nameof(RefSpriteNormal));
            RefRam2VramX = s.SerializeArray<ushort>(RefRam2VramX, 2, name: nameof(RefRam2VramX));
            RefVram2VramX = s.SerializeArray<ushort>(RefVram2VramX, 2, name: nameof(RefVram2VramX));
            RefSpriteX = s.SerializeArray<ushort>(RefSpriteX, 2, name: nameof(RefSpriteX));
        }
    }
}