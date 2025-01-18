using Terraria;
using Microsoft.Xna.Framework;

namespace LOSMod
{
    public static class LOSUtils
    {
        public static bool IsTileBlockingView(int x, int y)
        {
            var tile = Main.tile[x, y];
            //Checks if tile is active
            if (tile != null && tile.HasTile)
            {
                return Main.tileSolid[tile.TileType];
                
            }
            return false;
        }

        public static bool IsLineOfSightClear(Vector2 start, Vector2 end)
        {
            return Collision.CanHitLine(start, 1, 1, end, 1, 1);
        }
    }
}
