using System.Collections.Generic;

namespace Ray1LevelEditor
{
    /// <summary>Display names, groups and explanations of the object fields (sources: BinarySerializer.Ray1 ObjData.cs,
    /// Ray1Map and the PS1 decompilation fuerchter/rayman-ps1-decomp; "unknown" = meaning not established).</summary>
    public class ObjField
    {
        public string Name, Label, Group, Tip; public bool Runtime, ReadOnly;
        public ObjField(string n, string l, string g, string t, bool rt = false, bool ro = false) { Name = n; Label = l; Group = g; Tip = t; Runtime = rt; ReadOnly = ro; }
    }

    public static class ObjFields
    {
        public const string GObj = "Object", GPos = "Position", GGfx = "Graphics & Animation", GState = "State", GFight = "Combat",
            GMove = "Movement & Script", GFlags = "Flags", GInt = "Internal / Runtime";
        public static readonly string[] Groups = { GObj, GPos, GGfx, GState, GFight, GMove, GFlags, GInt };

        const string Load = " Source: PS1 decompilation (rayman-ps1-decomp): on level load (first_obj_init) the start values are overwritten from the current value, so the current value is authoritative; on respawn/reinit (obj_init) the current value is restored from the start value. Not separately verified for the PC version.";
        const string LoadI = " Source: PS1 decompilation: overwritten from the current value on level load (first_obj_init), afterwards only used for respawn/reinit (obj_init). Not separately verified for the PC version.";

        public static readonly List<ObjField> All = new List<ObjField>
        {
            new ObjField("Type", "Object type", GObj, "Kind of object (ObjType): determines behavior, graphics set and hard-coded engine logic."),

            new ObjField("XPosition", "X position", GPos, "Position in pixels (top-left corner of the object graphic; reference point = + OffsetBX). Authoritative on level load; the editor also updates Start X when you change/move it." + Load),
            new ObjField("YPosition", "Y position", GPos, "Position in pixels (reference point at the bottom = + OffsetBY). Authoritative on level load; the editor also updates Start Y when you change/move it." + Load),
            new ObjField("InitialXPosition", "Start X", GPos, "X position for respawn/reinit (when the object leaves the screen and restarts). Kept in sync with XPosition by the editor." + LoadI),
            new ObjField("InitialYPosition", "Start Y", GPos, "Y position for respawn/reinit. Kept in sync with YPosition by the editor." + LoadI),

            new ObjField("PCPacked_SpritesIndex", "DES sprites index", GGfx, "Index into the level's sprite table (DES). Selects the object's sprite set; must match animations/image buffer (usually the same value as the next two)."),
            new ObjField("PCPacked_AnimationsIndex", "DES animations index", GGfx, "Index into the level's animation table (DES). Normally identical to the sprites index."),
            new ObjField("PCPacked_ImageBufferIndex", "DES image buffer index", GGfx, "Index into the level's image data table (DES). Normally identical to the sprites index."),
            new ObjField("PCPacked_ETAIndex", "ETA index", GGfx, "Index of the state table (ETA = Etat/state machine): defines which states (MainEtat/SubEtat) exist, with animation, speed and follow-up state."),
            new ObjField("AnimationIndex", "Animation no.", GGfx, "Currently playing animation. Runtime value: reset from the ETA state (MainEtat/SubEtat) on reinit.", rt: true),
            new ObjField("AnimationFrame", "Animation frame", GGfx, "Current frame of the animation. Runtime value: 0 on reinit.", rt: true),
            new ObjField("DisplayPrio", "Draw layer", GGfx, "Layer the sprite is drawn on, 1-7; 0 = not drawn (comment in ObjData.cs). The engine may reset the value depending on the type (PS1: Prio(obj))."),
            new ObjField("Scale", "Scale", GGfx, "Sprite scale factor (short). Value 0 = normal (PS1: first_obj_init sets 0). Exact unit: unknown."),
            new ObjField("FlipX", "Flipped (FlipX)", GGfx, "Sprite mirrored horizontally = facing right/left. The flag is reset on reinit; some types set it via script/type logic.", rt: false),

            new ObjField("MainEtat", "Main state", GState, "Main state (Etat) in the ETA state machine, e.g. 0 = normal, 2 = type-specific special state. The MainEtat/SubEtat combination must exist in the ETA, otherwise warnings/crashes are possible. Authoritative on level load." + Load),
            new ObjField("SubEtat", "Sub state", GState, "Sub state (Sub-Etat) in the ETA state machine; determines animation and speed. Authoritative on level load." + Load),
            new ObjField("InitialMainEtat", "Start main state", GState, "Main state for respawn/reinit (obj_init: main_etat = init_main_etat)." + LoadI),
            new ObjField("InitialSubEtat", "Start sub state", GState, "Sub state for respawn/reinit (obj_init: sub_etat = init_sub_etat)." + LoadI),
            new ObjField("ChangeAnimationMode", "Anim. change mode", GState, "Mode for animation changes (ChangeAnimationMode, e.g. None). Runtime value: reset to None on reinit.", rt: true),
            new ObjField("ActiveFlag", "Active state", GState, "Activation state of the object (Alive/Dead/Reinit ...) in the engine. Runtime value: set by the engine itself at level start (PS1: first_obj_init).", rt: true),

            new ObjField("HitPoints", "Hit points", GFight, "Current hit points; for some types also abused as a parameter (e.g. animation start frame, mirroring). Authoritative on level load." + Load),
            new ObjField("InitialHitPoints", "Start hit points", GFight, "Hit points for respawn/reinit (obj_init: hit_points = init_hit_points)." + LoadI),
            new ObjField("HitSprite", "Hit sprite", GFight, "Sprite index used for hit/collision when shot; exact meaning unknown."),
            new ObjField("OffsetBX", "Reference point X (OffsetBX)", GFight, "Horizontal reference point (center) of the object relative to XPosition. 80 for Rayman. The engine uses it to compute floor/collision position."),
            new ObjField("OffsetBY", "Floor reference Y (OffsetBY)", GFight, "Vertical reference point (foot/floor) relative to YPosition; the engine uses it to find the floor below the object (firstFloorBelow - offset_by). 78 for Rayman."),
            new ObjField("OffsetHY", "Hitbox reference Y (OffsetHY)", GFight, "Vertical reference point for hit/hitbox (Rayman 20, Badguy 30). Exact meaning only inferred from usage."),
            new ObjField("FollowSprite", "Collision sprite", GFight, "Index of the sprite used for object collision (comment in ObjData.cs). For Blacktoon objects the value additionally selects parameters in the engine."),

            new ObjField("Command", "Movement command", GMove, "Current script/movement command (CommandType, e.g. GO_LEFT). Runtime value of the script interpreter; reset on reinit.", rt: true),
            new ObjField("SpeedX", "Speed X", GMove, "Current horizontal speed. Runtime value: 0 on reinit; ETA state/script set it again.", rt: true),
            new ObjField("SpeedY", "Speed Y", GMove, "Current vertical speed. Runtime value: 0 on reinit.", rt: true),
            new ObjField("GravityValue1", "Gravity 1", GMove, "Gravity state value 1 (fall acceleration). Runtime value: 0 on reinit. Exact meaning unknown.", rt: true),
            new ObjField("GravityValue2", "Gravity 2", GMove, "Gravity state value 2; some types (fruits) set 5/10 at start. Runtime value: 0 on reinit. Exact meaning unknown.", rt: true),
            new ObjField("FollowX", "Follow X", GMove, "Follow offset in X (platforms/carried objects). Exact meaning unknown; runtime value."),
            new ObjField("FollowY", "Follow Y", GMove, "Follow offset in Y. Exact meaning unknown; runtime value."),
            new ObjField("RaymanDistance", "Distance to Rayman", GMove, "Distance to Rayman; the engine recomputes it every frame - changing it has no effect.", rt: true),
            new ObjField("DetectZone", "Detection radius", GMove, "Radius (pixels) at which the object notices Rayman. The PS1 decompilation sets it per type on level load (e.g. 40 default, 250 stalactite); whether the PC version keeps the file value has not been verified."),
            new ObjField("DetectZoneFlag", "Detection flag", GMove, "Result of the detection (Rayman within radius). Runtime value: 0 on reinit.", rt: true),
            new ObjField("Timer", "Timer", GMove, "General-purpose counter per type. Runtime value: 0 on reinit.", rt: true),
            new ObjField("ReadCommands", "Script active (ReadCommands)", GMove, "Object executes its script. On reinit the engine sets the flag depending on whether a script exists."),
            new ObjField("CommandTest", "TEST flag (CommandTest)", GMove, "Result/state of the TEST condition in the script (derived from the name; not verified against the engine). Runtime value.", rt: true),

            new ObjField("IsLinked", "Linked (IsLinked)", GFlags, "Object belongs to a link group (objects that are activated/restarted together). The group is set in the 'Link group' field above; the editor sets the flag automatically."),
            new ObjField("IsActive", "Active (IsActive)", GFlags, "Object is currently active (near the camera). Runtime flag; the engine resets it at start.", rt: true),
            new ObjField("IsAlive", "Alive (IsAlive)", GFlags, "Object is 'alive'/visible in gameplay. Runtime flag; the engine sets it at level start depending on the type.", rt: true),
            new ObjField("FollowEnabled", "Follow enabled (FollowEnabled)", GFlags, "Object carries/follows other objects (e.g. platforms that take Rayman along). Some types set it via engine logic."),
            new ObjField("UnknownFlag1", "Unknown 1", GFlags, "Unknown; seems to indicate whether the object has been interacted with (comment in ObjData.cs)."),
            new ObjField("UnknownFlag2", "Unknown 2", GFlags, "Unknown."),
            new ObjField("UnknownFlag3", "Unknown 3", GFlags, "Unknown."),
            new ObjField("UnknownFlag4", "Unknown 4", GFlags, "Unknown."),
            new ObjField("UnknownFlag5", "Unknown 5", GFlags, "Unknown."),

            new ObjField("SpritesCount", "Sprite count", GInt, "Number of sprites in the DES set - part of the graphics data, do not edit by hand (read-only).", ro: true),
            new ObjField("AnimationsCount", "Animation count", GInt, "Number of animations in the DES set - do not edit by hand (read-only).", ro: true),
            new ObjField("CommandsCount", "Script length (bytes)", GInt, "Size of the script; the editor computes the value itself when the script is applied (read-only).", ro: true),
            new ObjField("CommandOffset", "Script position", GInt, "Current read position in the script. Runtime value: the engine sets -1 on reinit (read-only).", rt: true, ro: true),
            new ObjField("EDU_ExtHitPoints", "Extended hit points (EDU)", GInt, "Edu/Kit/Fan versions only: for voice objects (EDU_VoiceLine) sample index > 255; HitPoints = value % 256."),
            new ObjField("Short_3A", "Short_3A", GInt, "Unknown (offset 0x3A)."),
            new ObjField("Short_4A", "Short_4A", GInt, "Unknown (offset 0x4A)."),
            new ObjField("Short_50", "Short_50", GInt, "Unknown (offset 0x50); set per type as an auxiliary counter in the PS1 decompilation."),
            new ObjField("Short_52", "Short_52", GInt, "Unknown (offset 0x52); set per type as an auxiliary counter in the PS1 decompilation."),
            new ObjField("IFramesTimer", "Invulnerability timer", GInt, "Counter for hit pause/invulnerability; runtime value, -1 on reinit (PS1).", rt: true),
            new ObjField("TestBlockIndex", "Test block index", GInt, "Index for block collision tests; runtime value, 0 on reinit.", rt: true),
            new ObjField("ActiveTimer", "Active timer", GInt, "Counter preventing immediate re-activation; runtime value (120 after reinit).", rt: true),
            new ObjField("Byte_7D", "Byte_7D", GInt, "Unknown (offset 0x7D); auxiliary value per type in the PS1 decompilation."),
            new ObjField("InitFlag", "InitFlag", GInt, "According to ObjData.cs apparently unused."),
            new ObjField("CommandContextDepth", "Script context depth", GInt, "Depth of the script call stack (gosub); runtime value, 0xFF on reinit.", rt: true),
            new ObjField("Id", "Object no. (Id)", GInt, "Index of the object in the level; the engine reassigns it on load (read-only).", rt: true, ro: true),
            new ObjField("ScreenXPosition", "Screen X", GInt, "Position on screen; the engine recomputes it every frame (read-only).", rt: true, ro: true),
            new ObjField("ScreenYPosition", "Screen Y", GInt, "Position on screen; the engine recomputes it every frame (read-only).", rt: true, ro: true),
        };
    }
}
