using System.Collections.Generic;

namespace BinarySerializer.Ray1
{
    public static class WorldHelpers
    {
        public static IEnumerable<World> EnumerateWorlds(bool includeSpecial = false)
        {
            yield return World.Jungle;
            yield return World.Music;
            yield return World.Mountain;
            yield return World.Image;
            yield return World.Cave;
            yield return World.Cake;

            if (includeSpecial)
            {
                yield return World.Menu;
                yield return World.Multiplayer;
            }
        }
    }
}