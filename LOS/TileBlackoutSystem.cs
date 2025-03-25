using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using System;
using Terraria.ID;


namespace LOSMod
{
    public static class TileBlackoutSystem
    {
        public static bool DebugMode = true; // Set to true to enable debug visuals

        private static Texture2D magicPixel;

        // Initialize the magicPixel texture
        private static void EnsureMagicPixel()
        {
            if (magicPixel == null)
            {
                magicPixel = new Texture2D(Main.instance.GraphicsDevice, 1, 1);
                magicPixel.SetData(new[] { Color.White });
            }
        }

        public static void Unload()
        {
            magicPixel?.Dispose();
            magicPixel = null;
        }

        private static bool IsOutOfBounds(Point tilePos)
        {
            return tilePos.X < 0 || tilePos.Y < 0 || tilePos.X >= Main.maxTilesX || tilePos.Y >= Main.maxTilesY;
        }
        public static void DrawBlackout(LOSPlayer player)
        {
            if (!DebugMode) return;

            EnsureMagicPixel();

            var spriteBatch = Main.spriteBatch;

            // ✅ DO NOT call spriteBatch.Begin() or End() here

            // 🔴 TEST: Red square at top-left to confirm it works
            spriteBatch.Draw(magicPixel, new Rectangle(100, 100, 40, 40), Color.Red);

            // 🔹 Raycasting parameters
            int rayCount = 360;
            float maxDistance = 20f;
            Vector2 playerTilePos = player.Player.Center / 16f;

            for (int i = 0; i < rayCount; i++)
            {
                float angle = MathHelper.ToRadians(i * (360f / rayCount));
                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                for (float distance = 0; distance < maxDistance; distance += 0.5f)
                {
                    Vector2 currentPos = playerTilePos + direction * distance;
                    Point tilePos = currentPos.ToPoint();

                    if (IsOutOfBounds(tilePos))
                        break;

                    if (LOSUtils.IsTileBlockingView(tilePos.X, tilePos.Y))
                    {
                        Vector2 tileScreenPos = tilePos.ToVector2() * 16 - Main.screenPosition;
                        spriteBatch.Draw(magicPixel, new Rectangle((int)tileScreenPos.X, (int)tileScreenPos.Y, 16, 16), Color.Red);
                        break;
                    }

                    Vector2 debugScreenPos = currentPos * 16 - Main.screenPosition;
                    spriteBatch.Draw(magicPixel, new Rectangle((int)debugScreenPos.X, (int)debugScreenPos.Y, 4, 4), Color.Yellow);
                }
            }
        }
    }
}

