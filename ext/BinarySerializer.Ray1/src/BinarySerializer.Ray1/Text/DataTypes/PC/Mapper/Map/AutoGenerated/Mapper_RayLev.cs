using System.Collections.Generic;

namespace BinarySerializer.Ray1
{
    /// <summary>
    /// RayLev data for the Mapper
    /// </summary>
    public class Mapper_RayLev : Ray1TextSerializable
    {
        /// <summary>
        /// The DES CMD manifest
        /// </summary>
        public Dictionary<string, string> DESManifest { get; set; }

        public override void Read(Ray1TextParser parser) 
        {
            // Create the dictionary
            DESManifest = new Dictionary<string, string>();

            string firstValue;
            while ((firstValue = parser.ReadValue()) != null)
                // Add the item
                DESManifest.Add(firstValue, parser.ReadValue());
        }
    }
}