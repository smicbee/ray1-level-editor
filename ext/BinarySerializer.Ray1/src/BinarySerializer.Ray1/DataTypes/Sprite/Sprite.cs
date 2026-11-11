using BinarySerializer.PlayStation.PS1;

namespace BinarySerializer.Ray1
{
    public class Sprite : BinarySerializable
    {
        // The offset in the image buffer where this sprite is located
        public int ImageBufferOffset { get; set; }

        // The ID. Treated as a signed 16-bit value in most versions where 0, or sometimes less
        // than 0, is invalid.
        public ushort? Id { get; set; }

        // The full image size, used for drawing
        public short Width { get; set; }
        public short Height { get; set; }

        // The inner image size and position, excluding any transparent padding. This is used
        // to calculate the sprite size in-game.
        public byte SpriteWidth { get; set; }
        public byte SpriteHeight { get; set; }
        public byte SpriteXPosition { get; set; }
        public byte SpriteYPosition { get; set; }

        // The image depth. Only 4-bit and 8-bit modes are used.
        public SpriteDepth Depth { get; set; }

        // The sub-palette index to use in a 256-color palette. This is only used for 4-bit
        // sprites in some versions when loading directly from palettes rather than from VRAM.
        public byte SubPaletteIndex { get; set; }

        // Horizontal flip (on PC it's a flag instead)
        public bool FlipX { get; set; }

        // Same as width on Saturn. On PS1 it's used to indicate if the sprite has been processed (0 == no, 0xFF == yes).
        public byte ScanlineWidth { get; set; }

        // Flags, used a bit different based on version
        public SpriteFlags Flags { get; set; }

        // Unknown unused values
        public byte UnusedValue { get; set; }

        // PS1 VRAM data
        public CBA Clut { get; set; }
        public TSB TexturePage { get; set; }
        public byte ImageOffsetInPageX { get; set; }
        public byte ImageOffsetInPageY { get; set; }

        // Jaguar
        public int JAG_Data { get; set; }
        public int JAG_Pitch { get; set; }
        public short JAG_DWidth { get; set; }
        public short JAG_IWidth { get; set; }
        public short JAG_Index { get; set; }
        public short JAG_Width { get; set; }
        public int JAG_Flags { get; set; }

        public bool IsDummySprite()
        {
            // Get the settings
            Ray1Settings settings = Context.GetRequiredSettings<Ray1Settings>();

            // Rayman 2 doesn't have dummy sprites
            if (settings.EngineVersion == Ray1EngineVersion.R2_PS1)
                return false;

            // If the id is null then the sprite is null. If the id is 0
            // then the sprite is a dummy sprite used in animations to
            // define an inactive layer. We also check the width and
            // height just in case something went wrong.
            return Width <= 0 || Height <= 0 || Id is 0 or null;
        }

        public override void SerializeImpl(SerializerObject s) 
        {
            Ray1Settings settings = s.GetRequiredSettings<Ray1Settings>();

            if (settings.EngineBranch == Ray1EngineBranch.Jaguar)
            {
                // This mostly matches the Jaguar Object struct, but with some added data. I'm unsure
                // if what's marked as padding is actually padding, but it seems to be unused. The
                // reason we can't set it to log if not null is because we often read invalid structs
                // due to not always knowing the correct array lengths for the sprites.
                s.DoBits<long>(b =>
                {
                    b.SerializePadding(14); // Type & YPos fields. Always 0.
                    Height = b.SerializeBits<short>(Height, 10, name: nameof(Height));
                    b.SerializePadding(19); // Link field. Always 0.
                    JAG_Data = b.SerializeBits<int>(JAG_Data, 21, name: nameof(JAG_Data));
                });
                s.DoBits<long>(b =>
                {
                    Id = b.SerializeNullableBits<ushort>(Id, 12, name: nameof(Id));
                    Depth = b.SerializeBits<SpriteDepth>(Depth, 3, name: nameof(Depth));
                    JAG_Pitch = b.SerializeBits<int>(JAG_Pitch, 3, name: nameof(JAG_Pitch));
                    JAG_DWidth = b.SerializeBits<short>(JAG_DWidth, 10, name: nameof(JAG_DWidth));
                    JAG_IWidth = b.SerializeBits<short>(JAG_IWidth, 10, name: nameof(JAG_IWidth));
                    JAG_Index = b.SerializeBits<short>(JAG_Index, 7, name: nameof(JAG_Index));

                    // TODO: Parse the flags. They probably match the Jaguar Object flags. Usually
                    //       this value is set to 4, which would be the transparency flag, which makes
                    //       sense. Unsure about the rest though since there are only 4 flags...
                    JAG_Flags = b.SerializeBits<int>(JAG_Flags, 6, name: nameof(JAG_Flags));

                    JAG_Width = b.SerializeBits<short>(JAG_Width, 13, name: nameof(JAG_Width));
                });

                ImageBufferOffset = JAG_Data * 8;
                s.Log("ImageBufferOffset: {0}", ImageBufferOffset);
                
                SubPaletteIndex = (byte)(JAG_Index >> 3);
                s.Log("SubPaletteIndex: {0}", SubPaletteIndex);
                
                Width = (short)(JAG_Width * 8);
                s.Log("Width: {0}", Width);

                SpriteWidth = (byte)Width;
                SpriteHeight = (byte)Height;
            }
            else if (settings.EngineBranch is Ray1EngineBranch.PC or Ray1EngineBranch.GBA)
            {
                ImageBufferOffset = s.Serialize<int>(ImageBufferOffset, name: nameof(ImageBufferOffset));
                Id = s.SerializeNullable<byte>((byte?)Id, name: nameof(Id));
                Width = s.Serialize<byte>((byte)Width, name: nameof(Width));
                Height = s.Serialize<byte>((byte)Height, name: nameof(Height));
                SpriteWidth = s.Serialize<byte>(SpriteWidth, name: nameof(SpriteWidth));
                SpriteHeight = s.Serialize<byte>(SpriteHeight, name: nameof(SpriteHeight));
                s.DoBits<byte>(b =>
                {
                    SpriteXPosition = b.SerializeBits<byte>(SpriteXPosition, 4, name: nameof(SpriteXPosition));
                    SpriteYPosition = b.SerializeBits<byte>(SpriteYPosition, 4, name: nameof(SpriteYPosition));
                });
                s.DoBits<byte>(b =>
                {
                    // Unused
                    Depth = b.SerializeBits<SpriteDepth>(Depth, 4, name: nameof(Depth));

                    // It gets read when rendering, but appears to remain unused
                    SubPaletteIndex = b.SerializeBits<byte>(SubPaletteIndex, 4, name: nameof(SubPaletteIndex));
                });

                Flags = s.Serialize<SpriteFlags>(Flags, name: nameof(Flags));
            }
            else if (settings.EngineBranch == Ray1EngineBranch.PS1)
            {
                if (settings.EngineVersion == Ray1EngineVersion.R2_PS1)
                {
                    Width = s.Serialize<byte>((byte)Width, name: nameof(Width));
                    Height = s.Serialize<byte>((byte)Height, name: nameof(Height));
                    SpriteWidth = s.Serialize<byte>(SpriteWidth, name: nameof(SpriteWidth));
                    SpriteHeight = s.Serialize<byte>(SpriteHeight, name: nameof(SpriteHeight));
                    s.DoBits<byte>(b =>
                    {
                        SpriteXPosition = b.SerializeBits<byte>(SpriteXPosition, 4, name: nameof(SpriteXPosition));
                        SpriteYPosition = b.SerializeBits<byte>(SpriteYPosition, 4, name: nameof(SpriteYPosition));
                    });
                    Flags = s.Serialize<SpriteFlags>(Flags, name: nameof(Flags));

                    // PS1 VRAM data
                    ImageOffsetInPageX = s.Serialize<byte>(ImageOffsetInPageX, name: nameof(ImageOffsetInPageX));
                    ImageOffsetInPageY = s.Serialize<byte>(ImageOffsetInPageY, name: nameof(ImageOffsetInPageY));
                    Clut = s.SerializeObject<CBA>(Clut, name: nameof(Clut));
                    TexturePage = s.SerializeObject<TSB>(TexturePage, name: nameof(TexturePage));
                }
                else if (settings.EngineVersion == Ray1EngineVersion.PS1_JP ||
                         settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol3 ||
                         settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol6 ||
                         settings.EngineVersion == Ray1EngineVersion.Saturn)
                {
                    // Only used on PS1 JP vol. 3 demo and Saturn
                    ImageBufferOffset = s.Serialize<int>(ImageBufferOffset, name: nameof(ImageBufferOffset));

                    // The game doesn't treat this as a 12-bit value, but rather as a signed 16-bit value it checks for being greater
                    // than 0, or sometimes not equal to 0. But it appears that the invalid values are all set to 0xFFF since the
                    // original Jaguar value uses 12 bits.
                    s.DoBits<ushort>(b =>
                    {
                        Id = b.SerializeNullableBits<ushort>(Id, 12, name: nameof(Id));
                        b.SerializePadding(4, logIfNotNull: true);
                    });

                    // Only used on PS1 JP vol. 3 demo and Saturn 
                    Depth = s.Serialize<SpriteDepth>(Depth, name: nameof(Depth));

                    Width = s.Serialize<short>(Width, name: nameof(Width));
                    Height = s.Serialize<short>(Height, name: nameof(Height));

                    // Only used on PS1 JP vol. 3 demo and Saturn
                    SubPaletteIndex = s.Serialize<byte>(SubPaletteIndex, name: nameof(SubPaletteIndex));

                    // Only used on Saturn
                    FlipX = s.Serialize<bool>(FlipX, name: nameof(FlipX));

                    // Unused for all versions
                    UnusedValue = s.Serialize<byte>(UnusedValue, name: nameof(UnusedValue));

                    // On Saturn it's always the same as the width. On PS1 it is used to indicate if the sprite
                    // has been processed (0 == no, 0xFF == yes).
                    ScanlineWidth = s.Serialize<byte>(ScanlineWidth, name: nameof(ScanlineWidth));

                    // PS1 VRAM data
                    if (settings.EngineVersion != Ray1EngineVersion.Saturn)
                    {
                        s.DoWithDefaults(new SerializerDefaults()
                        {
                            // Everything here is garbage data in files for vol3 as it's filled during runtime and vol6
                            DisableFormattingWarnings = settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol3 ||
                                                        settings.EngineVersion == Ray1EngineVersion.PS1_JPDemoVol6
                        }, () =>
                        {
                            Clut = s.SerializeObject<CBA>(Clut, name: nameof(Clut));
                            TexturePage = s.SerializeObject<TSB>(TexturePage, name: nameof(TexturePage));
                            ImageOffsetInPageX = s.Serialize<byte>(ImageOffsetInPageX, name: nameof(ImageOffsetInPageX));
                            ImageOffsetInPageY = s.Serialize<byte>(ImageOffsetInPageY, name: nameof(ImageOffsetInPageY));
                            s.SerializePadding(2, logIfNotNull: true);
                        });
                    }

                    // The width and height are used as the sprite size here
                    SpriteWidth = (byte)Width;
                    SpriteHeight = (byte)Height;
                    SpriteXPosition = 0;
                    SpriteYPosition = 0;
                }
                else
                {
                    // Unused
                    ImageBufferOffset = s.Serialize<int>(ImageBufferOffset, name: nameof(ImageBufferOffset));
                    
                    Id = s.SerializeNullable<byte>((byte?)Id, name: nameof(Id));
                    Width = s.Serialize<byte>((byte)Width, name: nameof(Width));
                    Height = s.Serialize<byte>((byte)Height, name: nameof(Height));
                    SpriteWidth = s.Serialize<byte>(SpriteWidth, name: nameof(SpriteWidth));
                    SpriteHeight = s.Serialize<byte>(SpriteHeight, name: nameof(SpriteHeight));
                    s.DoBits<byte>(b =>
                    {
                        SpriteXPosition = b.SerializeBits<byte>(SpriteXPosition, 4, name: nameof(SpriteXPosition));
                        SpriteYPosition = b.SerializeBits<byte>(SpriteYPosition, 4, name: nameof(SpriteYPosition));
                    });
                    
                    // Unused (depth doesn't always match here since it's a leftover - use the texture page info instead!)
                    s.DoBits<byte>(b =>
                    {
                        Depth = b.SerializeBits<SpriteDepth>(Depth, 4, name: nameof(Depth));
                        SubPaletteIndex = b.SerializeBits<byte>(SubPaletteIndex, 4, name: nameof(SubPaletteIndex));
                    });
                    UnusedValue = s.Serialize<byte>(UnusedValue, name: nameof(UnusedValue));

                    // PS1 VRAM data
                    Clut = s.SerializeObject<CBA>(Clut, name: nameof(Clut));
                    TexturePage = s.SerializeObject<TSB>(TexturePage, name: nameof(TexturePage));
                    ImageOffsetInPageX = s.Serialize<byte>(ImageOffsetInPageX, name: nameof(ImageOffsetInPageX));
                    ImageOffsetInPageY = s.Serialize<byte>(ImageOffsetInPageY, name: nameof(ImageOffsetInPageY));
                    s.SerializePadding(2, logIfNotNull: true);
                }
            }
            else
            {
                throw new BinarySerializableException(this, $"Unsupported engine branch {settings.EngineBranch}");
            }
        }
    }
}