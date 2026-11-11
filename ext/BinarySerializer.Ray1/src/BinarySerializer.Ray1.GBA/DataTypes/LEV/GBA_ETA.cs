using System.Linq;

namespace BinarySerializer.Ray1.GBA
{
    public class GBA_ETA : BinarySerializable
    {
        public byte[] Pre_Lengths { get; set; }

        public ObjState[][] ETA { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ETA ??= new ObjState[Pre_Lengths.Length][];

            for (int i = 0; i < ETA.Length; i++)
            {
                s.DoAt(s.SerializePointer(ETA[i]?.FirstOrDefault()?.Offset, name: $"EtatPointers[{i}]"), () =>
                {
                    ETA[i] = s.SerializeObjectArray<ObjState>(ETA[i], Pre_Lengths[i], name: $"{nameof(ETA)}[{i}]");
                });
            }
        }
    }
}