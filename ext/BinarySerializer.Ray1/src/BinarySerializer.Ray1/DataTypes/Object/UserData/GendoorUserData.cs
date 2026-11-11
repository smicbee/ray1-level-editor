using System;

namespace BinarySerializer.Ray1
{
    public class GendoorUserData : BinarySerializable
    {
        public Pointer LinkedObjectsPointer { get; set; }
        public Pointer TriggerObjectsPointer { get; set; } // Objects that can trigger the gendoor when in range. We don't show these in the editor right now since they're unused in the prototype, even though the engine supports them.

        public ushort LinkedObjectsCount { get; set; }
        public ushort TriggerObjectsCount { get; set; }
        public GendoorFlags Flags_0 { get; set; } // Might be something different here?
        public GendoorFlags Flags_1 { get; set; }
        public byte RuntimeHasTriggered { get; set; } // Keeps track if the gendoor has triggered during runtime

        public short[] LinkedObjects { get; set; }
        public short[] TriggerObjects { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LinkedObjectsPointer = s.SerializePointer(LinkedObjectsPointer, name: nameof(LinkedObjectsPointer));
            TriggerObjectsPointer = s.SerializePointer(TriggerObjectsPointer, name: nameof(TriggerObjectsPointer));
            LinkedObjectsCount = s.Serialize<ushort>(LinkedObjectsCount, name: nameof(LinkedObjectsCount));
            TriggerObjectsCount = s.Serialize<ushort>(TriggerObjectsCount, name: nameof(TriggerObjectsCount));
            Flags_0 = s.Serialize<GendoorFlags>(Flags_0, name: nameof(Flags_0));
            Flags_1 = s.Serialize<GendoorFlags>(Flags_1, name: nameof(Flags_1));
            RuntimeHasTriggered = s.Serialize<byte>(RuntimeHasTriggered, name: nameof(RuntimeHasTriggered));
            s.SerializePadding(1);

            s.DoAt(LinkedObjectsPointer, () => 
                LinkedObjects = s.SerializeArray<short>(LinkedObjects, LinkedObjectsCount, name: nameof(LinkedObjects)));
            s.DoAt(TriggerObjectsPointer, () => 
                TriggerObjects = s.SerializeArray<short>(TriggerObjects, TriggerObjectsCount, name: nameof(TriggerObjects)));
        }

        [Flags]
        public enum GendoorFlags : byte
        {
            None = 0,

            TriggeredByRayman = 1 << 0,
            TriggeredByPoing = 1 << 1, // Triggered by Rayman's fist
            MultiTriggered = 1 << 2, // Indicates if the gendoor can be triggered multiple times
        }
    }
}