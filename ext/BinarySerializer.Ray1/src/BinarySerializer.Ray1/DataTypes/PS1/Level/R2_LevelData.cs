using System;
using System.Linq;

namespace BinarySerializer.Ray1.PS1
{
    /// <summary>
    /// Level data for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class R2_LevelData : BinarySerializable
    {
        public RayEvts RayEvts { get; set; }
        public RayEvts RayEvtsToChange { get; set; } // The values to use from the previous field
        public Level Level { get; set; }
        public Pointer ZDCDataPointer { get; set; }
        public Pointer ZDCArray1Pointer { get; set; } // Indexed using ZDCEntry.ZDCIndex
        public Pointer ZDCArray2Pointer { get; set; } // Indexed using ZDCEntry.ZDCIndex and additional value
        public Pointer ZDCArray3Pointer { get; set; } // Indexed using UnkPointer5 values
        public short[] ObjectLinkTable { get; set; }
        public short[] ActivatedObjects { get; set; }
        public RotationCollision[] RotationCollision { get; set; }

        // Serialized from pointers
        public ZDCBox[] ZDC { get; set; }
        public ZDC_TriggerFlags[] ZDCTriggerFlags { get; set; }
        public ushort[] ZDCArray2 { get; set; }
        public ZDCUnknownData[] ZDCArray3 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            RayEvts = s.SerializeObject<RayEvts>(RayEvts, name: nameof(RayEvts));
            RayEvtsToChange = s.SerializeObject<RayEvts>(RayEvtsToChange, name: nameof(RayEvtsToChange));
            Level = s.SerializeObject<Level>(Level, name: nameof(Level));
            
            ZDCDataPointer = s.SerializePointer(ZDCDataPointer, name: nameof(ZDCDataPointer));
            ZDCArray1Pointer = s.SerializePointer(ZDCArray1Pointer, name: nameof(ZDCArray1Pointer));
            ZDCArray2Pointer = s.SerializePointer(ZDCArray2Pointer, name: nameof(ZDCArray2Pointer));
            ZDCArray3Pointer = s.SerializePointer(ZDCArray3Pointer, name: nameof(ZDCArray3Pointer));

            ObjectLinkTable = s.SerializeArray<short>(ObjectLinkTable, Level.ObjectsCount, name: nameof(ObjectLinkTable));
            s.Align(logIfNotNull: true);
            ActivatedObjects = s.SerializeArray<short>(ActivatedObjects, Level.ObjectsCount, name: nameof(ActivatedObjects));
            s.Align(logIfNotNull: true);
            int rotCollisionCount = Level.R2_Objects.Where(x => x.HasRotationCollision).Max(x => x.RotationCollisionIndex) + 1;
            RotationCollision = s.SerializeObjectArray<RotationCollision>(RotationCollision, rotCollisionCount, name: nameof(RotationCollision));

            // Serialize data from pointers
            s.DoAt(ZDCDataPointer, () => 
                ZDC = s.SerializeObjectArray<ZDCBox>(ZDC, 237, name: nameof(ZDC)));
            s.DoAt(ZDCArray1Pointer, () => 
                ZDCTriggerFlags = s.SerializeArray<ZDC_TriggerFlags>(ZDCTriggerFlags, 237, name: nameof(ZDCTriggerFlags)));
            s.DoAt(ZDCArray2Pointer, () => 
                ZDCArray2 = s.SerializeArray<ushort>(ZDCArray2, 474, name: nameof(ZDCArray2)));
            s.DoAt(ZDCArray3Pointer, () => 
                ZDCArray3 = s.SerializeObjectArray<ZDCUnknownData>(ZDCArray3, 16, name: nameof(ZDCArray3)));
        }

        [Flags]
        public enum ZDC_TriggerFlags : byte
        {
            None = 0,

            Flag_0 = 1 << 0,

            Rayman = 1 << 1,
            Poing_0 = 1 << 2,
            
            Flag_3 = 1 << 3,
            Flag_4 = 1 << 4,
            Flag_5 = 1 << 5,
            Flag_6 = 1 << 6,

            Poing_1 = 1 << 7
        }
    }
}