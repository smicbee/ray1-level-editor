using System;
using System.IO;

namespace BinarySerializer.Ray1.Jaguar
{
    public class JAG_EEPROMEncoder : IStreamEncoder
    {
        public JAG_EEPROMEncoder(long length)
        {
            Length = length;
        }

        public long Length { get; }
        public string Name => "EEPROM";

        private void ReverseWords(Stream input, Stream output, bool throwOnEndOfStream)
        {
            const int wordSize = 2;

            byte[] buffer = new byte[Length];
            int read = input.Read(buffer, 0, buffer.Length);

            if (read != buffer.Length && throwOnEndOfStream)
                throw new EndOfStreamException();

            // The data is written reversed, 2 bytes at a time
            for (int i = 0; i < Length; i += wordSize)
                Array.Reverse(buffer, i, wordSize);

            output.Write(buffer, 0, buffer.Length);
        }

        public void DecodeStream(Stream input, Stream output) => ReverseWords(input, output, true);
        public void EncodeStream(Stream input, Stream output) => ReverseWords(input, output, false);
    }
}