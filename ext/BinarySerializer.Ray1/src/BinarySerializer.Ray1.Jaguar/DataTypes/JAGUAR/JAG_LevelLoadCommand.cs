using System.IO;

namespace BinarySerializer.Ray1.Jaguar
{
    /// <summary>
    /// Level load command for Rayman 1 (Jaguar)
    /// </summary>
    public class JAG_LevelLoadCommand : BinarySerializable
    {
        public LevelLoadCommandType Type { get; set; }

        // Arguments
        public uint UInt1 { get; set; }
        public uint UInt2 { get; set; }
        public short Short1 { get; set; }
        public short Short2 { get; set; }
        public short Short3 { get; set; }
        public Pointer PalettePointer { get; set; }
        public Pointer ImageBufferPointer { get; set; } // Compressed
        public uint ImageBufferMemoryPointer { get; set; } // Uncompressed data is loaded to this location
        public Pointer EventDefinitionDataMemoryPointer { get; set; } // full event definition array is copied to 0x001F9000
        public Pointer LevelMapBlockPointer { get; set; }
        public Pointer LevelEventBlockPointer { get; set; }
        public uint ImageBufferMemoryPointerPointer { get; set; } // The address of the image buffer in memory is written to this location. Is referenced in the event definition data.
        public uint TargetImageBufferMemoryPointer { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            var settings = s.GetRequiredSettings<Ray1Settings>();

            // Serialize the type
            Type = s.Serialize<LevelLoadCommandType>(Type, name: nameof(Type));

            // Make sure the type is valid
            if ((ushort)Type % 4 != 0 || (ushort)Type > 0x40)
                throw new InvalidDataException($"Level load command type {Type} at {Offset} is incorrect.");
            
            // Parse the data based on the type
            switch (Type) 
            {
                case LevelLoadCommandType.End:
                    break;

                case LevelLoadCommandType.PaletteDemo:
                    PalettePointer = s.SerializePointer(PalettePointer, name: nameof(PalettePointer));
                    UInt1 = s.Serialize<uint>(UInt1, name: nameof(UInt1));
                    Short1 = s.Serialize<short>(Short1, name: nameof(Short1));
                    break;

                case LevelLoadCommandType.Map:
                    UInt1 = s.Serialize<uint>(UInt1, name: nameof(UInt1));
                    LevelMapBlockPointer = s.SerializePointer(LevelMapBlockPointer, name: nameof(LevelMapBlockPointer));

                    if (settings.EngineVersion == Ray1EngineVersion.Jaguar_Proto)
                        Short1 = s.Serialize<short>(Short1, name: nameof(Short1));
                    else
                        LevelEventBlockPointer = s.SerializePointer(LevelEventBlockPointer, name: nameof(LevelEventBlockPointer));

                    break;

                case LevelLoadCommandType.Unzip:
                    // Used for vignettes/backgrounds/tiles
                    ImageBufferPointer = s.SerializePointer(ImageBufferPointer, name: nameof(ImageBufferPointer));
                    ImageBufferMemoryPointer = s.Serialize<uint>(ImageBufferMemoryPointer, name: nameof(ImageBufferMemoryPointer));
                    break;

                case LevelLoadCommandType.Fill:
                    UInt1 = s.Serialize<uint>(UInt1, name: nameof(UInt1));
                    UInt2 = s.Serialize<uint>(UInt2, name: nameof(UInt2));

                    if (settings.EngineVersion == Ray1EngineVersion.Jaguar_Proto)
                        Short1 = s.Serialize<short>(Short1, name: nameof(Short1));

                    break;

                case LevelLoadCommandType.SpritesProto:
                    // Used for sprites and graphics in the prototype
                    ImageBufferPointer = s.SerializePointer(ImageBufferPointer, name: nameof(ImageBufferPointer));
                    ImageBufferMemoryPointer = s.Serialize<uint>(ImageBufferMemoryPointer, name: nameof(ImageBufferMemoryPointer));
                    Short1 = s.Serialize<short>(Short1, name: nameof(Short1));
                    break;

                case LevelLoadCommandType.Copy:
                    ImageBufferMemoryPointer = s.Serialize<uint>(ImageBufferMemoryPointer, name: nameof(ImageBufferMemoryPointer));
                    TargetImageBufferMemoryPointer = s.Serialize<uint>(TargetImageBufferMemoryPointer, name: nameof(TargetImageBufferMemoryPointer));
                    Short1 = s.Serialize<short>(Short1, name: nameof(Short1));

                    if (settings.EngineVersion == Ray1EngineVersion.Jaguar_Proto)
                    {
                        Short2 = s.Serialize<short>(Short2, name: nameof(Short2));
                        Short3 = s.Serialize<short>(Short3, name: nameof(Short3));
                    }

                    break;

                case LevelLoadCommandType.Unk3:
                    Short1 = s.Serialize<short>(Short1, name: nameof(Short1));
                    break;

                case LevelLoadCommandType.UnkEventDef1:
                    Short1 = s.Serialize<short>(Short1, name: nameof(Short1));
                    Short2 = s.Serialize<short>(Short2, name: nameof(Short2));
                    EventDefinitionDataMemoryPointer = s.SerializePointer(EventDefinitionDataMemoryPointer, name: nameof(EventDefinitionDataMemoryPointer));
                    break;

                case LevelLoadCommandType.Sprites:
                    // Used for sprites
                    ImageBufferPointer = s.SerializePointer(ImageBufferPointer, name: nameof(ImageBufferPointer));
                    ImageBufferMemoryPointer = s.Serialize<uint>(ImageBufferMemoryPointer, name: nameof(ImageBufferMemoryPointer));
                    ImageBufferMemoryPointerPointer = s.Serialize<uint>(ImageBufferMemoryPointerPointer, name: nameof(ImageBufferMemoryPointerPointer));
                    break;

                case LevelLoadCommandType.UnkEventDef2:
                    Short1 = s.Serialize<short>(Short1, name: nameof(Short1));
                    Short2 = s.Serialize<short>(Short2, name: nameof(Short2));
                    Short3 = s.Serialize<short>(Short3, name: nameof(Short3));
                    EventDefinitionDataMemoryPointer = s.SerializePointer(EventDefinitionDataMemoryPointer, name: nameof(EventDefinitionDataMemoryPointer));
                    break;

                case LevelLoadCommandType.UnkEventDef3:
                    Short1 = s.Serialize<short>(Short1, name: nameof(Short1));
                    Short2 = s.Serialize<short>(Short2, name: nameof(Short2));
                    EventDefinitionDataMemoryPointer = s.SerializePointer(EventDefinitionDataMemoryPointer, name: nameof(EventDefinitionDataMemoryPointer));
                    break;

                case LevelLoadCommandType.Palette:
                    PalettePointer = s.SerializePointer(PalettePointer, name: nameof(PalettePointer));
                    break;

                case LevelLoadCommandType.NotImplemented3:
                case LevelLoadCommandType.NotImplemented4:
                case LevelLoadCommandType.NotImplemented5:
                case LevelLoadCommandType.NotImplemented6:
                    throw new InvalidDataException($"Level load command type {Type} at {Offset} is not yet implemented.");
            }
        }
        
        public enum LevelLoadCommandType : ushort
        {
            End = 0x00,
            PaletteDemo = 0x04, // "trans"
            Map = 0x08,
            Unzip = 0x0C, // Graphics
            Fill = 0x10,
            SpritesProto = 0x14, // "tranl"
            Copy = 0x18, // Move graphics
            Unk3 = 0x1C,
            UnkEventDef1 = 0x20,
            Sprites = 0x24,
            NotImplemented3 = 0x28,
            NotImplemented4 = 0x2C,

            // TODO: These probably load the multi-sprite data for the parallax scrolling sprites. Those verbe types reference tables with the sprite data.
            UnkEventDef2 = 0x30, // Pointer to code pointer
            UnkEventDef3 = 0x34, // Pointer to an empty entry with only first pointer and a code pointer.
            
            Palette = 0x38,
            NotImplemented5 = 0x3C,
            NotImplemented6 = 0x40,
        }
    }
}