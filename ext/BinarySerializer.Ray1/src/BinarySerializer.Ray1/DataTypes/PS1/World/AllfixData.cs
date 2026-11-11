namespace BinarySerializer.Ray1.PS1
{
    public class AllfixData : BinarySerializable
    {
        public long Pre_Length { get; set; }

        public Alpha Alpha { get; set; }
        public Alpha Alpha2 { get; set; }

        public ObjData Ray { get; set; }
        public ObjData RayLittle { get; set; }
        public ObjData ClockObj { get; set; }
        public ObjData DivObj { get; set; }
        public ObjData[] MapObj { get; set; } // 24 medallions and 1 object for the worldmap circle border

        public byte[] DataBlock { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Pointer p = s.CurrentPointer;
            
            Alpha = s.SerializeObject<Alpha>(Alpha, name: nameof(Alpha));
            Alpha2 = s.SerializeObject<Alpha>(Alpha2, name: nameof(Alpha2));

            Ray = s.SerializeObject<ObjData>(Ray, name: nameof(Ray));
            RayLittle = s.SerializeObject<ObjData>(RayLittle, name: nameof(RayLittle));
            ClockObj = s.SerializeObject<ObjData>(ClockObj, name: nameof(ClockObj));
            DivObj = s.SerializeObject<ObjData>(DivObj, name: nameof(DivObj));
            MapObj = s.SerializeObjectArray<ObjData>(MapObj, 25, name: nameof(MapObj));

            DataBlock = s.SerializeArray<byte>(DataBlock, Pre_Length - (s.CurrentPointer - p), name: nameof(DataBlock));
        }
    }
}