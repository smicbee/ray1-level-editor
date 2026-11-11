namespace BinarySerializer.Ray1
{
    public class HelpDefine : BinarySerializable
    {
        public HelpDefineEntry[][] Entries { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Entries = s.InitializeArray(Entries, 32);
            s.DoArray(Entries, (x, _, name) => s.SerializeObjectArray<HelpDefineEntry>(x, 5, name: name), name: nameof(Entries));
        }
    }
}