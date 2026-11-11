# Object fields (ObjData) - reference

Shown in the editor (tab "Object", groups as expanders). Sources: comments in `BinarySerializer.Ray1/DataTypes/Object/ObjData.cs`, the PS1 decompilation [fuerchter/rayman-ps1-decomp](https://github.com/fuerchter/rayman-ps1-decomp) (`first_obj_init`, `obj_init`, `REINIT_OBJECT`), and Ray1Map. "Unknown" = meaning not established. The PS1 logic has **not** been verified separately for the PC version.

## Start values vs. current values (from the PS1 decompilation)

- Level load (`first_obj_init`): `init_x_pos/init_y_pos/init_main_etat/init_sub_etat/init_hit_points` are **overwritten** from the current values. So `XPosition`, `YPosition`, `MainEtat`, `SubEtat` and `HitPoints` are the authoritative values on load.
- Respawn/reinit (`obj_init`, `REINIT_OBJECT`): current values are restored from the `Initial*` values; additionally `AnimationFrame=0`, `SpeedX/Y=0`, `Gravity*=0`, `Timer=0`, FlipX cleared, `CommandOffset=-1`, `IFramesTimer=-1`.
- Editor: moving/changing X/Y also writes `InitialX/Y` (both stay consistent). Initial state/hit points are not updated automatically - when changing MainEtat/SubEtat/HitPoints, set the Initial fields as well if needed.

Runtime value = the engine overwrites the value itself; read-only = cannot be changed in the editor.

## Object

| Field | Display name | Explanation | Note |
|---|---|---|---|
| `Type` | Object type | Kind of object (ObjType): determines behavior, graphics set and hard-coded engine logic. |  |

## Position

| Field | Display name | Explanation | Note |
|---|---|---|---|
| `XPosition` | X position | Position in pixels (top-left corner of the object graphic; reference point = + OffsetBX). Authoritative on level load; the editor also updates Start X when you change/move it. Source: PS1 decompilation (rayman-ps1-decomp): on level load (first_obj_init) the start values are overwritten from the current value, so the current value is authoritative; on respawn/reinit (obj_init) the current value is restored from the start value. Not separately verified for the PC version. |  |
| `YPosition` | Y position | Position in pixels (reference point at the bottom = + OffsetBY). Authoritative on level load; the editor also updates Start Y when you change/move it. Source: PS1 decompilation (rayman-ps1-decomp): on level load (first_obj_init) the start values are overwritten from the current value, so the current value is authoritative; on respawn/reinit (obj_init) the current value is restored from the start value. Not separately verified for the PC version. |  |
| `InitialXPosition` | Start X | X position for respawn/reinit (when the object leaves the screen and restarts). Kept in sync with XPosition by the editor. Source: PS1 decompilation: overwritten from the current value on level load (first_obj_init), afterwards only used for respawn/reinit (obj_init). Not separately verified for the PC version. |  |
| `InitialYPosition` | Start Y | Y position for respawn/reinit. Kept in sync with YPosition by the editor. Source: PS1 decompilation: overwritten from the current value on level load (first_obj_init), afterwards only used for respawn/reinit (obj_init). Not separately verified for the PC version. |  |

## Graphics & Animation

| Field | Display name | Explanation | Note |
|---|---|---|---|
| `PCPacked_SpritesIndex` | DES sprites index | Index into the level's sprite table (DES). Selects the object's sprite set; must match animations/image buffer (usually the same value as the next two). |  |
| `PCPacked_AnimationsIndex` | DES animations index | Index into the level's animation table (DES). Normally identical to the sprites index. |  |
| `PCPacked_ImageBufferIndex` | DES image buffer index | Index into the level's image data table (DES). Normally identical to the sprites index. |  |
| `PCPacked_ETAIndex` | ETA index | Index of the state table (ETA = Etat/state machine): defines which states (MainEtat/SubEtat) exist, with animation, speed and follow-up state. |  |
| `AnimationIndex` | Animation no. | Currently playing animation. Runtime value: reset from the ETA state (MainEtat/SubEtat) on reinit. | runtime value |
| `AnimationFrame` | Animation frame | Current frame of the animation. Runtime value: 0 on reinit. | runtime value |
| `DisplayPrio` | Draw layer | Layer the sprite is drawn on, 1-7; 0 = not drawn (comment in ObjData.cs). The engine may reset the value depending on the type (PS1: Prio(obj)). |  |
| `Scale` | Scale | Sprite scale factor (short). Value 0 = normal (PS1: first_obj_init sets 0). Exact unit: unknown. |  |
| `FlipX` | Flipped (FlipX) | Sprite mirrored horizontally = facing right/left. The flag is reset on reinit; some types set it via script/type logic. |  |

## State

| Field | Display name | Explanation | Note |
|---|---|---|---|
| `MainEtat` | Main state | Main state (Etat) in the ETA state machine, e.g. 0 = normal, 2 = type-specific special state. The MainEtat/SubEtat combination must exist in the ETA, otherwise warnings/crashes are possible. Authoritative on level load. Source: PS1 decompilation (rayman-ps1-decomp): on level load (first_obj_init) the start values are overwritten from the current value, so the current value is authoritative; on respawn/reinit (obj_init) the current value is restored from the start value. Not separately verified for the PC version. |  |
| `SubEtat` | Sub state | Sub state (Sub-Etat) in the ETA state machine; determines animation and speed. Authoritative on level load. Source: PS1 decompilation (rayman-ps1-decomp): on level load (first_obj_init) the start values are overwritten from the current value, so the current value is authoritative; on respawn/reinit (obj_init) the current value is restored from the start value. Not separately verified for the PC version. |  |
| `InitialMainEtat` | Start main state | Main state for respawn/reinit (obj_init: main_etat = init_main_etat). Source: PS1 decompilation: overwritten from the current value on level load (first_obj_init), afterwards only used for respawn/reinit (obj_init). Not separately verified for the PC version. |  |
| `InitialSubEtat` | Start sub state | Sub state for respawn/reinit (obj_init: sub_etat = init_sub_etat). Source: PS1 decompilation: overwritten from the current value on level load (first_obj_init), afterwards only used for respawn/reinit (obj_init). Not separately verified for the PC version. |  |
| `ChangeAnimationMode` | Anim. change mode | Mode for animation changes (ChangeAnimationMode, e.g. None). Runtime value: reset to None on reinit. | runtime value |
| `ActiveFlag` | Active state | Activation state of the object (Alive/Dead/Reinit ...) in the engine. Runtime value: set by the engine itself at level start (PS1: first_obj_init). | runtime value |

## Combat

| Field | Display name | Explanation | Note |
|---|---|---|---|
| `HitPoints` | Hit points | Current hit points; for some types also abused as a parameter (e.g. animation start frame, mirroring). Authoritative on level load. Source: PS1 decompilation (rayman-ps1-decomp): on level load (first_obj_init) the start values are overwritten from the current value, so the current value is authoritative; on respawn/reinit (obj_init) the current value is restored from the start value. Not separately verified for the PC version. |  |
| `InitialHitPoints` | Start hit points | Hit points for respawn/reinit (obj_init: hit_points = init_hit_points). Source: PS1 decompilation: overwritten from the current value on level load (first_obj_init), afterwards only used for respawn/reinit (obj_init). Not separately verified for the PC version. |  |
| `HitSprite` | Hit sprite | Sprite index used for hit/collision when shot; exact meaning unknown. |  |
| `OffsetBX` | Reference point X (OffsetBX) | Horizontal reference point (center) of the object relative to XPosition. 80 for Rayman. The engine uses it to compute floor/collision position. |  |
| `OffsetBY` | Floor reference Y (OffsetBY) | Vertical reference point (foot/floor) relative to YPosition; the engine uses it to find the floor below the object (firstFloorBelow - offset_by). 78 for Rayman. |  |
| `OffsetHY` | Hitbox reference Y (OffsetHY) | Vertical reference point for hit/hitbox (Rayman 20, Badguy 30). Exact meaning only inferred from usage. |  |
| `FollowSprite` | Collision sprite | Index of the sprite used for object collision (comment in ObjData.cs). For Blacktoon objects the value additionally selects parameters in the engine. |  |

## Movement & Script

| Field | Display name | Explanation | Note |
|---|---|---|---|
| `Command` | Movement command | Current script/movement command (CommandType, e.g. GO_LEFT). Runtime value of the script interpreter; reset on reinit. | runtime value |
| `SpeedX` | Speed X | Current horizontal speed. Runtime value: 0 on reinit; ETA state/script set it again. | runtime value |
| `SpeedY` | Speed Y | Current vertical speed. Runtime value: 0 on reinit. | runtime value |
| `GravityValue1` | Gravity 1 | Gravity state value 1 (fall acceleration). Runtime value: 0 on reinit. Exact meaning unknown. | runtime value |
| `GravityValue2` | Gravity 2 | Gravity state value 2; some types (fruits) set 5/10 at start. Runtime value: 0 on reinit. Exact meaning unknown. | runtime value |
| `FollowX` | Follow X | Follow offset in X (platforms/carried objects). Exact meaning unknown; runtime value. |  |
| `FollowY` | Follow Y | Follow offset in Y. Exact meaning unknown; runtime value. |  |
| `RaymanDistance` | Distance to Rayman | Distance to Rayman; the engine recomputes it every frame - changing it has no effect. | runtime value |
| `DetectZone` | Detection radius | Radius (pixels) at which the object notices Rayman. The PS1 decompilation sets it per type on level load (e.g. 40 default, 250 stalactite); whether the PC version keeps the file value has not been verified. |  |
| `DetectZoneFlag` | Detection flag | Result of the detection (Rayman within radius). Runtime value: 0 on reinit. | runtime value |
| `Timer` | Timer | General-purpose counter per type. Runtime value: 0 on reinit. | runtime value |
| `ReadCommands` | Script active (ReadCommands) | Object executes its script. On reinit the engine sets the flag depending on whether a script exists. |  |
| `CommandTest` | TEST flag (CommandTest) | Result/state of the TEST condition in the script (derived from the name; not verified against the engine). Runtime value. | runtime value |

## Flags

| Field | Display name | Explanation | Note |
|---|---|---|---|
| `IsLinked` | Linked (IsLinked) | Object belongs to a link group (objects that are activated/restarted together). The group is set in the 'Link group' field above; the editor sets the flag automatically. |  |
| `IsActive` | Active (IsActive) | Object is currently active (near the camera). Runtime flag; the engine resets it at start. | runtime value |
| `IsAlive` | Alive (IsAlive) | Object is 'alive'/visible in gameplay. Runtime flag; the engine sets it at level start depending on the type. | runtime value |
| `FollowEnabled` | Follow enabled (FollowEnabled) | Object carries/follows other objects (e.g. platforms that take Rayman along). Some types set it via engine logic. |  |
| `UnknownFlag1` | Unknown 1 | Unknown; seems to indicate whether the object has been interacted with (comment in ObjData.cs). |  |
| `UnknownFlag2` | Unknown 2 | Unknown. |  |
| `UnknownFlag3` | Unknown 3 | Unknown. |  |
| `UnknownFlag4` | Unknown 4 | Unknown. |  |
| `UnknownFlag5` | Unknown 5 | Unknown. |  |

## Internal / Runtime

| Field | Display name | Explanation | Note |
|---|---|---|---|
| `SpritesCount` | Sprite count | Number of sprites in the DES set - part of the graphics data, do not edit by hand (read-only). | read-only |
| `AnimationsCount` | Animation count | Number of animations in the DES set - do not edit by hand (read-only). | read-only |
| `CommandsCount` | Script length (bytes) | Size of the script; the editor computes the value itself when the script is applied (read-only). | read-only |
| `CommandOffset` | Script position | Current read position in the script. Runtime value: the engine sets -1 on reinit (read-only). | runtime value, read-only |
| `EDU_ExtHitPoints` | Extended hit points (EDU) | Edu/Kit/Fan versions only: for voice objects (EDU_VoiceLine) sample index > 255; HitPoints = value % 256. |  |
| `Short_3A` | Short_3A | Unknown (offset 0x3A). |  |
| `Short_4A` | Short_4A | Unknown (offset 0x4A). |  |
| `Short_50` | Short_50 | Unknown (offset 0x50); set per type as an auxiliary counter in the PS1 decompilation. |  |
| `Short_52` | Short_52 | Unknown (offset 0x52); set per type as an auxiliary counter in the PS1 decompilation. |  |
| `IFramesTimer` | Invulnerability timer | Counter for hit pause/invulnerability; runtime value, -1 on reinit (PS1). | runtime value |
| `TestBlockIndex` | Test block index | Index for block collision tests; runtime value, 0 on reinit. | runtime value |
| `ActiveTimer` | Active timer | Counter preventing immediate re-activation; runtime value (120 after reinit). | runtime value |
| `Byte_7D` | Byte_7D | Unknown (offset 0x7D); auxiliary value per type in the PS1 decompilation. |  |
| `InitFlag` | InitFlag | According to ObjData.cs apparently unused. |  |
| `CommandContextDepth` | Script context depth | Depth of the script call stack (gosub); runtime value, 0xFF on reinit. | runtime value |
| `Id` | Object no. (Id) | Index of the object in the level; the engine reassigns it on load (read-only). | runtime value, read-only |
| `ScreenXPosition` | Screen X | Position on screen; the engine recomputes it every frame (read-only). | runtime value, read-only |
| `ScreenYPosition` | Screen Y | Position on screen; the engine recomputes it every frame (read-only). | runtime value, read-only |

Additionally in the Flags section: **Link group** (editor concept, 0 = none) - objects with the same number form a link chain (activated/restarted together).
