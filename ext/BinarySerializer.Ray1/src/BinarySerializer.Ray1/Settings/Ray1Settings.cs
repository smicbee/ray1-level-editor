using System;
using System.Text;

namespace BinarySerializer.Ray1
{
    public class Ray1Settings
    {
        public Ray1Settings(Ray1EngineVersion engineVersion, World world = World.Jungle, int level = 1, Ray1PCVersion pcVersion = Ray1PCVersion.None, string volume = null)
        {
            EngineVersion = engineVersion;
            World = world;
            Level = level;
            PCVersion = pcVersion;
            Volume = volume;

            switch (EngineVersion)
            {
                case Ray1EngineVersion.PS1:
                case Ray1EngineVersion.PS1_EUDemo:
                case Ray1EngineVersion.R2_PS1:
                case Ray1EngineVersion.PS1_JP:
                case Ray1EngineVersion.PS1_JPDemoVol3:
                case Ray1EngineVersion.PS1_JPDemoVol6:
                case Ray1EngineVersion.Saturn:
                    EngineBranch = Ray1EngineBranch.PS1;
                    break;

                case Ray1EngineVersion.PC:
                case Ray1EngineVersion.PocketPC:
                case Ray1EngineVersion.PC_Kit:
                case Ray1EngineVersion.PC_Fan:
                case Ray1EngineVersion.PC_Edu:
                case Ray1EngineVersion.PS1_Edu:
                    EngineBranch = Ray1EngineBranch.PC;
                    break;

                case Ray1EngineVersion.GBA:
                case Ray1EngineVersion.DSi:
                    EngineBranch = Ray1EngineBranch.GBA;
                    break;

                case Ray1EngineVersion.Jaguar:
                case Ray1EngineVersion.Jaguar_Proto:
                case Ray1EngineVersion.Jaguar_Demo:
                    EngineBranch = Ray1EngineBranch.Jaguar;
                    break;

                case Ray1EngineVersion.SNES:
                    EngineBranch = Ray1EngineBranch.SNES;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(engineVersion), engineVersion, null);
            }

            IsVersioned = EngineVersion == Ray1EngineVersion.PC_Kit ||
                          EngineVersion == Ray1EngineVersion.PC_Edu ||
                          EngineVersion == Ray1EngineVersion.PS1_Edu ||
                          EngineVersion == Ray1EngineVersion.PC_Fan;
            IsLoadingPackedPCData = EngineBranch == Ray1EngineBranch.PC;
        }

        public Ray1EngineVersion EngineVersion { get; }
        public Ray1EngineBranch EngineBranch { get; }
        public Ray1PCVersion PCVersion { get; }

        public bool IsVersioned { get; }

        public World World { get; set; }
        public int Level { get; set; }
        public string Volume { get; set; }

        /// <summary>
        /// This determines how the data is to be serialized. For most platforms the data is already unpacked, such
        /// as PS1 and Saturn. However on PC the data is packed in the files and then unpacked into memory.
        /// </summary>
        public bool IsLoadingPackedPCData { get; set; }

        public static Encoding DefaultEncoding => Encoding.GetEncoding(437);
    }
}
