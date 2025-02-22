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

        if (player.blackTexture == null)
            return;

        var spriteBatch = Main.spriteBatch;
        var graphicsDevice = Main.instance.GraphicsDevice;

        graphicsDevice.SetRenderTarget(player.blackTexture);
        graphicsDevice.Clear(Color.Transparent);

        spriteBatch.Begin();

        // Raycasting parameters
        int rayCount = 360; // Adjust if needed
        float maxDistance = 20f; // Maximum distance for rays (in tiles)
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
                    // Draw the blocking tile overlay
                    Vector2 tileScreenPos = new Vector2(tilePos.X * 16 - Main.screenPosition.X, tilePos.Y * 16 - Main.screenPosition.Y);
                    spriteBatch.Draw(magicPixel, new Rectangle((int)tileScreenPos.X, (int)tileScreenPos.Y, 16, 16), Color.Black * 0.8f);

                    // Debug visual: Highlight obstructed tiles
                    if (DebugMode)
                    {
                        spriteBatch.Draw(magicPixel, new Rectangle((int)tileScreenPos.X, (int)tileScreenPos.Y, 16, 16), Color.Red * 0.5f);
                    }

                    break; // Stop the ray when hitting a blocking tile
                }

                // Debug visual: Show ray points
                if (DebugMode)
                {
                    Vector2 debugScreenPos = currentPos * 16f - Main.screenPosition;
                    Dust.NewDustPerfect(debugScreenPos, DustID.Smoke, Vector2.Zero, 0, Color.Yellow, 1.0f);
                }
            }
        }

            spriteBatch.End();

            graphicsDevice.SetRenderTarget(null);

            // Draw the blackout texture on the screen
            spriteBatch.Begin();
            spriteBatch.Draw(player.blackTexture, Vector2.Zero, Color.White);
            spriteBatch.End();
        }       
    }
}
