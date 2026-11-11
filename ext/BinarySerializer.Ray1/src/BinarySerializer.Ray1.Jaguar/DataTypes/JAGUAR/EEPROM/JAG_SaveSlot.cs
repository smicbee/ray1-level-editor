namespace BinarySerializer.Ray1.Jaguar
{
    public class JAG_SaveSlot : BinarySerializable
    {
        public string SaveName { get; set; } // 0x00006c96
        
        public byte ContinuesCount { get; set; } // 0x00006c84
        public byte LivesCount { get; set; } // 0x00006c60 NOTE: This is in BCD

        public JAG_SaveLevel PinkPlantWoods { get; set; } // 00006c2c
        public JAG_SaveLevel AnguishLagoon { get; set; } // 00006c2d
        public JAG_SaveLevel ForgottenSwamps { get; set; } // 00006c2e
        public JAG_SaveLevel MoskitosNest { get; set; } // 00006c2f
        public JAG_SaveLevel BongoHills { get; set; } // 0x00006c32
        public JAG_SaveLevel AllegroPresto { get; set; } // 0x00006c33
        public JAG_SaveLevel GongHeights { get; set; } // 0x00006c34
        public JAG_SaveLevel MrSaxsHullaballoo { get; set; } // 0x00006c35
        public JAG_SaveLevel TwilightGulch { get; set; } // 0x00006c38
        public JAG_SaveLevel TheHardRocks { get; set; } // 0x00006c39
        public JAG_SaveLevel MrStonesPeaks { get; set; } // 0x00006c3a
        public JAG_SaveLevel MrSkopsStalactites { get; set; } // 0x00006c46
        public JAG_SaveLevel EraserPlains { get; set; } // 0x00006c3c
        public JAG_SaveLevel PencilPentathlon { get; set; } // 0x00006c3d
        public JAG_SaveLevel CrystalPalace { get; set; } // 0x00006c44
        public JAG_SaveLevel EatatJoes { get; set; } // 0x00006c45
        public JAG_SaveLevel UnusedCakeLevel1 { get; set; } // 0x00006c48
        public JAG_SaveLevel UnusedCakeLevel2 { get; set; } // 0x00006c49

        // 00006c2b
        public bool UnlockedPinkPlantWoods { get; set; }
        public bool UnlockedAnguishLagoon { get; set; }
        public bool UnlockedForgottenSwamps { get; set; }
        public bool UnlockedMoskitosNest { get; set; }
        public bool UnlockedJungleSave { get; set; }

        // 00006c36
        public bool HelicoPower { get; set; }
        public bool GrabPower { get; set; }
        public bool UnusedPower { get; set; } // Unused, never gets checked for

        // 0x00006c31
        public bool UnlockedBongoHills { get; set; }
        public bool UnlockedAllegroPresto { get; set; }
        public bool UnlockedGongHeights { get; set; }
        public bool UnlockedMrSaxsHullaballoo { get; set; }
        public bool UnlockedMusicSave { get; set; }

        // 00006c42
        public bool FistPower { get; set; }
        public bool RunPower { get; set; }
        public bool HangPower { get; set; }

        // 0x00006c3b
        public bool UnlockedTwilightGulch { get; set; }
        public bool UnlockedTheHardRocks { get; set; }
        public bool UnlockedMrStonesPeaks { get; set; }
        public bool UnlockedMountainSave { get; set; }

        // 0x00006c37
        public bool UnlockedEraserPlains { get; set; }
        public bool UnlockedPencilPentathlon { get; set; }
        public bool UnlockedSpaceMamasCrater { get; set; }
        public bool UnlockedImageSave { get; set; }

        // 0x00006c47
        public bool UnlockedCrystalPalace { get; set; }
        public bool UnlockedEatatJoes { get; set; }
        public bool UnlockedMrSkopsStalactites { get; set; }
        public bool UnlockedCaveSave { get; set; }

        // 0x00006c43
        public bool UnlockedMrDarksChateau { get; set; }

        public JAG_SaveLevel SpaceMamasCrater { get; set; } // 0x00006c3e
        public byte UnusedValue { get; set; } // Doesn't get read by the game

        // 0x00006c4a
        public bool CompletedJungleBonus4 { get; set; }
        public bool CompletedJungleBonus3 { get; set; }
        public bool CompletedJungleBonus1 { get; set; }
        public bool CompletedJungleBonus2 { get; set; }
        public bool CompletedMusicBonus1 { get; set; }
        public bool CompletedMusicBonus2 { get; set; }
        public bool CompletedMountainBonus1 { get; set; }
        public bool CompletedMountainBonus2 { get; set; }
        public bool CompletedImageBonus1 { get; set; }
        public bool CompletedImageBonus2 { get; set; }
        public bool CompletedCaveBonus1 { get; set; }

        public byte InteractabilityCooldown { get; set; } // 0x00006c40 - probably not supposed to be included in the save

        // 0x00006c40
        public bool HelpedJoe { get; set; }
        public bool BeatenJungleBoss { get; set; }
        public bool BeatenMusicBoss { get; set; }
        public bool BeatenMountainBoss { get; set; }
        public bool BeatenImageBoss { get; set; }
        public bool BeatenCaveBoss { get; set; }
        public bool AllowReplayingBosses { get; set; } // Meant to be set after defeating Mr Dark?

        public override void SerializeImpl(SerializerObject s)
        {
            SaveName = s.SerializeString(SaveName, length: 4, name: SaveName);
            ContinuesCount = s.Serialize<byte>(ContinuesCount, name: nameof(ContinuesCount));
            LivesCount = s.Serialize<byte>(LivesCount, name: nameof(LivesCount));
            PinkPlantWoods = s.SerializeObject<JAG_SaveLevel>(PinkPlantWoods, name: nameof(PinkPlantWoods));
            AnguishLagoon = s.SerializeObject<JAG_SaveLevel>(AnguishLagoon, name: nameof(AnguishLagoon));
            ForgottenSwamps = s.SerializeObject<JAG_SaveLevel>(ForgottenSwamps, name: nameof(ForgottenSwamps));
            MoskitosNest = s.SerializeObject<JAG_SaveLevel>(MoskitosNest, name: nameof(MoskitosNest));
            BongoHills = s.SerializeObject<JAG_SaveLevel>(BongoHills, name: nameof(BongoHills));
            AllegroPresto = s.SerializeObject<JAG_SaveLevel>(AllegroPresto, name: nameof(AllegroPresto));
            GongHeights = s.SerializeObject<JAG_SaveLevel>(GongHeights, name: nameof(GongHeights));
            MrSaxsHullaballoo = s.SerializeObject<JAG_SaveLevel>(MrSaxsHullaballoo, name: nameof(MrSaxsHullaballoo));
            TwilightGulch = s.SerializeObject<JAG_SaveLevel>(TwilightGulch, name: nameof(TwilightGulch));
            TheHardRocks = s.SerializeObject<JAG_SaveLevel>(TheHardRocks, name: nameof(TheHardRocks));
            MrStonesPeaks = s.SerializeObject<JAG_SaveLevel>(MrStonesPeaks, name: nameof(MrStonesPeaks));
            MrSkopsStalactites = s.SerializeObject<JAG_SaveLevel>(MrSkopsStalactites, name: nameof(MrSkopsStalactites));
            EraserPlains = s.SerializeObject<JAG_SaveLevel>(EraserPlains, name: nameof(EraserPlains));
            PencilPentathlon = s.SerializeObject<JAG_SaveLevel>(PencilPentathlon, name: nameof(PencilPentathlon));
            CrystalPalace = s.SerializeObject<JAG_SaveLevel>(CrystalPalace, name: nameof(CrystalPalace));
            EatatJoes = s.SerializeObject<JAG_SaveLevel>(EatatJoes, name: nameof(EatatJoes));
            UnusedCakeLevel1 = s.SerializeObject<JAG_SaveLevel>(UnusedCakeLevel1, name: nameof(UnusedCakeLevel1));
            UnusedCakeLevel2 = s.SerializeObject<JAG_SaveLevel>(UnusedCakeLevel2, name: nameof(UnusedCakeLevel2));

            s.DoBits<byte>(b =>
            {
                UnlockedPinkPlantWoods = b.SerializeBits<bool>(UnlockedPinkPlantWoods, 1, name: nameof(UnlockedPinkPlantWoods));
                UnlockedAnguishLagoon = b.SerializeBits<bool>(UnlockedAnguishLagoon, 1, name: nameof(UnlockedAnguishLagoon));
                UnlockedForgottenSwamps = b.SerializeBits<bool>(UnlockedForgottenSwamps, 1, name: nameof(UnlockedForgottenSwamps));
                UnlockedMoskitosNest = b.SerializeBits<bool>(UnlockedMoskitosNest, 1, name: nameof(UnlockedMoskitosNest));
                UnlockedJungleSave = b.SerializeBits<bool>(UnlockedJungleSave, 1, name: nameof(UnlockedJungleSave));

                HelicoPower = b.SerializeBits<bool>(HelicoPower, 1, name: nameof(HelicoPower));
                GrabPower = b.SerializeBits<bool>(GrabPower, 1, name: nameof(GrabPower));
                UnusedPower = b.SerializeBits<bool>(UnusedPower, 1, name: nameof(UnusedPower));
            });
            s.DoBits<byte>(b =>
            {
                UnlockedBongoHills = b.SerializeBits<bool>(UnlockedBongoHills, 1, name: nameof(UnlockedBongoHills));
                UnlockedAllegroPresto = b.SerializeBits<bool>(UnlockedAllegroPresto, 1, name: nameof(UnlockedAllegroPresto));
                UnlockedGongHeights = b.SerializeBits<bool>(UnlockedGongHeights, 1, name: nameof(UnlockedGongHeights));
                UnlockedMrSaxsHullaballoo = b.SerializeBits<bool>(UnlockedMrSaxsHullaballoo, 1, name: nameof(UnlockedMrSaxsHullaballoo));
                UnlockedMusicSave = b.SerializeBits<bool>(UnlockedMusicSave, 1, name: nameof(UnlockedMusicSave));

                FistPower = b.SerializeBits<bool>(FistPower, 1, name: nameof(FistPower));
                RunPower = b.SerializeBits<bool>(RunPower, 1, name: nameof(RunPower));
                HangPower = b.SerializeBits<bool>(HangPower, 1, name: nameof(HangPower));
            });
            s.DoBits<byte>(b =>
            {
                UnlockedTwilightGulch = b.SerializeBits<bool>(UnlockedTwilightGulch, 1, name: nameof(UnlockedTwilightGulch));
                UnlockedTheHardRocks = b.SerializeBits<bool>(UnlockedTheHardRocks, 1, name: nameof(UnlockedTheHardRocks));
                UnlockedMrStonesPeaks = b.SerializeBits<bool>(UnlockedMrStonesPeaks, 1, name: nameof(UnlockedMrStonesPeaks));
                UnlockedMountainSave = b.SerializeBits<bool>(UnlockedMountainSave, 1, name: nameof(UnlockedMountainSave));
            });
            s.DoBits<byte>(b =>
            {
                UnlockedEraserPlains = b.SerializeBits<bool>(UnlockedEraserPlains, 1, name: nameof(UnlockedEraserPlains));
                UnlockedPencilPentathlon = b.SerializeBits<bool>(UnlockedPencilPentathlon, 1, name: nameof(UnlockedPencilPentathlon));
                UnlockedSpaceMamasCrater = b.SerializeBits<bool>(UnlockedSpaceMamasCrater, 1, name: nameof(UnlockedSpaceMamasCrater));
                UnlockedImageSave = b.SerializeBits<bool>(UnlockedImageSave, 1, name: nameof(UnlockedImageSave));
            });
            s.DoBits<byte>(b =>
            {
                UnlockedCrystalPalace = b.SerializeBits<bool>(UnlockedCrystalPalace, 1, name: nameof(UnlockedCrystalPalace));
                UnlockedEatatJoes = b.SerializeBits<bool>(UnlockedEatatJoes, 1, name: nameof(UnlockedEatatJoes));
                UnlockedMrSkopsStalactites = b.SerializeBits<bool>(UnlockedMrSkopsStalactites, 1, name: nameof(UnlockedMrSkopsStalactites));
                UnlockedCaveSave = b.SerializeBits<bool>(UnlockedCaveSave, 1, name: nameof(UnlockedCaveSave));
            });
            s.DoBits<byte>(b =>
            {
                UnlockedMrDarksChateau = b.SerializeBits<bool>(UnlockedMrDarksChateau, 1, name: nameof(UnlockedMrDarksChateau));
            });

            SpaceMamasCrater = s.SerializeObject<JAG_SaveLevel>(SpaceMamasCrater, name: nameof(SpaceMamasCrater));
            UnusedValue = s.Serialize<byte>(UnusedValue, name: nameof(UnusedValue));

            s.DoBits<ushort>(b =>
            {
                CompletedJungleBonus4 = b.SerializeBits<bool>(CompletedJungleBonus4, 1, name: nameof(CompletedJungleBonus4));
                CompletedJungleBonus3 = b.SerializeBits<bool>(CompletedJungleBonus3, 1, name: nameof(CompletedJungleBonus3));
                CompletedJungleBonus1 = b.SerializeBits<bool>(CompletedJungleBonus1, 1, name: nameof(CompletedJungleBonus1));
                CompletedJungleBonus2 = b.SerializeBits<bool>(CompletedJungleBonus2, 1, name: nameof(CompletedJungleBonus2));
                CompletedMusicBonus1 = b.SerializeBits<bool>(CompletedMusicBonus1, 1, name: nameof(CompletedMusicBonus1));
                CompletedMusicBonus2 = b.SerializeBits<bool>(CompletedMusicBonus2, 1, name: nameof(CompletedMusicBonus2));
                CompletedMountainBonus1 = b.SerializeBits<bool>(CompletedMountainBonus1, 1, name: nameof(CompletedMountainBonus1));
                CompletedMountainBonus2 = b.SerializeBits<bool>(CompletedMountainBonus2, 1, name: nameof(CompletedMountainBonus2));
                CompletedImageBonus1 = b.SerializeBits<bool>(CompletedImageBonus1, 1, name: nameof(CompletedImageBonus1));
                CompletedImageBonus2 = b.SerializeBits<bool>(CompletedImageBonus2, 1, name: nameof(CompletedImageBonus2));
                CompletedCaveBonus1 = b.SerializeBits<bool>(CompletedCaveBonus1, 1, name: nameof(CompletedCaveBonus1));
            });

            s.DoBits<byte>(b =>
            {
                HelpedJoe = b.SerializeBits<bool>(HelpedJoe, 1, name: nameof(HelpedJoe));
                BeatenJungleBoss = b.SerializeBits<bool>(BeatenJungleBoss, 1, name: nameof(BeatenJungleBoss));
                BeatenMusicBoss = b.SerializeBits<bool>(BeatenMusicBoss, 1, name: nameof(BeatenMusicBoss));
                BeatenMountainBoss = b.SerializeBits<bool>(BeatenMountainBoss, 1, name: nameof(BeatenMountainBoss));
                BeatenImageBoss = b.SerializeBits<bool>(BeatenImageBoss, 1, name: nameof(BeatenImageBoss));
                BeatenCaveBoss = b.SerializeBits<bool>(BeatenCaveBoss, 1, name: nameof(BeatenCaveBoss));
                AllowReplayingBosses = b.SerializeBits<bool>(AllowReplayingBosses, 1, name: nameof(AllowReplayingBosses));
            });

            InteractabilityCooldown = s.Serialize<byte>(InteractabilityCooldown, name: nameof(InteractabilityCooldown));
        }
    }

    public class JAG_SaveLevel : BinarySerializable
    {
        public bool Cage1 { get; set; }
        public bool Cage2 { get; set; }
        public bool Cage3 { get; set; }
        public bool Cage4 { get; set; }
        public bool Cage5 { get; set; }
        public bool Cage6 { get; set; }
        public bool Life1 { get; set; }
        public bool Life2 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoBits<byte>(b =>
            {
                Cage1 = b.SerializeBits<bool>(Cage1, 1, name: nameof(Cage1));
                Cage2 = b.SerializeBits<bool>(Cage2, 1, name: nameof(Cage2));
                Cage3 = b.SerializeBits<bool>(Cage3, 1, name: nameof(Cage3));
                Cage4 = b.SerializeBits<bool>(Cage4, 1, name: nameof(Cage4));
                Cage5 = b.SerializeBits<bool>(Cage5, 1, name: nameof(Cage5));
                Cage6 = b.SerializeBits<bool>(Cage6, 1, name: nameof(Cage6));
                Life1 = b.SerializeBits<bool>(Life1, 1, name: nameof(Life1));
                Life2 = b.SerializeBits<bool>(Life2, 1, name: nameof(Life2));
            });
        }
    }
}