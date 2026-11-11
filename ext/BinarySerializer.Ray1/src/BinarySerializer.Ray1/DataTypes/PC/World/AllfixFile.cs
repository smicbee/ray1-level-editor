namespace BinarySerializer.Ray1.PC
{
    public class AllfixFile : BaseWorldFile
    {
        public uint RaymanExeCheckSum3 { get; set; }

        public uint DESIndex_Ray { get; set; }
        public uint DESIndex_Alpha { get; set; }
        public uint DESIndex_Alpha2 { get; set; }
        public uint DESIndex_Alpha3 { get; set; } // Japan/China only
        public uint DESIndex_RayLittle { get; set; }
        public uint DESIndex_MapObj { get; set; }
        public uint DESIndex_ClockObj { get; set; }
        public uint DESIndex_DivObj { get; set; }
        public uint DESIndex_Parchem { get; set; } // EDU/Kit only

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            // Serialize PC Header
            if (settings.IsVersioned)
                GameVersion = s.SerializeObject<GameVersion>(GameVersion, name: nameof(GameVersion));

            // Serialize the ETA
            Eta = s.SerializeArraySize<States, byte>(Eta, name: nameof(Eta));
            Eta = s.SerializeObjectArray<States>(Eta, Eta.Length, name: nameof(Eta));

            // Serialize the DES
            DesItemCount = s.Serialize<ushort>(DesItemCount, name: nameof(DesItemCount));

            // We need to read one less DES as DES 0 is not in this file
            DesItems = s.SerializeObjectArray<Design>(DesItems, DesItemCount - 1, onPreSerialize: data => data.Pre_Type = Design.Type.AllFix, name: nameof(DesItems));

            RaymanExeCheckSum3 = s.Serialize(RaymanExeCheckSum3, name: nameof(RaymanExeCheckSum3));

            DESIndex_Ray = s.Serialize<uint>(DESIndex_Ray, name: nameof(DESIndex_Ray));
            DESIndex_Alpha = s.Serialize<uint>(DESIndex_Alpha, name: nameof(DESIndex_Alpha));
            DESIndex_Alpha2 = s.Serialize<uint>(DESIndex_Alpha2, name: nameof(DESIndex_Alpha2));

            if (settings.PCVersion == Ray1PCVersion.PC_1_21_JP)
                DESIndex_Alpha3 = s.Serialize<uint>(DESIndex_Alpha3, name: nameof(DESIndex_Alpha3));

            DESIndex_RayLittle = s.Serialize<uint>(DESIndex_RayLittle, name: nameof(DESIndex_RayLittle));
            DESIndex_MapObj = s.Serialize<uint>(DESIndex_MapObj, name: nameof(DESIndex_MapObj));
            DESIndex_ClockObj = s.Serialize<uint>(DESIndex_ClockObj, name: nameof(DESIndex_ClockObj));
            DESIndex_DivObj = s.Serialize<uint>(DESIndex_DivObj, name: nameof(DESIndex_DivObj));

            if (settings.EngineVersion == Ray1EngineVersion.PC_Kit || 
                settings.EngineVersion == Ray1EngineVersion.PC_Edu ||
                settings.EngineVersion == Ray1EngineVersion.PS1_Edu || 
                settings.EngineVersion == Ray1EngineVersion.PC_Fan)
                DESIndex_Parchem = s.Serialize<uint>(DESIndex_Parchem, name: nameof(DESIndex_Parchem));
        }
    }
}