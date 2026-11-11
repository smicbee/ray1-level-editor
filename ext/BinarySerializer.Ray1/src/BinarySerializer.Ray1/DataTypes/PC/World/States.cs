namespace BinarySerializer.Ray1.PC
{
    /// <summary>
    /// ETA data for PC
    /// </summary>
    public class States : BinarySerializable
    {
        /// <summary>
        /// The obj states, order by Etat and SubEtat
        /// </summary>
        public ObjState[][] ObjectStates { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectStates = s.SerializeArraySize<ObjState[], byte>(ObjectStates, name: nameof(ObjectStates));

            s.DoArray(ObjectStates, (x, _, name) =>
            {
                x = s.SerializeArraySize<ObjState, byte>(x, name: name);
                return s.SerializeObjectArray<ObjState>(x, x.Length, name: name);
            }, name: nameof(ObjectStates));
        }
    }
}