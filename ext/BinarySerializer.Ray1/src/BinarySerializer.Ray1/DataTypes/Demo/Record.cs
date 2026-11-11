using System;

namespace BinarySerializer.Ray1
{
    public class Record : BinarySerializable
    {
        public RayEvts RayEvts { get; set; }
        public string LoadingVignette { get; set; }

        public short XPos { get; set; }
        public short YPos { get; set; }

        public byte World { get; set; }
        public byte Level { get; set; }

        public Pointer InputsPointer { get; set; }
        public int R2_InputsBufferLength { get; set; }

        public uint PCPacked_InputsPointer { get; set; }
        public int InputsBufferLength { get; set; }

        // TODO: Parse and decompress inputs. They're RLE encoded.
        public byte[] InputsBuffer { get; set; }
        public ushort[] R2_InputsBuffer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            if (settings.EngineVersion == Ray1EngineVersion.R2_PS1)
            {
                RayEvts = s.SerializeObject<RayEvts>(RayEvts, name: nameof(RayEvts));
                InputsPointer = s.SerializePointer(InputsPointer, name: nameof(InputsPointer));
                R2_InputsBufferLength = s.Serialize<int>(R2_InputsBufferLength, name: nameof(R2_InputsBufferLength));
                XPos = s.Serialize<short>(XPos, name: nameof(XPos));
                YPos = s.Serialize<short>(YPos, name: nameof(YPos));
                World = s.Serialize<byte>(World, name: nameof(World));
                Level = s.Serialize<byte>(Level, name: nameof(Level));
                s.SerializePadding(2, logIfNotNull: true);

                s.DoAt(InputsPointer, () => R2_InputsBuffer = s.SerializeArray<ushort>(R2_InputsBuffer, R2_InputsBufferLength * 2, name: nameof(R2_InputsBuffer)));
            }
            else if (settings.IsLoadingPackedPCData)
            {
                RayEvts = s.SerializeObject<RayEvts>(RayEvts, name: nameof(RayEvts));
                LoadingVignette = s.SerializeString(LoadingVignette, 9, name: nameof(LoadingVignette));
                World = s.Serialize<byte>(World, name: nameof(World));
                Level = s.Serialize<byte>(Level, name: nameof(Level));
                s.SerializePadding(2, logIfNotNull: true);
                InputsBufferLength = s.Serialize<int>(InputsBufferLength, name: nameof(InputsBufferLength));
                PCPacked_InputsPointer = s.Serialize<uint>(PCPacked_InputsPointer, name: nameof(PCPacked_InputsPointer));

                InputsBuffer = s.SerializeArray<byte>(InputsBuffer, InputsBufferLength, name: nameof(InputsBuffer));
            }
            else
            {
                throw new NotImplementedException("Not implemented unpacked Rayman 1 record struct");
            }
        }
    }
}