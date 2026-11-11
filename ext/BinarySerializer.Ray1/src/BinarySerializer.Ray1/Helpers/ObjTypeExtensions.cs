namespace BinarySerializer.Ray1
{
    public static class ObjTypeExtensions
    {
        public static int GetUserDataLength(this R2_ObjType objType)
        {
            switch (objType)
            {
                case R2_ObjType.TYPE_56:
                    return 80;

                case R2_ObjType.TYPE_10:
                case R2_ObjType.TYPE_2A:
                    return 52;

                case R2_ObjType.TYPE_2F:
                    return 48;

                case R2_ObjType.TYPE_TRAMPOLINE:
                case R2_ObjType.TYPE_ROTATING_CUBE:
                case R2_ObjType.TYPE_CANNONBALL:
                case R2_ObjType.TYPE_54:
                    return 44;

                case R2_ObjType.TYPE_GUARD:
                    return 36;

                case R2_ObjType.TYPE_SCARED_PLATFORM:
                case R2_ObjType.TYPE_37:
                    return 32;

                case R2_ObjType.TYPE_27:
                    return 30;

                case R2_ObjType.TYPE_45:
                case R2_ObjType.TYPE_GUNSHOT:
                case R2_ObjType.TYPE_DESTRUCTABLE_GROUND:
                    return 28;

                case R2_ObjType.TYPE_23:
                case R2_ObjType.TYPE_TRAP_CUBE:
                    return 26;

                case R2_ObjType.TYPE_RAYMAN:
                case R2_ObjType.TYPE_POING_REFLECT:
                case R2_ObjType.TYPE_PLATFORM:
                case R2_ObjType.TYPE_MOVE_AUTOJUMP_PLAT:
                case R2_ObjType.TYPE_1E:
                case R2_ObjType.TYPE_1F:
                case R2_ObjType.TYPE_25:
                case R2_ObjType.TYPE_2C:
                case R2_ObjType.TYPE_3F:
                case R2_ObjType.TYPE_50:
                    return 24;

                case R2_ObjType.TYPE_POING:
                    return 22;

                case R2_ObjType.TYPE_GENERATING_DOOR:
                case R2_ObjType.TYPE_DESTROYING_DOOR:
                case R2_ObjType.TYPE_1D:
                case R2_ObjType.TYPE_20:
                case R2_ObjType.TYPE_21:
                case R2_ObjType.TYPE_43:
                case R2_ObjType.TYPE_5A:
                case R2_ObjType.TYPE_DINO:
                    return 16;

                case R2_ObjType.TYPE_28:
                    return 14;

                case R2_ObjType.TYPE_16:
                case R2_ObjType.TYPE_TELEPORTER:
                case R2_ObjType.TYPE_TRIGGER:
                case R2_ObjType.TYPE_3A:
                    return 12;

                case R2_ObjType.TYPE_CANNON:
                    return 11;

                case R2_ObjType.TYPE_0E:
                case R2_ObjType.TYPE_17:
                    return 10;

                case R2_ObjType.TYPE_HOOK:
                case R2_ObjType.TYPE_PT_GRAPPIN:
                case R2_ObjType.TYPE_14:
                case R2_ObjType.TYPE_15:
                case R2_ObjType.TYPE_1A:
                case R2_ObjType.TYPE_29:
                case R2_ObjType.TYPE_39:
                case R2_ObjType.TYPE_3B:
                case R2_ObjType.TYPE_VIEW_FINDER:
                case R2_ObjType.TYPE_55:
                    return 8;

                case R2_ObjType.TYPE_46:
                case R2_ObjType.TYPE_4E:
                    return 6;

                case R2_ObjType.TYPE_MINE:
                case R2_ObjType.TYPE_EXPLOSION:
                case R2_ObjType.TYPE_0F:
                case R2_ObjType.TYPE_2D:
                case R2_ObjType.TYPE_36:
                case R2_ObjType.TYPE_41:
                case R2_ObjType.TYPE_42:
                case R2_ObjType.TYPE_44:
                case R2_ObjType.TYPE_49:
                case R2_ObjType.TYPE_4A:
                case R2_ObjType.TYPE_4B:
                case R2_ObjType.TYPE_4F:
                case R2_ObjType.TYPE_5D:
                case R2_ObjType.TYPE_SMACKBX003:
                case R2_ObjType.TYPE_65:
                    return 4;

                case R2_ObjType.TYPE_19:
                case R2_ObjType.TYPE_3D:
                    return 2;

                case R2_ObjType.TYPE_2E:
                    return 1;

                // NOTE: Invalid (unallocated) objects always have a length which is the max of any object in the level. Since
                //       we only have one prototype level available we can know that this will always be 44.
                case R2_ObjType.TYPE_INVALID:
                    return 44;

                default:
                    return 0;
            }
        }
    }
}