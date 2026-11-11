using System.Collections.Generic;

namespace BinarySerializer.Ray1
{
    public class TextLocFile : Ray1TextSerializable
    {
        public string[] Strings { get; set; }

        public override void Read(Ray1TextParser parser)
        {
            var tempStrings = new List<string>();

            string value;

            // Read values into a temporary list
            while ((value = parser.ReadValue(true)) != null)
                tempStrings.Add(value);

            // Set strings
            Strings = tempStrings.ToArray();
        }
    }
}