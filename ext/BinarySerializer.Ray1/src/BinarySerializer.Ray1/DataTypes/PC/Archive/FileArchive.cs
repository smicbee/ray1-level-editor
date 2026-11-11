using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinarySerializer.Ray1.PC
{
    /// <summary>
    /// Encrypted file archive data for PC
    /// </summary>
    public class FileArchive : BinarySerializable
    {
        public GameVersion GameVersion { get; set; }
        public FileArchiveEntry[] Entries { get; set; }

        public T ReadFile<T>(Context context, string fileName, Action<T> onPreSerialize = null)
            where T : BinarySerializable, new() => ReadFile<T>(context, Array.FindIndex(Entries, x => x.FileName == fileName), onPreSerialize);

        public T ReadFile<T>(Context context, int index, Action<T> onPreSerialize = null)
            where T : BinarySerializable, new()
        {
            // Make sure the index is not out of bounds
            if (index < 0 || index >= Entries.Length)
                return null;

            var s = context.Deserializer;
            var entry = Entries[index];
            T output = null;

            // Deserialize the file
            s.DoAt(Offset + entry.FileOffset, () =>
            {
                s.DoProcessed(new Xor8Processor(entry.XORKey), () => output = s.SerializeObject<T>(default, onPreSerialize, name: entry.FileName ?? index.ToString()));
            });

            return output;
        }

        public byte[] ReadFileBytes(Context context, string fileName) => ReadFileBytes(context, Array.FindIndex(Entries, x => x.FileName == fileName));

        public byte[] ReadFileBytes(Context context, int index)
        {
            // Make sure the index is not out of bounds
            if (index < 0 || index >= Entries.Length)
                return null;

            var s = context.Deserializer;
            var entry = Entries[index];
            byte[] output = null;

            // Deserialize the file
            s.DoAt(Offset + entry.FileOffset, () =>
            {
                s.DoProcessed(new Xor8Processor(entry.XORKey), () => output = s.SerializeArray<byte>(default, entry.FileSize, name: entry.FileName ?? index.ToString()));
            });

            return output;
        }

        public void RepackArchive(Context context, Dictionary<string, Action<SerializerObject>> fileWriter)
        {
            var settings = context.GetRequiredSettings<Ray1Settings>();

            if (settings.EngineVersion == Ray1EngineVersion.PC || 
                settings.EngineVersion == Ray1EngineVersion.PocketPC)
                throw new NotImplementedException("Repacking is not supported for Rayman 1"); // The header is in the exe

            // Get every file before we start writing
            foreach (var entry in Entries.Where(x => !fileWriter.ContainsKey(x.FileName)))
            {
                var data = new Array<byte>
                {
                    Value = ReadFileBytes(context, entry.FileName)
                };

                fileWriter.Add(entry.FileName, x => x.SerializeObject(data, name: entry.FileName));
            }

            var s = context.Serializer;

            // Keep track of the offset
            uint offset = (uint)((Entries.Length + 1) * 19 + 12);

            // Write every file
            foreach (var entry in Entries)
            {
                // Set the file offset
                entry.FileOffset = offset;
                var fileOffset = Offset + entry.FileOffset;
                s.Goto(fileOffset);

                // Calculate the checksum
                Checksum8Processor checksumProcessor = new();

                // Serialize file
                s.DoProcessed(checksumProcessor, () => fileWriter[entry.FileName](s));

                // Set the file size
                entry.FileSize = (uint)(s.CurrentPointer - fileOffset);
                
                // Remove xor encryption
                entry.XORKey = 0;

                // Set the checksum
                entry.Checksum = (byte)checksumProcessor.CalculatedValue;

                // Increment the offset by the size
                offset += entry.FileSize;
            }

            // Write the header
            s.DoAt(Offset, () =>
            {
                // Write PC header
                if (settings.IsVersioned)
                    GameVersion = s.SerializeObject<GameVersion>(GameVersion, name: nameof(GameVersion));

                // Write file entries
                s.SerializeObjectArray<FileArchiveEntry>(Entries, Entries.Length, name: nameof(Entries));

                // Write end file
                s.SerializeObject<FileArchiveEntry>(new FileArchiveEntry
                {
                    FileName = "ENDFILE"
                }, name: "ENDFILE");
            });
        }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            var settings = s.GetRequiredSettings<Ray1Settings>();

            // Read the header
            if (settings.IsVersioned)
                GameVersion = s.SerializeObject<GameVersion>(GameVersion, name: nameof(GameVersion));

            // For Rayman 1 the header is hard-coded in the game executable
            if (settings.EngineVersion == Ray1EngineVersion.PC || 
                settings.EngineVersion == Ray1EngineVersion.PocketPC)
            {
                if (s is BinarySerializer)
                    throw new Exception("Can't serialize Rayman 1 archive headers");

                var headerBytes = ArchiveHeaderTables.GetHeader(settings, Path.GetFileName(Offset.File.FilePath));
                var headerLength = headerBytes.Length / 12;

                using var headerStream = new MemoryStream(headerBytes);

                var key = $"{Offset}_Header";

                var file = new StreamFile(s.Context, key, headerStream);

                s.Context.AddFile(file);

                s.DoAt(s.Context.GetRequiredFile(key).StartPointer, () => 
                    Entries = s.SerializeObjectArray<FileArchiveEntry>(Entries, headerLength, name: nameof(Entries)));
            }
            else
            {
                Entries = s.SerializeObjectArrayUntil(
                    obj: Entries,
                    conditionCheckFunc: x => x.FileName == "ENDFILE",
                    getLastObjFunc: () => new FileArchiveEntry() { FileName = "ENDFILE" },
                    name: nameof(Entries));
            }
        }
    }
}