namespace BinarySerializer.Ray1.PC
{
    public class BigMap : BinarySerializable
    {
        public Pointer MapTilesPointer { get; set; }
        public Pointer MapTileTexturesPointersPointer { get; set; }
        public Pointer Pointer_08 { get; set; }
        public Pointer TileTexturesPointer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapTilesPointer = s.SerializePointer(MapTilesPointer, name: nameof(MapTilesPointer));
            MapTileTexturesPointersPointer = s.SerializePointer(MapTileTexturesPointersPointer, name: nameof(MapTileTexturesPointersPointer));
            Pointer_08 = s.SerializePointer(Pointer_08, name: nameof(Pointer_08));
            TileTexturesPointer = s.SerializePointer(TileTexturesPointer, name: nameof(TileTexturesPointer));
        }
    }
}