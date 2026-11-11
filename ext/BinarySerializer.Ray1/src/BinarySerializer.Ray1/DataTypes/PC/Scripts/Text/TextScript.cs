using System;

namespace BinarySerializer.Ray1.PC
{
    /// <summary>
    /// Binary text script
    /// </summary>
    public class TextScript : BinarySerializable
    {
        public byte LanguagesCount { get; set; } // NombreLangues
        public byte LanguageUsed { get; set; } // LangueUtilisee
        public KeyboardType KeyboardType { get; set; }

        public string[] LanguageNames { get; set; }

        public uint TextDefineCount { get; set; }
        public uint PCPacked_TextDefinePointer { get; set; }

        public string[] PCPacked_TextDefine { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            if (!settings.IsLoadingPackedPCData)
                throw new NotImplementedException("Not implemented serializing unpacked text data");

            LanguagesCount = s.Serialize<byte>(LanguagesCount, name: nameof(LanguagesCount));
            LanguageUsed = s.Serialize<byte>(LanguageUsed, name: nameof(LanguageUsed));
            KeyboardType = s.Serialize<KeyboardType>(KeyboardType, name: nameof(KeyboardType));

            // Most versions have 3 languages, but sometimes the NumberOfLanguages is set to 1
            // because only 1 is available. Other versions may have up to 5.
            int numLangNames = Math.Min(5, Math.Max(3, (int)LanguagesCount));

            if (settings.EngineVersion == Ray1EngineVersion.PS1_Edu && 
                (settings.Volume.StartsWith("IT") || settings.Volume.StartsWith("CS")))
                numLangNames = 5;

            if (settings.EngineVersion == Ray1EngineVersion.PC_Edu && (settings.Volume.StartsWith("HN") || 
                                                                       settings.Volume.StartsWith("IS") || 
                                                                       settings.Volume.StartsWith("NL") ||
                                                                       settings.Volume.StartsWith("PO") ||
                                                                       settings.Volume.StartsWith("KO") ||
                                                                       settings.Volume.StartsWith("CH") ||
                                                                       settings.Volume.StartsWith("CS") ||
                                                                       settings.Volume.StartsWith("CT") ||
                                                                       settings.Volume.StartsWith("IT")))
                numLangNames = 5;

            LanguageNames = s.SerializeStringArray(LanguageNames, numLangNames, 11, name: nameof(LanguageNames));

            s.Align(baseOffset: Offset);

            TextDefineCount = s.Serialize<uint>(TextDefineCount, name: nameof(TextDefineCount));
            PCPacked_TextDefinePointer = s.Serialize<uint>(PCPacked_TextDefinePointer, name: nameof(PCPacked_TextDefinePointer));

            PCPacked_TextDefine = s.SerializeLengthPrefixedStringArray<byte>(PCPacked_TextDefine, TextDefineCount, name: nameof(PCPacked_TextDefine));
        }
    }
}