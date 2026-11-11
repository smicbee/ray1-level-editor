using System;

namespace BinarySerializer.Ray1.PS1
{
    /// <summary>
    /// Base class for PS1 XXX packs. These files are packs of multiple
    /// files made to load multiple files from disc faster.
    /// </summary>
    public abstract class Pack : BinarySerializable
    {
        /// <summary>
        /// The amount of packed files
        /// </summary>
        public uint FilesCount { get; set; }

        /// <summary>
        /// The file pointers
        /// </summary>
        public Pointer[] FilePointers { get; set; }
        
        /// <summary>
        /// The total size of the file pack
        /// </summary>
        public uint PackSize { get; set; }

        public void SerializeFile(SerializerObject s, int file, Action<long> serializeAction)
        {
            s.DoAt(FilePointers[file], () =>
            {
                long length;

                if (file == FilesCount - 1)
                    length = PackSize - FilePointers[file].FileOffset;
                else
                    length = FilePointers[file + 1] - FilePointers[file];

                serializeAction(length);
            });
        }

        public override void SerializeImpl(SerializerObject s) 
        {
            Pointer baseAddress = s.CurrentPointer;
            FilesCount = s.Serialize<uint>(FilesCount, name: nameof(FilesCount));

            // Serialize the file pointers. These are file offset, so use an anchor.
            FilePointers = s.SerializePointerArray(FilePointers, FilesCount, anchor: baseAddress, name: nameof(FilePointers));
            PackSize = s.Serialize<uint>(PackSize, name: nameof(PackSize));
        }
    }
}