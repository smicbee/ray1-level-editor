using System;
using System.IO;
using System.Linq;

namespace BinarySerializer.Ray1.PC
{
    public class LevelTimeEncoder : IStreamEncoder
    {
        public LevelTimeEncoder(int world, int level, SaveRevision revision)
        {
            World = world;
            Level = level;
            Revision = revision;
        }

        public LevelTimeEncoder(Ray1Settings settings)
        {
            World = (int)settings.World;
            Level = settings.Level;
            Revision = settings.EngineVersion switch
            {
                Ray1EngineVersion.PC_Kit => SaveRevision.KIT,
                Ray1EngineVersion.PC_Fan => SaveRevision.FAN_60N,
                _ => throw new Exception($"Could not determine the save revision to use from the engine version {settings.EngineVersion}")
            };
        }

        public int World { get; }
        public int Level { get; }
        public SaveRevision Revision { get; }

        public string Name => nameof(LevelTimeEncoder);
        
        public void DecodeStream(Stream input, Stream output)
        {
            var buffer = new byte[17];
            input.Read(buffer, 0, buffer.Length);

            int xorIndex = World - 1 + 6 * (Level - 1);
            byte[] xorTable1 = Revision == SaveRevision.KIT ? XORTable1_KIT : XORTable1_FAN;
            byte[] xorTable2 = Revision == SaveRevision.KIT ? XORTable2_KIT : XORTable2_FAN;

            int ghostValue_3 = buffer[7] ^ xorTable2[xorIndex];
            int ghostValue_2 = buffer[4] ^ xorTable1[xorIndex];
            int ghostValue_1 = buffer[16] ^ xorTable1[xorIndex];
            int ghostValue_0 = buffer[1] ^ xorTable2[xorIndex];
            int ghostCheckValue = (buffer[15] ^ xorTable2[xorIndex]) - 11;

            byte checkValue = (byte)((buffer[10] ^ 0x4F) >> 1);
            int value_3 = buffer[9] ^ 0x7C;
            int value_2 = buffer[13] ^ 0x1A;
            int value_1 = buffer[3] ^ 0x63;
            int value_0 = buffer[5] ^ 0x6F;

            if (ghostCheckValue == checkValue &&
                ghostValue_3 == value_2 &&
                ghostValue_2 == value_3 &&
                ghostValue_1 == value_1 &&
                ghostValue_0 == value_0)
            {
                output.WriteByte((byte)value_2);
                output.WriteByte((byte)value_3);
                output.WriteByte((byte)value_1);
                output.WriteByte((byte)value_0);
            }
            else
            {
                // If it doesn't match we default to -1
                output.Write(Enumerable.Repeat((byte)0xFF, 4).ToArray(), 0, 4);
            }
        }

        public void EncodeStream(Stream input, Stream output)
        {
            int xorIndex = World - 1 + 6 * (Level - 1);
            byte[] xorTable1 = Revision == SaveRevision.KIT ? XORTable1_KIT : XORTable1_FAN;
            byte[] xorTable2 = Revision == SaveRevision.KIT ? XORTable2_KIT : XORTable2_FAN;

            var buffer = new byte[17];

            buffer[7] = buffer[13] = (byte)input.ReadByte();
            buffer[4] = buffer[9] = (byte)input.ReadByte();
            buffer[16] = buffer[3] = (byte)input.ReadByte();
            buffer[1] = buffer[5] = (byte)input.ReadByte();

            throw new NotImplementedException();
        }

        public enum SaveRevision
        {
            KIT,
            FAN_60N,
        }

        #region XOR Tables

        public byte[] XORTable1_KIT = new byte[]
        {
            0x41, 0x51, 0x57, 0x5A, 0x53, 0x58, 0x45, 0x44, 0x43, 0x52, 0x46, 0x56,
            0x54, 0x47, 0x42, 0x59, 0x48, 0x4E, 0x55, 0x4A, 0x49, 0x4B, 0x4F, 0x4C
        };

        public byte[] XORTable2_KIT = new byte[]
        {
            0x6D, 0x70, 0x6C, 0x6F, 0x6B, 0x69, 0x6A, 0x6E, 0x75, 0x68, 0x62, 0x79,
            0x67, 0x76, 0x74, 0x66, 0x63, 0x72, 0x64, 0x78, 0x65, 0x73, 0x77, 0x7A
        };

        // NOTE: Although each table is 24 bytes like in KIT these games have 40-60 levels, thus the game will read more bytes than are available. The additional bytes might be different depending on the release of the game. If so the validation check on the save should fail and it'll return -1.
        public byte[] XORTable1_FAN = new byte[]
        {
            0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46,
            0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46,
            0x00, 0x31, 0x31, 0x31, 0x31, 0x31, 0x31, 0x32, 0x32, 0x32, 0x32, 0x32,
            0x32, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x34, 0x34, 0x34, 0x34, 0x34,
            0x34, 0x00, 0x00, 0x00, 0x20, 0x07, 0x3F, 0x1F, 0x07, 0x3D, 0x20, 0x07
        };

        public byte[] XORTable2_FAN = new byte[]
        {
            0x31, 0x31, 0x31, 0x31, 0x31, 0x31, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32,
            0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x34, 0x34, 0x34, 0x34, 0x34, 0x34,
            0x00, 0x00, 0x00, 0x20, 0x07, 0x3F, 0x1F, 0x07, 0x3D, 0x20, 0x07, 0x3A,
            0x21, 0x07, 0x38, 0x24, 0x0A, 0x35, 0x25, 0x0C, 0x33, 0x2D, 0x0F, 0x30,
            0x34, 0x11, 0x2E, 0x2D, 0x0F, 0x30, 0x28, 0x0F, 0x33, 0x23, 0x0C, 0x35
        };

        #endregion
    }
}