using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
// using Terraria.DataStructures;
// using Microsoft.Xna.Framework;

namespace LOSMod
{
    public class LOSPlayer : ModPlayer
    {
        // Field to hold the blackout texture
        public RenderTarget2D blackTexture;

        // Ensure the RenderTarget2D is created only when needed
        private void EnsureBlackTexture()
        {
            if (blackTexture == null || blackTexture.Width != Main.screenWidth || blackTexture.Height != Main.screenHeight)
            {
                blackTexture?.Dispose(); // Dispose of old texture if it exists
                blackTexture = new RenderTarget2D(Main.instance.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            }
        }

        public override void PostUpdate()
        {
            // Perform any updates needed before rendering
            EnsureBlackTexture(); // Ensure the blackout texture is initialized and updated
        }

        public override void Unload()
        {
            // Properly dispose of the texture when unloading the mod
            blackTexture?.Dispose();
            blackTexture = null;
        }
    }
}
