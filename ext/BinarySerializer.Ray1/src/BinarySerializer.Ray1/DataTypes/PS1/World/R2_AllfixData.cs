namespace BinarySerializer.Ray1.PS1
{
    /// <summary>
    /// Allfix footer data for Rayman 2
    /// </summary>
    public class R2_AllfixData : BinarySerializable
    {
        public Pointer RaymanCharacterPointer { get; set; }
        public Pointer RaymanUserDataPointer { get; set; }
        public Pointer RaymanAnimSetPointer { get; set; }

        public uint DESIndex_Ray { get; set; }
        public uint DESIndex_Alpha { get; set; }
        public uint DESIndex_Png { get; set; }

        public FontTable[] Alpha { get; set; }
        public Pointer Pointer_36 { get; set; } // Sound bank related, includes a .vab file
        public byte SaveZoneWorldsCount { get; set; } // The amount of worlds to store in the save zone
        public byte[] SaveZoneLevelCounts { get; set; } // The amount of levels in each world for the save zone
        public RecordedDemos Demos { get; set; }

        // Serialized from pointers
        public Character RaymanCollisionData { get; set; }
        public byte[] RaymanUserData { get; set; }
        public AnimationSet RaymanAnimSet { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            RaymanCharacterPointer = s.SerializePointer(RaymanCharacterPointer, name: nameof(RaymanCharacterPointer));
            RaymanUserDataPointer = s.SerializePointer(RaymanUserDataPointer, name: nameof(RaymanUserDataPointer));
            RaymanAnimSetPointer = s.SerializePointer(RaymanAnimSetPointer, name: nameof(RaymanAnimSetPointer));

            DESIndex_Ray = s.Serialize<uint>(DESIndex_Ray, name: nameof(DESIndex_Ray));
            DESIndex_Alpha = s.Serialize<uint>(DESIndex_Alpha, name: nameof(DESIndex_Alpha));
            DESIndex_Png = s.Serialize<uint>(DESIndex_Png, name: nameof(DESIndex_Png));
            Alpha = s.SerializeObjectArray<FontTable>(Alpha, 5, name: nameof(Alpha));
            Pointer_36 = s.SerializePointer(Pointer_36, name: nameof(Pointer_36));
            SaveZoneWorldsCount = s.Serialize<byte>(SaveZoneWorldsCount, name: nameof(SaveZoneWorldsCount));
            SaveZoneLevelCounts = s.SerializeArray<byte>(SaveZoneLevelCounts, 7, name: nameof(SaveZoneLevelCounts));
            Demos = s.SerializeObject<RecordedDemos>(Demos, name: nameof(Demos));

            s.DoAt(RaymanAnimSetPointer, () => 
                RaymanAnimSet = s.SerializeObject<AnimationSet>(RaymanAnimSet, name: nameof(RaymanAnimSet)));
            s.DoAt(RaymanUserDataPointer, () =>
                RaymanUserData = s.SerializeArray<byte>(RaymanUserData, R2_ObjType.TYPE_RAYMAN.GetUserDataLength(), name: nameof(RaymanUserData)));
            s.DoAt(RaymanCharacterPointer, () => 
                RaymanCollisionData = s.SerializeObject<Character>(RaymanCollisionData, name: nameof(RaymanCollisionData)));
        }
    }
}