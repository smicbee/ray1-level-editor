using System;
using System.Linq;

namespace BinarySerializer.Ray1.PS1
{
    public class Executable : BinarySerializable
    {
        public ExecutableConfig Pre_PS1_Config { get; set; }

        public ZDCReference[] TypeZDC { get; set; }
        public ZDCBox[] ZDCData { get; set; }
        public ObjTypeFlags[] ObjTypeFlags { get; set; }
        public WorldInfo[] WorldInfo { get; set; }

        public byte[][] PS1_LevelBackgroundIndexTable { get; set; }
        // TODO: This should be multiple tables, one per file type
        public FileInfo[] PS1_FileTable { get; set; }

        public RGBA5551Color[] SAT_Palettes { get; set; }
        public string[][] SAT_FNDFileTable { get; set; }
        public string[][] SAT_FNDSPFileTable { get; set; }
        public byte[][] SAT_FNDIndexTable { get; set; }

        public int GetFileTypeIndex(ExecutableConfig config, FileType type) => Array.FindIndex(PS1_FileTable, x => x.Offset.AbsoluteOffset == config.FileTableInfos.FirstOrDefault(t => t.FileType == type)?.Offset);

        public override void SerializeImpl(SerializerObject s)
        {
            var settings = s.GetRequiredSettings<Ray1Settings>();

            s.DoAt(s.GetPreDefinedPointer(PS1_DefinedPointer.TypeZDC), () => 
                TypeZDC = s.SerializeObjectArray<ZDCReference>(TypeZDC, 256, name: nameof(TypeZDC)));

            s.DoAt(s.GetPreDefinedPointer(PS1_DefinedPointer.ZDCData), () => 
                ZDCData = s.SerializeObjectArray<ZDCBox>(ZDCData, 200, name: nameof(ZDCData)));

            s.DoAt(s.GetPreDefinedPointer(PS1_DefinedPointer.ObjTypeFlags), () => 
                ObjTypeFlags = s.SerializeObjectArray<ObjTypeFlags>(ObjTypeFlags, 256, name: nameof(ObjTypeFlags)));

            s.DoAt(s.GetPreDefinedPointer(PS1_DefinedPointer.WorldInfo), () => 
                WorldInfo = s.SerializeObjectArray<WorldInfo>(WorldInfo, 24, name: nameof(WorldInfo)));

            PS1_LevelBackgroundIndexTable ??= new byte[6][];

            s.DoAt(s.GetPreDefinedPointer(PS1_DefinedPointer.PS1_LevelBackgroundIndexTable), () =>
            {
                for (int i = 0; i < PS1_LevelBackgroundIndexTable.Length; i++)
                    PS1_LevelBackgroundIndexTable[i] = s.SerializeArray<byte>(PS1_LevelBackgroundIndexTable[i], 30, name: $"{nameof(PS1_LevelBackgroundIndexTable)}[{i}]");
            });

            if (settings.EngineVersion == Ray1EngineVersion.Saturn)
            {
                s.DoAt(s.GetRequiredPreDefinedPointer(PS1_DefinedPointer.SAT_Palettes), () => 
                    SAT_Palettes = s.SerializeObjectArray<RGBA5551Color>(SAT_Palettes, 25 * 256 * 2, name: nameof(SAT_Palettes)));

                SAT_FNDFileTable ??= new string[6][];

                s.DoAt(s.GetRequiredPreDefinedPointer(PS1_DefinedPointer.SAT_FndFileTable), () =>
                {
                    for (int i = 0; i < SAT_FNDFileTable.Length; i++)
                        SAT_FNDFileTable[i] = s.SerializeStringArray(SAT_FNDFileTable[i], 10, 12, name: $"{nameof(SAT_FNDFileTable)}[{i}]");
                });

                SAT_FNDSPFileTable ??= new string[6][];

                s.DoAt(s.GetRequiredPreDefinedPointer(PS1_DefinedPointer.SAT_FndSPFileTable), () =>
                {
                    for (int i = 0; i < SAT_FNDSPFileTable.Length; i++)
                        SAT_FNDSPFileTable[i] = s.SerializeStringArray(SAT_FNDSPFileTable[i], 5, 12, name: $"{nameof(SAT_FNDSPFileTable)}[{i}]");
                });

                SAT_FNDIndexTable ??= new byte[7][];

                s.DoAt(s.GetRequiredPreDefinedPointer(PS1_DefinedPointer.SAT_FndIndexTable), () =>
                {
                    for (int i = 0; i < SAT_FNDIndexTable.Length; i++)
                        SAT_FNDIndexTable[i] = s.SerializeArray<byte>(SAT_FNDIndexTable[i], 25, name: $"{nameof(SAT_FNDIndexTable)}[{i}]");
                });
            }
            else
            {
                if (Pre_PS1_Config == null)
                    throw new Exception($"{nameof(Pre_PS1_Config)} must be set before serializing");

                var fileTableInfos = Pre_PS1_Config.FileTableInfos;

                PS1_FileTable ??= new FileInfo[fileTableInfos.Sum(x => x.Count)];

                var index = 0;
                foreach (var info in fileTableInfos)
                {
                    s.DoAt(new Pointer(info.Offset, Offset.File), () =>
                    {
                        for (int i = 0; i < info.Count; i++)
                        {
                            PS1_FileTable[index] = s.SerializeObject<FileInfo>(PS1_FileTable[index], name: $"{nameof(PS1_FileTable)}_{info.FileType}[{i}]");
                            index++;
                        }
                    });
                }
            }
        }
    }
}