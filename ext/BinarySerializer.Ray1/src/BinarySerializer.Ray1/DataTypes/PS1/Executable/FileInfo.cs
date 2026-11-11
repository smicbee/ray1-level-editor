using BinarySerializer.PlayStation.PS1.Psyq;

namespace BinarySerializer.Ray1.PS1
{
    public class FileInfo : BinarySerializable
    {
        public Pointer FilePathPointer { get; set; }
        public uint MemoryAddress { get; set; } // Retail units have access to memory ranges between 0x80000000 and 0x801FFFFF
        public uint DevMemoryAddress { get; set; } // Dev units have access to memory ranges between 0x80000000 and 0x807FFFFF
        public CdlFILE File { get; set; }

        public string FilePath { get; set; }
        public string ProcessedFilePath { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            if (settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol3 ||
                settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol6)
            {
                FilePath = s.SerializeString(FilePath, 32, name: nameof(FilePath));
                MemoryAddress = s.Serialize<uint>(MemoryAddress, name: nameof(MemoryAddress));
                DevMemoryAddress = s.Serialize<uint>(DevMemoryAddress, name: nameof(DevMemoryAddress));
                File = s.SerializeObject<CdlFILE>(File, x => x.Pre_HasFileName = false, name: nameof(File));

                // The demos use earlier Psyq versions where there seems to be padding instead of a file name
                s.SerializePadding(settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol6 ? 12 : 8);
            }
            else
            {
                FilePathPointer = s.SerializePointer(FilePathPointer, name: nameof(FilePathPointer));
                MemoryAddress = s.Serialize<uint>(MemoryAddress, name: nameof(MemoryAddress));
                DevMemoryAddress = s.Serialize<uint>(DevMemoryAddress, name: nameof(DevMemoryAddress));
                File = s.SerializeObject<CdlFILE>(File, name: nameof(File));

                s.DoAt(FilePathPointer, () => FilePath = s.SerializeString(FilePath, name: nameof(FilePath)));
            }

            ProcessedFilePath = FilePath.Replace('\\', '/').Replace(";1", "").TrimStart('/');

            if (settings.EngineVersion == Ray1EngineVersion.PS1_EUDemo || 
                settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol3 ||
                settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol6)
                ProcessedFilePath = ProcessedFilePath.Replace("RAY/", "");
        }
    }
}