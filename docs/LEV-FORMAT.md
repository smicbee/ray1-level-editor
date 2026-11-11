# Rayman 1 PC (Rayman Forever / GOG, v1.21) - level format (`PCMAP\<WORLD>\RAY<n>.LEV`)

Sources: BinarySerializer.Ray1 (RayCarrot, MIT) - read; statements marked **[verified]** were checked against the 82 GOG files
(round trip read -> write = byte-identical, see README). All values are little-endian.

## Files
- `PCMAP\ALLFIX.DAT` - global DES/ETA (sprites/animations/states for Rayman, Alpha, MapObj ...). DES index 1..7.
- `PCMAP\RAY<w>.WLD` (w=1..6: Jungle, Music, Mountain, Image, Cave, Cake) - world DES/ETA (index 8.. = world DES), background PCX indices.
- `PCMAP\<JUNGLE|MUSIC|MOUNTAIN|IMAGE|CAVE|CAKE>\RAY<n>.LEV` - levels (82 files: 22/18/13/13/12/4).
- `VIGNET.DAT` - PCX backgrounds (Fond), indexed via `FondIndex`.
- All DES indices in objects count globally: 0 = null, 1..7 = ALLFIX, 8.. = WLD. ETA likewise (ALLFIX first, then WLD).

## LEV layout (PC 1.21, no version header, no level-wide checksums except block checksums)
| Offset | Content |
|---|---|
| 0x00 | u32 pointer to the object block (`ObjectsPointer`) |
| 0x04 | u32 pointer to the normal tileset (`TileSetNormalPointer`) |
| 0x08 | MapInfo (below) |
| after that | u8 `FondIndex`, u8 `ScrollDiffFondIndex`, i32 `ScrollDiffSprites` (parallax DES index) |
| after that | `TileSetModeX` (coarse textures/block code for the "rough" mode; read with checksums + XOR; passed through unchanged) |
| `TileSetNormalPointer` | TileSetNormal |
| `ObjectsPointer` | LevelObjects |

### MapInfo
`u8 checksum8` (sum of all following bytes of the block, mod 256) - `u16 W` - `u16 H` - 3 x palette (256 x RGB, 6 bits per channel = 0..63) - `u8 LastPlan1Palette` (always 2) - `W*H` x block.
Block (8 bytes, row by row): `u16 TileIndex - u8 BlockType (collision) - u8 pad (garbage, must be preserved for byte identity) - u8 RenderMode (0 = fully transparent, 1 = opaque, 2 = partially transparent) - u8 pad`.
Note: according to the community the game ignores RenderMode; the editor sets it consistently (tile 0 -> 1; transparent tile -> 2; otherwise 0). **[verified]**: opaque tile => 0 (except tile 0 => 1), transparent => 2.
Maps consist of 16x16 px tiles; world coordinates are in pixels.

### BlockType (collision)
0 None, 1 ChangeDirection, 2/3 Solid 45 deg right/left, 4-7 Solid 30 deg, 8 Damage, 9 Bounce, 10 Water, 11 Exit, 12 Climb, 13 WaterNoSplash, 14 Passthrough, 15 Solid, 16 Seed, 18-23 slippery slopes, 24 Spikes, 25 Cliff, 30 Slippery.

### TileSetNormal
`u32[1200] BlocksOffsetTable` - `u32 TotalBlocksCount` - `u32 OpaqueBlocksCount` - `u32 MapBlocksSize` - MapBlocks (Checksum8-protected); **the whole header part is XOR 0xFF in KIT/EDU/Fan, unencrypted in PC 1.21**.
MapBlocks: `Opaque x (256 B pixels + u32 TransparencyMode + 28 B unknown = 288 B)`, then `Transparent x (256 B pixels + 256 B alpha + 4 + 28 = 544 B)`, 32 B unknown, u8 Checksum8.
Tile pixels: palette index = `255 - byte` (palette plane 0). The map `TileIndex` indexes `BlocksOffsetTable` -> offset relative to the start of MapBlocks. **[verified]** (tiles render correctly).

### LevelObjects (objects / "events")
`u8 checksum8` (PC 1.21: **no** block checksum, no XOR - only KIT/EDU/Fan use XOR 0x91) - `u16 N` - `u16[N] LinkTable` - `N x ObjData` - `N x ObjCommandsData`.
- **LinkTable**: `t[i]==i` = unlinked; otherwise a ring list (i -> t[i] -> ... -> i). The editor shows group IDs.
- **ObjData (PC-packed, 0x70 bytes)**: u32 DES sprites, u32 DES animations, u32 DES image buffer (usually all three equal), u32 ETA index, u32 command pointer, u32 label pointer (runtime placeholder), CommandContext{i16 ReturnOffset, u16 Count}, i32 Flag2, i32 IsLinked, i32 IsActive, **i32 X, i32 Y** (pixels), u8 ActiveFlag +3 pad, i16 Id, i16 ScreenX/Y, i16 Short_3A, **i16 InitialX/Y**, i16 SpeedX/Y, u16 SpritesCount, i16 CommandOffset, i16 CommandsCount, ... HitPoints, state (MainEtat/SubEtat, Initial*), ObjType (u8), block collision types[5], OffsetBX/BY/HY, animation index/frame, flags (FlipX, ReadCommands, FollowEnabled, ...). Full field list: `BinarySerializer.Ray1\DataTypes\Object\ObjData.cs` (EngineBranch PC, `IsLoadingPackedPCData`). Display names and explanations: [OBJ-FIELDS.md](OBJ-FIELDS.md).
- **ObjCommandsData** per object: `u16 byteLength - u16 labelCount - Commands (byteLength bytes) - u16[labelCount] LabelOffsets`. Command = 1 byte type + arguments (GO_LEFT/RIGHT/WAIT/UP/DOWN/SUBSTATE/SKIP/ADD/STATE/PREPARELOOP/LABEL/GOTO/GOSUB/BRANCH*/SETTEST/WAITSTATE/SKIPTRUE/FALSE/NOP/... 1 arg; DOLOOP/RETURN 0; TEST 1-2; SPEED 3; X/Y 2).
- Object types: `ObjType` (0..270, e.g. 23 TYPE_RAYMAN, 99 **TYPE_RAY_POS = start position**, 54 MEDAILLON, 161 WIZ/Tings, 142 ONEUP). State -> animation: `ETA[eta].ObjectStates[MainEtat][SubEtat].AnimationIndex` -> `DES[des].Animations[i]` -> layer -> `Sprites[SpriteIndex]` (pixels from DES ImageData, palette index used directly; bytes 161/250 + following values are processed to 0, `Design.ProcessImageData`).
- Level end (EXIT collision/gendoor/medallion), dark phases etc. are implemented via type/collision/commands, not via separate structures.

## Encryption / checksums (Rayman 1 PC 1.21 vs. designer/fan versions)
LEV: Checksum8 only in MapInfo/tileset blocks and - only for Kit/Edu/Fan - XOR 0xFF/0x91/0x7D/0xF3. WLD/ALLFIX: DES ImageData XOR 0x8F + Checksum8; PCX index array XOR 0x15. The editor does **not** modify WLD/ALLFIX; level checksums are recomputed by the serializer on write **[verified: round trip identical => checksums correct]**.

## Open points
- `TileSetModeX`, `MapBlocksSize`, the 28/32 unknown bytes: written back unchanged. Creating new tile graphics (new tiles in the set) is not implemented (only painting existing tiles).
- Script label offsets are recomputed by the editor on apply/save (via `ObjCommandCompiler`); script semantics are otherwise not interpreted by the engine model here (the preview is an approximation).
