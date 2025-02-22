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
            EnsureMagicPixel();

            var spriteBatch = Main.spriteBatch;
            var graphicsDevice = Main.instance.GraphicsDevice;

            // 🔥 Ensure we're drawing directly to the screen
            graphicsDevice.SetRenderTarget(null);
            graphicsDevice.Clear(Color.Transparent); 

            // ✅ Start drawing (Prevents crashes)
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            // 🔴 TEST: Draw a Large Red Square (Confirm Rendering Works)
            for (int i = 0; i < 5; i++)
            {
                spriteBatch.Draw(magicPixel, new Rectangle(100 + (i * 10), 100, 10, 10), Color.Red);
            }


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
                        // Main.NewText($"Blocked Tile at {tilePos.X}, {tilePos.Y}", 255, 100, 0);

                        // ✅ Fix Tile Position (Screen Space)
                        Vector2 tileScreenPos = new Vector2(
                            tilePos.X * 16 - (int)Main.screenPosition.X,
                            tilePos.Y * 16 - (int)Main.screenPosition.Y
                        );

                        // 🔥 Draw Red Tiles (Ensure Visibility)
                        spriteBatch.Draw(magicPixel, new Rectangle((int)tileScreenPos.X, (int)tileScreenPos.Y, 16, 16), Color.Red);
                        
                        break;
                    }

                    // ✅ Debug Mode: Draw Yellow Rays
                    if (DebugMode)
                    {
                        Vector2 debugScreenPos = new Vector2(
                            currentPos.X * 16 - Main.screenPosition.X,
                            currentPos.Y * 16 - Main.screenPosition.Y
                        );

                        // 🔥 Make Debug Points Bigger for Visibility
                        spriteBatch.Draw(magicPixel, new Rectangle((int)debugScreenPos.X, (int)debugScreenPos.Y, 8, 8), Color.Yellow);
                    }
                }
            }

            // ✅ End drawing
            spriteBatch.End();
        }
    }
}

