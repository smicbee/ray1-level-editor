namespace BinarySerializer.Ray1.PS1
{
    public class BigRayData : BinarySerializable
    {
        public long Pre_Length { get; set; }

        public ObjData BigRay { get; set; }
        public byte[] DataBlock { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Pointer p = s.CurrentPointer;
            
            BigRay = s.SerializeObject<ObjData>(BigRay, name: nameof(BigRay));
            DataBlock = s.SerializeArray<byte>(DataBlock, Pre_Length - (s.CurrentPointer - p), name: nameof(DataBlock));
        }
    }
}