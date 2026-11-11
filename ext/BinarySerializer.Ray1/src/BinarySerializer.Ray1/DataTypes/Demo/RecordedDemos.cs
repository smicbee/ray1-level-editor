using System;

namespace BinarySerializer.Ray1
{
    public class RecordedDemos : BinarySerializable
    {
        public RayEvts SaveRayEvts { get; set; } // Used to store the flags for when in a demo
        public Pointer DemosPointer { get; set; }
        public int DefaultRuntime { get; set; }
        public int DefaultRuntimeFinished { get; set; }
        public int Runtime { get; set; }
        public short Temps { get; set; }
        public short DemosCount { get; set; }
        public int Mode { get; set; }

        // Serialized from pointers
        public Record[] Records { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            if (settings.EngineVersion == Ray1EngineVersion.R2_PS1)
            {
                SaveRayEvts = s.SerializeObject<RayEvts>(SaveRayEvts, name: nameof(SaveRayEvts));
                DemosPointer = s.SerializePointer(DemosPointer, name: nameof(DemosPointer));
                DefaultRuntime = s.Serialize<int>(DefaultRuntime, name: nameof(DefaultRuntime));
                DefaultRuntimeFinished = s.Serialize<int>(DefaultRuntimeFinished, name: nameof(DefaultRuntimeFinished));
                Runtime = s.Serialize<int>(Runtime, name: nameof(Runtime));
                Temps = s.Serialize<short>(Temps, name: nameof(Temps));
                DemosCount = s.Serialize<short>(DemosCount, name: nameof(DemosCount));
                Mode = s.Serialize<int>(Mode, name: nameof(Mode));

                s.DoAt(DemosPointer, () => Records = s.SerializeObjectArray<Record>(Records, DemosCount, name: nameof(Records)));
            }
            else if (settings.IsLoadingPackedPCData)
            {
                DemosCount = s.Serialize<byte>((byte)DemosCount, name: nameof(DemosCount));
                DefaultRuntime = s.Serialize<int>(DefaultRuntime, name: nameof(DefaultRuntime));
                DefaultRuntimeFinished = s.Serialize<int>(DefaultRuntimeFinished, name: nameof(DefaultRuntimeFinished));

                Records = s.SerializeObjectArray<Record>(Records, DemosCount, name: nameof(Records));
            }
            else
            {
                throw new NotSupportedException("Rayman 1 doesn't have an unpacked demos struct");
            }
        }
    }
}