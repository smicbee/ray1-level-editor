using System;
using System.Collections.Generic;
using System.Linq;

namespace BinarySerializer.Ray1.Jaguar
{
    /// <summary>
    /// ROM data
    /// </summary>
    public class JAG_ROM : BinarySerializable
    {
        #region Prototype Reference Data

        public JAG_Proto_ReferenceEntry[] References { get; set; }
        public uint UnkReferenceValue { get; set; }

        #endregion

        #region Global Data

        public Pointer[] WorldLoadCommandPointers { get; set; }
        public Pointer[][] LevelLoadCommandPointers { get; set; }

        #endregion

        #region Global Data (Parsed)

        public JAG_LevelLoadCommandCollection AllfixLoadCommands { get; set; }
        public JAG_LevelLoadCommandCollection[] WorldLoadCommands { get; set; }
        public JAG_LevelLoadCommandCollection[][] LevelLoadCommands { get; set; }

        #endregion

        #region Map Data

        /// <summary>
        /// The map data for the current level
        /// </summary>
        public MapData MapData { get; set; }

        /// <summary>
        /// The event data for the current level
        /// </summary>
        public JAG_EventBlock EventData { get; set; }

        /// <summary>
        /// The current sprite palette
        /// </summary>
        public GBR655Color[] SpritePalette { get; set; }

        /// <summary>
        /// The current tileset data
        /// </summary>
        public GBR655Color[] TileData { get; set; }

        public JAG_MultiSprite[] EventDefinitions { get; set; }
        public JAG_MultiSprite[] AdditionalEventDefinitions { get; set; }

        /// <summary>
        /// The image buffers for the current level, with the key being the memory pointer pointer
        /// </summary>
        public Dictionary<uint, byte[]> ImageBuffers { get; set; }

        public Pointer BackgroundPointer { get; set; }
        public GBR655Color[] Background { get; set; }

        public JAG_WorldInfo[] WorldInfos { get; set; } // World map

        // Prototype only
        public Dictionary<uint, Sprite[]> ImageBufferSprites { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a data block pointer for the Jaguar prototype
        /// </summary>
        /// <param name="reference">The reference type</param>
        /// <returns>The pointer</returns>
        public Pointer GetProtoDataPointer(JAG_Proto_References reference)
        {
            var s = reference.ToString();
            return References.First(x => x.String.Replace(".", "__") == s).DataPointer;
        }

        /// <summary>
        /// Gets a data block reference for the Jaguar prototype
        /// </summary>
        /// <param name="name">The reference name</param>
        /// <returns>The reference</returns>
        public JAG_Proto_ReferenceEntry GetProtoDataReference(string name) => References.First(x => x.String.Replace(".", "__") == name);
        public JAG_Proto_ReferenceEntry GetProtoDataReference(JAG_Proto_References reference) => GetProtoDataReference(reference.ToString());

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            var settings = s.GetRequiredSettings<Ray1Settings>();

            JAG_ROMConfig config = JAG_ROMConfig.FromEngineVersion(settings.EngineVersion);

            // Get info
            var levels = config.NumLevels;

            // Serialize the references for the prototype
            if (settings.EngineVersion == Ray1EngineVersion.Jaguar_Proto)
            {
                s.DoAt(new Pointer(0x8BB6A8, Offset.File), () =>
                {
                    References = s.SerializeObjectArray<JAG_Proto_ReferenceEntry>(References, 1676, onPreSerialize: (x => x.Pre_StringBasePointer = new Pointer(0x8C0538, Offset.File)), name: nameof(References));

                    // Unknown initial 4 bytes, part of the string table
                    UnkReferenceValue = s.Serialize<uint>(UnkReferenceValue, name: nameof(UnkReferenceValue));
                });
            }

            // Serialize event definition data
            if (settings.EngineVersion == Ray1EngineVersion.Jaguar)
            {
                const string key = "RAM_EventDefinitions";

                if (!s.Context.FileExists(key))
                {
                    // Copied to 0x001f9000 in memory. All pointers to 0x001Fxxxx likely point to an entry in this table
                    s.DoAt(s.GetRequiredPreDefinedPointer(JAG_DefinedPointer.EventDefinitions), () =>
                    {
                        byte[] eventDefsDataBytes = s.SerializeArray<byte>(null, config.EventCount * 0x28, name: nameof(eventDefsDataBytes));
                        var file = new MemoryMappedStreamFile(s.Context, key, 0x001f9000, eventDefsDataBytes, Endian.Big);
                        s.Context.AddFile(file);
                        s.DoAt(file.StartPointer, () => EventDefinitions = s.SerializeObjectArray<JAG_MultiSprite>(EventDefinitions, config.EventCount, name: nameof(EventDefinitions)));
                    });
                }
            }
            else
            {
                var offset = settings.EngineVersion == Ray1EngineVersion.Jaguar_Proto ? GetProtoDataPointer(JAG_Proto_References.MS_rayman) : s.GetRequiredPreDefinedPointer(JAG_DefinedPointer.EventDefinitions);

                // Pointers all point to the ROM, not RAM
                s.DoAt(offset, () => EventDefinitions = s.SerializeObjectArray<JAG_MultiSprite>(EventDefinitions,
                    config.EventCount, name: nameof(EventDefinitions)));
            }

            if (AdditionalEventDefinitions == null) 
            {
                if (settings.EngineVersion != Ray1EngineVersion.Jaguar_Proto)
                {
                    AdditionalEventDefinitions = config.AdditionalEventDefinitionPointers.Select(p =>
                    {
                        return s.DoAt(new Pointer(p, s.GetRequiredPreDefinedPointer(JAG_DefinedPointer.EventDefinitions).File), () => s.SerializeObject<JAG_MultiSprite>(default, name: nameof(AdditionalEventDefinitions)));
                    }).ToArray();
                }
                else
                {
                    // TODO: This doesn't seem fully correct
                    //AdditionalEventDefinitions = References.Where(x => x.String.StartsWith("obj_")).Select(x => s.DoAt(x.DataPointer, () => s.SerializeObject<Jaguar_R1_EventDefinition>(default, name: nameof(AdditionalEventDefinitions)))).ToArray();
                }
            }

            if (settings.EngineVersion != Ray1EngineVersion.Jaguar_Proto)
            {
                // Serialize allfix sprite data
                s.DoAt(s.GetRequiredPreDefinedPointer(JAG_DefinedPointer.FixSprites), () => AllfixLoadCommands = s.SerializeObject<JAG_LevelLoadCommandCollection>(AllfixLoadCommands, name: nameof(AllfixLoadCommands)));

                // Serialize world sprite data
                s.DoAt(s.GetRequiredPreDefinedPointer(JAG_DefinedPointer.WorldSprites), () =>
                {
                    WorldLoadCommandPointers ??= new Pointer[6];
                    WorldLoadCommands ??= new JAG_LevelLoadCommandCollection[6];

                    for (int i = 0; i < 6; i++)
                    {
                        Pointer FunctionPointer = s.SerializePointer(null, name: nameof(FunctionPointer));
                        s.DoAt(FunctionPointer, () => {
                            // Parse assembly
                            ushort instruction = s.Serialize<ushort>(0x41f9, name: nameof(instruction)); // Load effective address
                            WorldLoadCommandPointers[i] = s.SerializePointer(WorldLoadCommandPointers[i], name: $"{nameof(WorldLoadCommandPointers)}[{i}]");
                            s.DoAt(WorldLoadCommandPointers[i], () => {
                                WorldLoadCommands[i] = s.SerializeObject<JAG_LevelLoadCommandCollection>(WorldLoadCommands[i], name: $"{nameof(WorldLoadCommands)}[{i}]");
                            });
                        });
                    }
                });

                s.DoAt(s.GetRequiredPreDefinedPointer(JAG_DefinedPointer.MapData), () =>
                {
                    LevelLoadCommandPointers ??= new Pointer[7][];
                    LevelLoadCommands ??= new JAG_LevelLoadCommandCollection[7][];

                    var extraMapCmds = config.ExtraMapCommands;

                    // Serialize map data for each world (6 + extra)
                    for (int i = 0; i < 7; i++)
                    {
                        // Get the number of levels in the world
                        int numLevels = i < 6 ? levels[i].Value : extraMapCmds.Length;

                        LevelLoadCommandPointers[i] ??= new Pointer[numLevels];
                        LevelLoadCommands[i] ??= new JAG_LevelLoadCommandCollection[numLevels];

                        // Get the levels to serialize for the extra map commands
                        int[] extraMapCommands = i == 6 ? extraMapCmds : null;

                        // Serialize the pointer to the load function pointer list
                        Pointer FunctionListPointer = s.SerializePointer(null, name: nameof(FunctionListPointer));

                        // Parse the load functions
                        s.DoAt(FunctionListPointer, () =>
                        {
                            // Serialize every level load function
                            for (int j = 0; j < numLevels; j++)
                            {
                                // If the special world, go to the valid map
                                if (i == 6)
                                    s.Goto(FunctionListPointer + 4 * extraMapCommands[j]);

                                // Serialize the pointer to the function
                                Pointer FunctionPointer = s.SerializePointer(null, name: nameof(FunctionPointer));

                                s.DoAt(FunctionPointer, () => {
                                    // Parse assembly
                                    ushort instruction = s.Serialize<ushort>(0x41f9, name: nameof(instruction)); // Load effective address
                                    while (instruction != 0x41f9)
                                    {
                                        switch (instruction)
                                        {
                                            case 0x23fc: // Move (q)
                                                s.Serialize<uint>(0, name: "MoveArg0"); // arguments differ for each level
                                                s.Serialize<uint>(0, name: "MoveArg1");
                                                break;
                                            case 0x33fc: // Move (w)
                                                s.Serialize<ushort>(0, name: "MoveArg0"); // arguments differ for each level
                                                s.Serialize<ushort>(0, name: "MoveArg1");
                                                break;
                                            case 0x0839: // btst (b)
                                                s.SerializeArray<byte>(null, 6, name: "BtstArgs");
                                                break;
                                            case 0x5279: // addq (w)
                                                s.Serialize<uint>(0, name: "AddqArg0");
                                                break;
                                        }
                                        instruction = s.Serialize<ushort>(0x41f9, name: nameof(instruction)); // Load effective address
                                    }

                                    // Serialize the load command pointer
                                    LevelLoadCommandPointers[i][j] = s.SerializePointer(LevelLoadCommandPointers[i][j], allowInvalid: true, name: $"{nameof(LevelLoadCommandPointers)}[{i}][{j}]");

                                    // Serialize the load commands
                                    s.DoAt(LevelLoadCommandPointers[i][j], () => {
                                        LevelLoadCommands[i][j] = s.SerializeObject<JAG_LevelLoadCommandCollection>(LevelLoadCommands[i][j], name: $"{nameof(LevelLoadCommands)}[{i}][{j}]");
                                    });
                                });
                            }
                        });
                    }
                });

                // Get the current map load commands
                var wldCommands = WorldLoadCommands[Array.FindIndex(levels, x => x.Key == settings.World)];
                var mapCommands = LevelLoadCommands[Array.FindIndex(levels, x => x.Key == settings.World)][settings.Level - 1].Commands;

                // Get pointers
                var mapPointer = mapCommands.FirstOrDefault(x => x.Type == JAG_LevelLoadCommand.LevelLoadCommandType.Map)?.LevelMapBlockPointer;
                var eventPointer = mapCommands.FirstOrDefault(x => x.Type == JAG_LevelLoadCommand.LevelLoadCommandType.Map)?.LevelEventBlockPointer;
                var palPointer = mapCommands.FirstOrDefault(x => x.Type == JAG_LevelLoadCommand.LevelLoadCommandType.Palette || x.Type == JAG_LevelLoadCommand.LevelLoadCommandType.PaletteDemo)?.PalettePointer;

                Pointer tilesPointer;

                if (settings.EngineVersion == Ray1EngineVersion.Jaguar)
                    tilesPointer = mapCommands.LastOrDefault(x => x.Type == JAG_LevelLoadCommand.LevelLoadCommandType.Unzip && x.ImageBufferMemoryPointer == 0x001B3B68)?.ImageBufferPointer ?? WorldLoadCommands[Array.FindIndex(levels, x => x.Key == settings.World)].Commands.First(x => x.Type == JAG_LevelLoadCommand.LevelLoadCommandType.Unzip && x.ImageBufferMemoryPointer == 0x001B3B68).ImageBufferPointer;
                else
                    tilesPointer = WorldLoadCommands[Array.FindIndex(levels, x => x.Key == settings.World)].Commands.Last(x => x.Type == JAG_LevelLoadCommand.LevelLoadCommandType.Unzip && x.ImageBufferMemoryPointer == 0x001BD800).ImageBufferPointer;

                // Serialize map and event data
                s.DoAt(mapPointer, () => s.DoEncoded(new RNC2Encoder(), () => MapData = s.SerializeObject<MapData>(MapData, name: nameof(MapData))));

                if (settings.EngineVersion == Ray1EngineVersion.Jaguar)
                    s.DoAt(eventPointer, () => s.DoEncoded(new RNC2Encoder(), () => EventData = s.SerializeObject<JAG_EventBlock>(EventData, name: nameof(EventData))));
                else if (settings.EngineVersion == Ray1EngineVersion.Jaguar_Demo)
                    s.DoAt(eventPointer, () => EventData = s.SerializeObject<JAG_EventBlock>(EventData, name: nameof(EventData)));

                // Serialize sprite palette
                s.DoAt<GBR655Color[]>(palPointer, () => SpritePalette = s.SerializeObjectArray<GBR655Color>(SpritePalette, 256, name: nameof(SpritePalette)));

                // Serialize tile data
                s.DoAt(tilesPointer, () => s.DoEncoded(new RNC2Encoder(), () => TileData = s.SerializeObjectArray<GBR655Color>(TileData, s.CurrentLength / 2, name: nameof(TileData))));

                // Serialize image buffers
                if (ImageBuffers == null)
                {
                    ImageBuffers = new Dictionary<uint, byte[]>();

                    var index = 0;
                    foreach (var cmd in AllfixLoadCommands.Commands.Concat(wldCommands.Commands).Concat(mapCommands).Where(x => x.Type == JAG_LevelLoadCommand.LevelLoadCommandType.Sprites))
                    {
                        // Later commands overwrite previous ones
                        /*if (ImageBuffers.ContainsKey(cmd.ImageBufferMemoryPointerPointer))
                            continue;
                        */

                        // Temp fix for certain demo buffers
                        try
                        {
                            s.DoAt(cmd.ImageBufferPointer, () => s.DoEncoded(new RNC2Encoder(), () =>
                            {
                                ImageBuffers[cmd.ImageBufferMemoryPointerPointer] = s.SerializeArray<byte>(default, s.CurrentLength, $"ImageBuffer[{index}]");
                            }));
                        }
                        catch (Exception ex)
                        {
                            s.SystemLogger?.LogWarning($"Failed to serialize image buffer at {cmd.ImageBufferMemoryPointerPointer} with error {ex.Message}");
                            ImageBuffers[cmd.ImageBufferMemoryPointerPointer] = new byte[0];
                        }
                        index++;
                    }
                }

                // Serialize background
                var vigs = config.Vignette;
                BackgroundPointer = mapCommands.Concat(wldCommands.Commands).FirstOrDefault(x => x.Type == JAG_LevelLoadCommand.LevelLoadCommandType.Unzip && vigs.Any(y => y.Key == x.ImageBufferPointer.AbsoluteOffset))?.ImageBufferPointer;

                if (BackgroundPointer != null)
                    s.DoAt(BackgroundPointer, () => s.DoEncoded(new RNC2Encoder(), () => Background = s.SerializeObjectArray<GBR655Color>(Background, s.CurrentLength / 2, name: nameof(Background))));

                // Although this isn't an array it's easiest to parse it like this. The game however has pointers to each entry
                // from pointer tables which appear right before this.
                s.DoAt(s.GetRequiredPreDefinedPointer(JAG_DefinedPointer.WorldInfos), () => 
                    WorldInfos = s.SerializeObjectArray<JAG_WorldInfo>(WorldInfos, config.WorldInfoCount, name: nameof(WorldInfos)));

                foreach (JAG_WorldInfo worldInfo in WorldInfos)
                    foreach (JAG_WorldInfoLink link in worldInfo.Links)
                        link.EntryPointer?.ResolveObject(s);
            }
            else
            {
                // NOTE: ml_pics also has load commands, but only 1 and it's a duplicate from ml_jun

                LevelLoadCommands ??= new JAG_LevelLoadCommandCollection[][]
                {
                    new JAG_LevelLoadCommandCollection[1],
                };

                // Level load commands
                s.DoAt(GetProtoDataPointer(JAG_Proto_References.ml_jun), () => LevelLoadCommands[0][0] = s.SerializeObject<JAG_LevelLoadCommandCollection>(LevelLoadCommands[0][0], name: $"{nameof(LevelLoadCommands)}[0][0]"));

                // Palette
                s.DoAt(GetProtoDataPointer(JAG_Proto_References.coltable), () => SpritePalette = s.SerializeObjectArray<GBR655Color>(SpritePalette, 256, name: nameof(SpritePalette)));
                
                // Map
                s.DoAt(GetProtoDataPointer(JAG_Proto_References.jun_map), () => MapData = s.SerializeObject<MapData>(MapData, name: nameof(MapData)));
                
                // Events
                s.DoAt(GetProtoDataPointer(JAG_Proto_References.test_mapevent), () => EventData = s.SerializeObject<JAG_EventBlock>(EventData, onPreSerialize: x =>
                {
                    x.Pre_OffListPointer = GetProtoDataPointer(JAG_Proto_References.test_offlist);
                    x.Pre_EventsPointer = GetProtoDataPointer(JAG_Proto_References.test_event);
                }, name: nameof(EventData)));
                
                // Tilemap
                s.DoAt(GetProtoDataPointer(JAG_Proto_References.jun_block), () => TileData = s.SerializeObjectArray<GBR655Color>(TileData, 440 * (16 * 16), name: nameof(TileData)));

                // Serialize image buffers and sprites
                if (ImageBuffers == null)
                {
                    var orderedPointers = References.Where(x => x.DataPointer != null).OrderBy(x => x.DataPointer.AbsoluteOffset).Select(x => x.DataPointer).Distinct().ToArray();

                    // Get the sprites to load
                    var sprites = new string[]
                    {
                        "ray", // Rayman
                        "liv", // Lividstones
                        "cha", // Hunter
                        "nen", // Jungle (platforms, scenery etc.)
                        "bal", // Hunter bullet
                        "bb1", // Mr Stone
                        "mag", // Magician
                        "mst", // Moskito
                        "div", // Allfix (HUD, photographer etc.)
                        //"ten", // Tentacle flower (has no sprites/animations)
                    }.Select(x => new
                    {
                        RomAdr = GetProtoDataReference($"inc{x}").DataPointer,
                        MemAdr = GetProtoDataReference($"adr_{x}").DataValue,
                        Length = GetProtoDataReference($"len_{x}").DataValue,
                        DescrAdr = GetProtoDataReference($"obj_{(x == "ray" ? "rayman" : x)}").DataPointer,
                        Name = x
                    });

                    ImageBuffers = new Dictionary<uint, byte[]>();
                    ImageBufferSprites = new Dictionary<uint, Sprite[]>();

                    foreach (var spr in sprites)
                    {
                        // Serialize the image buffer
                        s.DoAt(spr.RomAdr, () => ImageBuffers[spr.MemAdr] = s.SerializeArray<byte>(default, spr.Length, name: $"ImageBuffer_{spr.Name}"));

                        // Get length of sprites array
                        var spritesLength = (orderedPointers[Array.FindIndex(orderedPointers, x => x == spr.DescrAdr) + 1].AbsoluteOffset - spr.DescrAdr.AbsoluteOffset) / 0x10;

                        // Serialize sprites
                        s.DoAt(spr.DescrAdr, () => ImageBufferSprites[spr.MemAdr] = s.SerializeObjectArray<Sprite>(default, spritesLength, name: $"{nameof(ImageBufferSprites)}_{spr.Name}"));
                    }
                }

                // Serialize background
                BackgroundPointer = GetProtoDataPointer(JAG_Proto_References.jun_plan0);
                s.DoAt(BackgroundPointer, () => Background = s.SerializeObjectArray<GBR655Color>(Background, 192 * 246, name: nameof(Background)));
            }
        }

        #endregion
    }
}