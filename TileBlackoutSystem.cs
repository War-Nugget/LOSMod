using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;


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

            // Draw blackout over obstructed tiles
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    if (LOSUtils.IsTileBlockingView(x, y))
                    {
                        Vector2 tilePosition = new Vector2(x * 16 - Main.screenPosition.X, y * 16 - Main.screenPosition.Y);
                        spriteBatch.Draw(magicPixel, new Rectangle((int)tilePosition.X, (int)tilePosition.Y, 16, 16), Color.Black * 0.8f);
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
