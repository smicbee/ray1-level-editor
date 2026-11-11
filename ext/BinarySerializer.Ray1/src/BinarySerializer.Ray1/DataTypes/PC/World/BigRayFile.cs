namespace BinarySerializer.Ray1.PC
{
    public class BigRayFile : BaseWorldFile
    {
        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            // Serialize PC Header
            if (settings.IsVersioned)
                GameVersion = s.SerializeObject<GameVersion>(GameVersion, name: nameof(GameVersion));

            // Hard-code to 1 item
            DesItemCount = 1;

            // Serialize the DES
            DesItems = s.SerializeObjectArray<Design>(DesItems, DesItemCount, onPreSerialize: data => data.Pre_Type = Design.Type.BigRay, name: nameof(DesItems));

            // Serialize the ETA
            Eta = s.SerializeArraySize<States, byte>(Eta, name: nameof(Eta));
            Eta = s.SerializeObjectArray<States>(Eta, Eta.Length, name: nameof(Eta));
        }
    }
}