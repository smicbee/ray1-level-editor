using System;
using System.Collections.Generic;
using System.Linq;

namespace BinarySerializer.Ray1.Jaguar
{
    /// <summary>
    /// A multi-sprite (MS)
    /// </summary>
    public class JAG_MultiSprite : BinarySerializable 
    {
		#region Event Data

		public short SpritesCount { get; set; } // Number of layers in an animation
		public Pointer CharacterPointer { get; set; }
		public ushort Verbe { get; set; } // Index to function list

		public Pointer CodePointer { get; set; }
		public Pointer CurrentStatePointer { get; set; }
		public Pointer ComplexDataPointer { get; set; }
		public ushort UShort_10 { get; set; }
		public ushort UShort_12 { get; set; }
		public Pointer SpritesPointer { get; set; }

		public uint ImageBufferMemoryPointerPointer { get; set; }
		public uint UInt_1C { get; set; }
		public byte Byte_20 { get; set; }
		public byte Byte_21 { get; set; }
		public byte Byte_22 { get; set; }
		public byte Byte_23 { get; set; }
		public ushort UShort_24 { get; set; }

		public byte[] UnkBytes { get; set; }
		public byte Byte_24 { get; set; }

		public byte Byte_25 { get; set; }
        public byte Byte_26 { get; set; }

		public byte Byte_27 { get; set; }
		public Pointer UnkPointer1 { get; set; }
		public Pointer UnkPointer2 { get; set; }

		public Pointer AnimationPointer { get; set; }
		public byte FrameCount { get; set; }
		public AnimationLayer[] AnimationLayers { get; set; }

		#endregion

		#region Parsed from Pointers

		public JAG_Character Character { get; set; }
		public Sprite[] Sprites { get; set; }
		public JAG_EventState[] States { get; set; }
		public JAG_EventComplexData ComplexData { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Handles the data serialization
		/// </summary>
		/// <param name="s">The serializer object</param>
		public override void SerializeImpl(SerializerObject s) 
        {
			var settings = s.GetRequiredSettings<Ray1Settings>();

			SpritesCount = s.Serialize<short>(SpritesCount, name: nameof(SpritesCount));
			CharacterPointer = s.SerializePointer(CharacterPointer, name: nameof(CharacterPointer));
			Verbe = s.Serialize<ushort>(Verbe, name: nameof(Verbe));
			if (Verbe == 29) {
				CurrentStatePointer = s.SerializePointer(CurrentStatePointer, name: nameof(CurrentStatePointer));
				ComplexDataPointer = s.SerializePointer(ComplexDataPointer, name: nameof(ComplexDataPointer));
				UShort_10 = s.Serialize<ushort>(UShort_10, name: nameof(UShort_10));
				UShort_12 = s.Serialize<ushort>(UShort_12, name: nameof(UShort_12));
				ImageBufferMemoryPointerPointer = s.Serialize<uint>(ImageBufferMemoryPointerPointer, name: nameof(ImageBufferMemoryPointerPointer));
				UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));
				Byte_20 = s.Serialize<byte>(Byte_20, name: nameof(Byte_20));
				Byte_21 = s.Serialize<byte>(Byte_21, name: nameof(Byte_21));
				Byte_22 = s.Serialize<byte>(Byte_22, name: nameof(Byte_22));
				Byte_23 = s.Serialize<byte>(Byte_23, name: nameof(Byte_23));
				UShort_24 = s.Serialize<ushort>(UShort_24, name: nameof(UShort_24));
                Byte_25 = s.Serialize<byte>(Byte_25, name: nameof(Byte_25));
                Byte_26 = s.Serialize<byte>(Byte_26, name: nameof(Byte_26));
				CodePointer = s.SerializePointer(CodePointer, name: nameof(CodePointer));
			} else if (Verbe == 6 || Verbe == 7 || Verbe == 30 || Verbe == 31 || (settings.EngineVersion == Ray1EngineVersion.Jaguar_Proto && Verbe == 15)) {
				CurrentStatePointer = s.SerializePointer(CurrentStatePointer, name: nameof(CurrentStatePointer));
				ComplexDataPointer = s.SerializePointer(ComplexDataPointer, name: nameof(ComplexDataPointer));
				UShort_10 = s.Serialize<ushort>(UShort_10, name: nameof(UShort_10));
				UShort_12 = s.Serialize<ushort>(UShort_12, name: nameof(UShort_12));
				ImageBufferMemoryPointerPointer = s.Serialize<uint>(ImageBufferMemoryPointerPointer, name: nameof(ImageBufferMemoryPointerPointer));
				UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));
				UShort_24 = s.Serialize<ushort>(UShort_24, name: nameof(UShort_24));
                Byte_25 = s.Serialize<byte>(Byte_25, name: nameof(Byte_25));
                Byte_26 = s.Serialize<byte>(Byte_26, name: nameof(Byte_26));
				Byte_20 = s.Serialize<byte>(Byte_20, name: nameof(Byte_20));
				Byte_21 = s.Serialize<byte>(Byte_21, name: nameof(Byte_21));
				Byte_22 = s.Serialize<byte>(Byte_22, name: nameof(Byte_22));
				Byte_23 = s.Serialize<byte>(Byte_23, name: nameof(Byte_23));
				Byte_24 = s.Serialize<byte>(Byte_24, name: nameof(Byte_24));
				Byte_25 = s.Serialize<byte>(Byte_25, name: nameof(Byte_25));
				Byte_26 = s.Serialize<byte>(Byte_26, name: nameof(Byte_26));
				Byte_27 = s.Serialize<byte>(Byte_27, name: nameof(Byte_27));
			} else if (Verbe == 23 || Verbe == 11 || Verbe == 2 || (settings.EngineVersion == Ray1EngineVersion.Jaguar_Demo && Verbe == 10)) {
				CodePointer = s.SerializePointer(CodePointer, name: nameof(CodePointer));
				UnkBytes = s.SerializeArray<byte>(UnkBytes, 0x1c, name: nameof(UnkBytes));
			} else if (Verbe == 36 || Verbe == 37 || Verbe == 56) {
				Byte_20 = s.Serialize<byte>(Byte_20, name: nameof(Byte_20));
				Byte_21 = s.Serialize<byte>(Byte_21, name: nameof(Byte_21));
				Byte_22 = s.Serialize<byte>(Byte_22, name: nameof(Byte_22));
				Byte_23 = s.Serialize<byte>(Byte_23, name: nameof(Byte_23));
				UnkBytes = s.SerializeArray<byte>(UnkBytes, 0x1C, name: nameof(UnkBytes));
			} else if (Verbe == 111 || Verbe == 17) {
				UnkBytes = s.SerializeArray<byte>(UnkBytes, 0x8, name: nameof(UnkBytes));
				UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));
				UShort_10 = s.Serialize<ushort>(UShort_10, name: nameof(UShort_10));
				CodePointer = s.SerializePointer(CodePointer, name: nameof(CodePointer));
				UnkPointer1 = s.SerializePointer(UnkPointer1, name: nameof(UnkPointer1));
				UnkBytes = s.SerializeArray<byte>(UnkBytes, 0xA, name: nameof(UnkBytes));
			} else if (Verbe == 112 || Verbe == 113 || Verbe == 114 || Verbe == 26) {
				UShort_10 = s.Serialize<ushort>(UShort_10, name: nameof(UShort_10));
				CodePointer = s.SerializePointer(CodePointer, name: nameof(CodePointer));
				UnkBytes = s.SerializeArray<byte>(UnkBytes, 0x1a, name: nameof(UnkBytes));
			} else if (Verbe == 25) {
				SpritesPointer = s.SerializePointer(SpritesPointer, name: nameof(SpritesPointer));
				AnimationPointer = s.SerializePointer(AnimationPointer, name: nameof(AnimationPointer));
				CodePointer = s.SerializePointer(CodePointer, name: nameof(CodePointer));
				Byte_20 = s.Serialize<byte>(Byte_20, name: nameof(Byte_20));
				Byte_21 = s.Serialize<byte>(Byte_21, name: nameof(Byte_21));
				FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
				Byte_23 = s.Serialize<byte>(Byte_23, name: nameof(Byte_23));
				UnkBytes = s.SerializeArray<byte>(UnkBytes, 0x10, name: nameof(UnkBytes));
			} else if(settings.EngineVersion == Ray1EngineVersion.Jaguar_Proto && (Verbe == 10 || Verbe == 26 || Verbe == 19)) {
				if (Verbe == 10) {
					UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));
					UShort_10 = s.Serialize<ushort>(UShort_10, name: nameof(UShort_10));
					CodePointer = s.SerializePointer(CodePointer, name: nameof(CodePointer));
					UnkBytes = s.SerializeArray<byte>(UnkBytes, 0x16, name: nameof(UnkBytes));
				} else if (Verbe == 26 || Verbe == 19) {
					UShort_10 = s.Serialize<ushort>(UShort_10, name: nameof(UShort_10));
					UnkPointer1 = s.SerializePointer(UnkPointer1, name: nameof(UnkPointer1));
					UnkBytes = s.SerializeArray<byte>(UnkBytes, 0x1A, name: nameof(UnkBytes));
				}
			} else {
				CurrentStatePointer = s.SerializePointer(CurrentStatePointer, name: nameof(CurrentStatePointer));
				UnkPointer1 = s.SerializePointer(UnkPointer1, name: nameof(UnkPointer1));
				UShort_10 = s.Serialize<ushort>(UShort_10, name: nameof(UShort_10));
				UShort_12 = s.Serialize<ushort>(UShort_12, name: nameof(UShort_12));
				SpritesPointer = s.SerializePointer(SpritesPointer, name: nameof(SpritesPointer));
				ImageBufferMemoryPointerPointer = s.Serialize<uint>(ImageBufferMemoryPointerPointer, name: nameof(ImageBufferMemoryPointerPointer));
				UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));
				Byte_20 = s.Serialize<byte>(Byte_20, name: nameof(Byte_20));
				Byte_21 = s.Serialize<byte>(Byte_21, name: nameof(Byte_21));
				Byte_22 = s.Serialize<byte>(Byte_22, name: nameof(Byte_22));
				Byte_23 = s.Serialize<byte>(Byte_23, name: nameof(Byte_23));
				UShort_24 = s.Serialize<ushort>(UShort_24, name: nameof(UShort_24));
                Byte_25 = s.Serialize<byte>(Byte_25, name: nameof(Byte_25));
                Byte_26 = s.Serialize<byte>(Byte_26, name: nameof(Byte_26));
			}

            s.DoAt(CharacterPointer, () => Character = s.SerializeObject<JAG_Character>(Character, name: nameof(Character)));

			if (!s.FullSerialize)
				return;

            // Serialize event states
			if (CurrentStatePointer != null) {
				// TODO: Find way to get the length
				// First look back to find current state index
				uint currentStateIndex = 1;
				while (true) {
					Pointer CheckPtr0 = null, CheckPtr1 = null;
					bool success = true;
					s.DoAt(CurrentStatePointer - currentStateIndex * 0xC + 0x2, () => {
						try {
							CheckPtr0 = s.SerializePointer(CheckPtr0, name: nameof(CheckPtr0));
							CheckPtr1 = s.SerializePointer(CheckPtr1, name: nameof(CheckPtr1));
						} catch (Exception) {
							success = false;
						}
					});
					if (!success
					|| (CheckPtr0 != null && CheckPtr0.File != CurrentStatePointer.File)
					|| (CheckPtr1 != null && CheckPtr1.File != CurrentStatePointer.File)) {
						currentStateIndex--;
						break;
					}
					currentStateIndex++;
				}
				// Serialize from first state index
				s.DoAt(CurrentStatePointer - currentStateIndex * 0xC, () => {
					var temp = new List<JAG_EventState>();

					var index = 0;
					while (true) {

						if (s.CurrentAbsoluteOffset > CurrentStatePointer.AbsoluteOffset) {
							Pointer CheckPtr0 = null, CheckPtr1 = null;
							bool success = true;
							s.DoAt(s.CurrentPointer + 0x2, () => {
								try {
									CheckPtr0 = s.SerializePointer(CheckPtr0, name: nameof(CheckPtr0));
									CheckPtr1 = s.SerializePointer(CheckPtr1, name: nameof(CheckPtr1));
								} catch (Exception) {
									success = false;
								}
							});
							if (!success
							|| (CheckPtr0 != null && CheckPtr0.File != CurrentStatePointer.File)
							|| (CheckPtr1 != null && CheckPtr1.File != CurrentStatePointer.File)) {
								break;
							}
							/*
							byte[] CheckBytes = default;
							s.DoAt(s.CurrentPointer + 0xA, () => {
								CheckBytes = s.SerializeArray<byte>(CheckBytes, 2, name: nameof(CheckBytes));
							});
							if (CheckBytes[0] != 0 || CheckBytes[1] != 0xFF)
							{
								break;
							}*/
						}
						var i = s.SerializeObject<JAG_EventState>(default, name: $"{nameof(States)}[{index}]");

						temp.Add(i);

						index++;
					}

					States = temp.ToArray();
				});
			} else if (ComplexDataPointer != null) {
				// TODO: Why does this not work for the proto?
				if (!(Verbe == 30 && SpritesCount == 5)) { // Different struct in this case, that only has states with code pointers
					s.DoAt(ComplexDataPointer, () => {
						ComplexData = s.SerializeObject<JAG_EventComplexData>(ComplexData, onPreSerialize: cd => {
							cd.Pre_Verbe = Verbe;
							cd.Pre_SpritesCount = (ushort)SpritesCount;
						}, name: nameof(ComplexData));
					});
				}
			}

			s.DoAt(AnimationPointer, () => {
				int layers = SpritesCount > 0 ? SpritesCount : 1;
				AnimationLayers = s.SerializeObjectArray<AnimationLayer>(AnimationLayers, layers * FrameCount, name: nameof(AnimationLayers));
			});

			// Serialize sprites based on the state animations
			if (settings.EngineVersion != Ray1EngineVersion.Jaguar_Proto)
            {
                s.DoAt(SpritesPointer, () => {
                    if (Verbe == 25)
                    {
                        ImageBufferMemoryPointerPointer = s.Serialize<uint>(ImageBufferMemoryPointerPointer, name: nameof(ImageBufferMemoryPointerPointer));
                    }

                    if (AnimationLayers != null)
                    {
                        int maxImageIndex = AnimationLayers.Max(x => UShort_12 == 5 ? BitHelpers.ExtractBits(x.SpriteIndex, 7, 0) : x.SpriteIndex);
                        Sprites = s.SerializeObjectArray<Sprite>(Sprites, maxImageIndex + 1, name: nameof(Sprites));
                    }
                    else if (States != null && States.Length > 0)
                    {
                        int maxImageIndex = States
                            .Where(x => x?.Animation != null)
                            .SelectMany(x => x.Animation.Layers)
                            .Max(x => UShort_12 == 5 ? BitHelpers.ExtractBits(x.SpriteIndex, 7, 0) : x.SpriteIndex);
                        Sprites = s.SerializeObjectArray<Sprite>(Sprites, maxImageIndex + 1, name: nameof(Sprites));
                    }
                    else
                    {
                        var temp = new List<Sprite>();

                        var index = 0;
                        while (true)
                        {
                            var i = s.SerializeObject<Sprite>(default, name: $"{nameof(Sprites)}[{index}]");

                            if (temp.Any() && i.Id != null && i.ImageBufferOffset < temp.Last().ImageBufferOffset)
                                break;

                            temp.Add(i);

                            index++;
                        }

                        Sprites = temp.ToArray();
                    }
                });
            }
        }

		#endregion
	}
}