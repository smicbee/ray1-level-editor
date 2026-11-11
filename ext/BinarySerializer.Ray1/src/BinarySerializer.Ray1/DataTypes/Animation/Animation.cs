using System.Collections.Generic;

namespace BinarySerializer.Ray1
{
    /// <summary>
    /// A sprite animation, defining its layers for each frame
    /// </summary>
    public class Animation : BinarySerializable
    {
        public Pointer LayersPointer { get; set; }
        public Pointer FramesPointer { get; set; }
        public Pointer SoundEventsPointer { get; set; }

        public uint PCPacked_LayersPointer { get; set; }
        public uint PCPacked_FramesPointer { get; set; }

        public ushort LayersCount { get; set; }
        public byte SpeedXValue { get; set; } // Some value used for setting horizontal obj speed
        public ushort FramesCount { get; set; }
        public byte SoundEventKeyFramesCount { get; set; }

        public AnimationLayer[] Layers { get; set; }
        public byte[] PS1EDU_CompressedLayers { get; set; }
        public Pointer<AnimationLayer[]>[] R2_Layers { get; set; }
        public AnimationFrame DefaultFrame { get; set; } // Unused
        public AnimationFrame[] Frames { get; set; }
        public SoundEventKeyFrame[] SoundEventKeyFrames { get; set; }

        public void PS1EDU_UncompressLayers(AnimationLayer[] commonLayers)
        {
            // In the PS1 EDU games the animation layers are compressed to save memory. Common
            // animations are reused from a global array (one for FIX and one for WORLD).

            List<AnimationLayer> layers = new();
            int offset = 0;

            while (offset < PS1EDU_CompressedLayers.Length)
            {
                if (PS1EDU_CompressedLayers[offset] < 2)
                {
                    layers.Add(new AnimationLayer()
                    {
                        FlipX = PS1EDU_CompressedLayers[offset + 0] == 1,
                        XPosition = PS1EDU_CompressedLayers[offset + 1],
                        YPosition = PS1EDU_CompressedLayers[offset + 2],
                        SpriteIndex = PS1EDU_CompressedLayers[offset + 3],
                    });

                    offset += 4;
                }
                else
                {
                    layers.Add(commonLayers[PS1EDU_CompressedLayers[offset] - 2]);
                    offset++;
                }
            }

            Layers = layers.ToArray();
        }

        public override void SerializeImpl(SerializerObject s) 
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            // Rayman 2 adds sound events
            if (settings.EngineVersion == Ray1EngineVersion.R2_PS1)
            {
                // Serialize pointers
                LayersPointer = s.SerializePointer(LayersPointer, name: nameof(LayersPointer));
                FramesPointer = s.SerializePointer(FramesPointer, name: nameof(FramesPointer));
                SoundEventsPointer = s.SerializePointer(SoundEventsPointer, name: nameof(SoundEventsPointer));

                // Serialize values
                s.DoBits<ushort>(b =>
                {
                    LayersCount = b.SerializeBits<ushort>(LayersCount, 14, name: nameof(LayersCount));
                    SpeedXValue = b.SerializeBits<byte>(SpeedXValue, 2, name: nameof(SpeedXValue));
                });
                FramesCount = s.Serialize<byte>((byte)FramesCount, name: nameof(FramesCount));
                SoundEventKeyFramesCount = s.Serialize<byte>(SoundEventKeyFramesCount, name: nameof(SoundEventKeyFramesCount));

                // Serialize layers
                s.DoAt(LayersPointer, () =>
                {
                    // Serialize the layer pointers
                    R2_Layers = s.SerializePointerArray<AnimationLayer[]>(R2_Layers, FramesCount, name: nameof(R2_Layers));

                    foreach (Pointer<AnimationLayer[]> layersPointer in R2_Layers)
                        layersPointer.ResolveObjectArray(s, LayersCount);
                });

                // Serialize frames
                s.DoAt(FramesPointer, () => Frames = s.SerializeObjectArray<AnimationFrame>(Frames, FramesCount, name: nameof(Frames)));

                // Serialize sound events
                s.DoAt(SoundEventsPointer, () =>
                    SoundEventKeyFrames = s.SerializeObjectArray<SoundEventKeyFrame>(SoundEventKeyFrames, SoundEventKeyFramesCount, name: nameof(SoundEventKeyFrames)));
            }
            else if (settings.EngineBranch is Ray1EngineBranch.SNES or Ray1EngineBranch.Jaguar)
            {
                // TODO: Does the padding here actually contain data?
                // Serialize data
                FramesCount = s.Serialize<byte>((byte)FramesCount, name: nameof(FramesCount));
                s.SerializePadding(1);
                LayersCount = s.Serialize<byte>((byte)LayersCount, name: nameof(LayersCount));
                s.SerializePadding(1);

                // Serialize data from pointers
                Layers = s.SerializeObjectArray(Layers, LayersCount * FramesCount, name: nameof(Layers));
            }
            // The default engine format for an animation in Rayman 1
            else if (!settings.IsLoadingPackedPCData)
            {
                // Serialize pointers
                LayersPointer = s.SerializePointer(LayersPointer, name: nameof(LayersPointer));
                FramesPointer = s.SerializePointer(FramesPointer, name: nameof(FramesPointer));

                // Serialize data
                s.DoBits<ushort>(b =>
                {
                    LayersCount = b.SerializeBits<ushort>(LayersCount, 14, name: nameof(LayersCount));
                    SpeedXValue = b.SerializeBits<byte>(SpeedXValue, 2, name: nameof(SpeedXValue));
                });
                FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));

                // Serialize data from pointers
                s.DoAt(LayersPointer, () => Layers = s.SerializeObjectArray(Layers, LayersCount * FramesCount, name: nameof(Layers)));
                s.DoAt(FramesPointer, () => Frames = s.SerializeObjectArray(Frames, FramesCount, name: nameof(Frames)));
            }
            else
            {
                // The data is stored like the default engine format, but with pointers being filled in later
                if (settings.EngineVersion == Ray1EngineVersion.PS1_Edu)
                {
                    // Serialize pointers (get filled during runtime)
                    PCPacked_LayersPointer = s.Serialize<uint>(PCPacked_LayersPointer, name: nameof(PCPacked_LayersPointer));
                    PCPacked_FramesPointer = s.Serialize<uint>(PCPacked_FramesPointer, name: nameof(PCPacked_FramesPointer));

                    // Serialize data
                    s.DoBits<ushort>(b =>
                    {
                        LayersCount = b.SerializeBits<ushort>(LayersCount, 14, name: nameof(LayersCount));
                        SpeedXValue = b.SerializeBits<byte>(SpeedXValue, 2, name: nameof(SpeedXValue));
                    });
                    FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));
                }
                // For the remaining PC versions the data is packed after the header without any real pointers
                else
                {
                    s.DoBits<ushort>(b =>
                    {
                        LayersCount = b.SerializeBits<ushort>(LayersCount, 14, name: nameof(LayersCount));
                        SpeedXValue = b.SerializeBits<byte>(SpeedXValue, 2, name: nameof(SpeedXValue));
                    });
                    FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));

                    PCPacked_FramesPointer = s.Serialize<uint>(PCPacked_FramesPointer, name: nameof(PCPacked_FramesPointer));

                    s.DoProcessed(new DataLengthProcessor(), p =>
                    {
                        p.Serialize<ushort>(s, "FramesOffset");

                        Layers = s.SerializeObjectArray<AnimationLayer>(Layers, LayersCount * FramesCount, name: nameof(Layers));
                        DefaultFrame = s.SerializeObject<AnimationFrame>(DefaultFrame, name: nameof(DefaultFrame));
                    });

                    if (PCPacked_FramesPointer != 0xFFFFFFFF)
                        Frames = s.SerializeObjectArray<AnimationFrame>(Frames, FramesCount, name: nameof(Frames));
                }
            }
        }
    }
}