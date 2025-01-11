using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using System;


namespace LOSMod
{
    public static class TileBlackoutSystem
    {
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
    int rayCount = 360; // Number of rays
    float maxDistance = 20f; // Maximum distance for rays (in tiles)
    Vector2 playerTilePos = player.Player.Center / 16f;

    for (int i = 0; i < rayCount; i++)
    {
        float angle = MathHelper.ToRadians(i); // Convert angle to radians
        Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

        for (float distance = 0; distance < maxDistance; distance += 0.5f)
        {
            Vector2 currentPos = playerTilePos + direction * distance;
            Point tilePos = currentPos.ToPoint();

            if (tilePos.X < 0 || tilePos.Y < 0 || tilePos.X >= Main.maxTilesX || tilePos.Y >= Main.maxTilesY)
                break;

            if (LOSUtils.IsTileBlockingView(tilePos.X, tilePos.Y))
            {
                // Black out the obstructing tile
                Vector2 tileScreenPos = new Vector2(tilePos.X * 16 - Main.screenPosition.X, tilePos.Y * 16 - Main.screenPosition.Y);
                spriteBatch.Draw(magicPixel, new Rectangle((int)tileScreenPos.X, (int)tileScreenPos.Y, 16, 16), Color.Black * 0.8f);
                break; // Stop the ray when hitting a blocking tile
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