using System;

namespace BinarySerializer.Ray1.Jaguar
{
    /// <summary>
    /// An Atari Jaguar object
    /// </summary>
    public class JAG_Object : BinarySerializable
    {
        public JAG_ObjectType Type { get; set; }

        /// <summary>
        /// This field gives the value in the vertical counter (in half lines) for the first
        /// (top) line of the object. The vertical counter is latched when the Object
        /// Processor starts so it has the same value across the whole line. If the
        /// display is interlaced the number is even for even lines and odd for odd lines.
        /// If the display is non-interlaced the number is always even. The object will
        /// be active while the vertical counter &gt;= YPOS and HEIGHT &gt; 0.
        /// </summary>
        public int YPos { get; set; }

        /// <summary>
        /// This field gives the number of data lines in the object. As each line is
        /// displayed the height is reduced by one for non-interlaced displays or by two
        /// for interlaced displays. (The height becomes zero if this would result in a
        /// negative value.) The new value is written back to the object.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// This defines the address of the next object. These nineteen bits replace bits
        /// 3 to 21 in the register OLP. This allows an object to link to another object
        /// within the same 4 Mbytes.
        /// </summary>
        public int Link { get; set; }

        /// <summary>
        /// This defines where the pixel data can be found. Like LINK this is a phrase
        /// address. These twenty-one bits define bits 3 to 23 of the data address. This
        /// allows object data to be positioned anywhere in memory. After a line is
        /// displayed the new data address is written back to the object.
        /// </summary>
        public int Data { get; set; }

        /// <summary>
        /// This defines the X position of the first pixel to be plotted. This 12 bit field
        /// defines start positions in the range -2048 to +2047. Address 0 refers to the
        /// left-most pixel in the line buffer.
        /// </summary>
        public int XPos { get; set; }
        
        public JAG_ObjectDepth Depth { get; set; }

        /// <summary>
        /// This value defines how much data, embedded in the image data, must be
        /// skipped. For instance two screens and their common Z buffer could be
        /// arranged in memory in successive phrases (in order that access to the Z
        /// buffer does not cause a page fault). The value 8 * PITCH is added to the
        /// data address when a new phrase must be fetched. A pitch value of one is
        /// used when the pixel data is contiguous - a value of zero will cause the
        /// same phrase to be repeated.
        /// </summary>
        public int Pitch { get; set; }

        /// <summary>
        /// This is the data width in phrases. i.e. Data for the next line of pixels can be
        /// found at 8 * (DATA + DWIDTH).
        /// </summary>
        public int DWidth { get; set; }

        /// <summary>
        /// This is the image width in phrases (must be non zero), and may be used for
        /// clipping.
        /// </summary>
        public int IWidth { get; set; }

        public int PixelsWidth => (int)(DWidth * 8 * Depth switch
        {
            JAG_ObjectDepth.BPP_1 => 8,
            JAG_ObjectDepth.BPP_2 => 4,
            JAG_ObjectDepth.BPP_4 => 2,
            JAG_ObjectDepth.BPP_8 => 1,
            JAG_ObjectDepth.BPP_16 => 1 / 2f,
            JAG_ObjectDepth.BPP_24 => 1 / 3f,
            _ => throw new ArgumentOutOfRangeException()
        });

        /// <summary>
        /// For images with 1 to 4 bits/pixel the top 7 to 4 bits of the index provide the
        /// most significant bits of the palette address.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Flag to draw object from right to left.
        /// </summary>
        public bool Reflect { get; set; }

        /// <summary>
        /// Flag to add object to data in line buffer. The values are then signed offsets
        /// for intensity and the two colour vectors.
        /// </summary>
        public bool RMW { get; set; }

        /// <summary>
        /// Flag to make logical colour zero and reserved physical colours transparent.
        /// </summary>
        public bool Trans { get; set; }

        /// <summary>
        /// This bit forces the Object Processor to release the bus between data
        /// fetches. This should typically be set for low colour resolution objects
        /// because there is time for another bus master to use the bus between data
        /// fetches. For high colour resolution objects the bus should be held by the
        /// Object Processor because there is very little time between data fetches
        /// and other bus masters would probably cause DRAM page faults thereby
        /// slowing the system. External bus masters, the refresh mechanism and
        /// graphics processor DMA mechanism all have higher bus priorities and are
        /// unaffected by this bit.
        /// </summary>
        public bool Release { get; set; }

        /// <summary>
        /// This field identifies the first pixel to be displayed. This can be used to clip
        /// an image. The significance of the bits depends on the colour resolution of
        /// the object and whether the object is scaled. The least significant bit is only
        /// significant for scaled objects where the pixels are written into the line
        /// buffer one at a time. The remaining bits define the first pair of pixels to be
        /// displayed. In 1 bit per pixel mode all five bits are significant, In 2 bits per
        /// pixel mode only the top four bits are significant. Writing zeroes to this field
        /// displays the whole phrase.
        /// </summary>
        public int FirstPix { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoBits<long>(b =>
            {
                Type = b.SerializeBits<JAG_ObjectType>(Type, 3, name: nameof(Type));

                if (Type != JAG_ObjectType.Bitmap)
                    throw new NotImplementedException("Only unscaled bitmap object types are currently supported");

                YPos = b.SerializeBits<int>(YPos, 11, name: nameof(YPos));
                Height = b.SerializeBits<int>(Height, 10, name: nameof(Height));
                Link = b.SerializeBits<int>(Link, 19, name: nameof(Link));
                Data = b.SerializeBits<int>(Data, 21, name: nameof(Data));
            });
            s.DoBits<long>(b =>
            {
                XPos = b.SerializeBits<int>(XPos, 12, name: nameof(XPos));
                Depth = b.SerializeBits<JAG_ObjectDepth>(Depth, 3, name: nameof(Depth));
                Pitch = b.SerializeBits<int>(Pitch, 3, name: nameof(Pitch));
                DWidth = b.SerializeBits<int>(DWidth, 10, name: nameof(DWidth));
                IWidth = b.SerializeBits<int>(IWidth, 10, name: nameof(IWidth));
                Index = b.SerializeBits<int>(Index, 7, name: nameof(Index));
                Reflect = b.SerializeBits<bool>(Reflect, 1, name: nameof(Reflect));
                RMW = b.SerializeBits<bool>(RMW, 1, name: nameof(RMW));
                Trans = b.SerializeBits<bool>(Trans, 1, name: nameof(Trans));
                Release = b.SerializeBits<bool>(Release, 1, name: nameof(Release));
                FirstPix = b.SerializeBits<int>(FirstPix, 6, name: nameof(FirstPix));
                b.SerializePadding(9, logIfNotNull: true);
            });

            s.Log($"Dimensions: {PixelsWidth}x{Height}");
        }

        public enum JAG_ObjectType
        {
            Bitmap = 0,
            ScaledBitmap = 1,
            GraphicsProcessor = 2,
            Branch = 3,
            Stop = 4,
        }

        public enum JAG_ObjectDepth
        {
            BPP_1 = 0,
            BPP_2 = 1,
            BPP_4 = 2,
            BPP_8 = 3,
            BPP_16 = 4,
            BPP_24 = 5,
        }
    }
}