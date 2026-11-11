using System;
using System.Linq;
using System.Threading.Tasks;

namespace BinarySerializer.Ray1.PC.PS1EDU
{
    /// <summary>
    /// GRX bundle data for EDU on PS1
    /// </summary>
    public class GRX
    {
        /// <summary>
        /// The magic header
        /// </summary>
        public byte[] Magic { get; set; }

        /// <summary>
        /// The base offset
        /// </summary>
        public Pointer BaseOffset { get; set; }

        /// <summary>
        /// The amount of files in the bundle
        /// </summary>
        public uint FileCount { get; set; }

        /// <summary>
        /// The files
        /// </summary>
        public GRXFileEntry[] Files { get; set; }

        /// <summary>
        /// Gets the file with the specified file name
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <returns>The file</returns>
        public GRXFileEntry GetFile(string fileName) => Files?.FirstOrDefault(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)) ?? throw new Exception($"No matching file was found for name {fileName}");

        /// <summary>
        /// Gets the file bytes based on name
        /// </summary>
        /// <param name="s">The serializer object</param>
        /// <param name="fileName">The file name</param>
        /// <returns>The file bytes</returns>
        public async Task<byte[]> GetFileBytesAsync(SerializerObject s, string fileName)
        {
            // Attempt to find the file
            var file = GetFile(fileName);

            s.Goto(BaseOffset + file.FileOffset);

            await s.FillCacheForReadAsync(file.FileSize);

            // Read the bytes
            byte[] buffer = s.SerializeArray<byte>(default, file.FileSize, name: nameof(buffer));

            // Return the buffer
            return buffer;
        }

        public async Task SerializeHeaderAsync(SerializerObject s)
        {
            await s.FillCacheForReadAsync(8);
            
            Magic = s.SerializeArray<byte>(Magic, 4, name: nameof(Magic));
            BaseOffset = s.SerializePointer(BaseOffset, name: nameof(BaseOffset));

            await s.FillCacheForReadAsync(BaseOffset.FileOffset - 8);
            
            FileCount = s.Serialize<uint>(FileCount, name: nameof(FileCount));
            Files = s.SerializeObjectArray<GRXFileEntry>(Files, FileCount, name: nameof(Files));
        }
    }
}