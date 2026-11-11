namespace BinarySerializer.Ray1
{
    /// <summary>
    /// Collision for a rotated object. This is filled out based off of the sprite and existing collision.
    /// </summary>
    public class RotationCollision : BinarySerializable
    {
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public int X3 { get; set; }
        public int Y3 { get; set; }
        public int X4 { get; set; }
        public int Y4 { get; set; }
        public int Int_20 { get; set; } // What is this? Defaults to -1.

        public override void SerializeImpl(SerializerObject s)
        {
            X1 = s.Serialize<int>(X1, name: nameof(X1));
            Y1 = s.Serialize<int>(Y1, name: nameof(Y1));
            X2 = s.Serialize<int>(X2, name: nameof(X2));
            Y2 = s.Serialize<int>(Y2, name: nameof(Y2));
            X3 = s.Serialize<int>(X3, name: nameof(X3));
            Y3 = s.Serialize<int>(Y3, name: nameof(Y3));
            X4 = s.Serialize<int>(X4, name: nameof(X4));
            Y4 = s.Serialize<int>(Y4, name: nameof(Y4));
            Int_20 = s.Serialize<int>(Int_20, name: nameof(Int_20));
        }
    }
}