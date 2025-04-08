using Terraria;
using Terraria.ModLoader;
using Terraria.GameInput;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;


namespace LOSMod
{
    public class LOSPlayer : ModPlayer
    {
        public RenderTarget2D blackTexture;

        private void EnsureBlackTexture()
        {
            if (blackTexture == null || blackTexture.Width != Main.screenWidth || blackTexture.Height != Main.screenHeight)
            {
                blackTexture?.Dispose();
                blackTexture = new RenderTarget2D(Main.instance.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            }
        }

        public override void Unload()
        {
            blackTexture?.Dispose();
            blackTexture = null;
        }

        public override void PreUpdate()
        {
            if (TileBlackoutSystem.DebugMode && !Main.dedServ)
            {
                TileBlackoutSystem.DrawBlackout(this); // Prepares blackoutTarget
            }
        }


        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Main.chatText.StartsWith("/toggle_debug"))
            {
                ToggleDebugMode();
                Main.chatText = "";
            }


            if (LOS.Instance != null && LOS.Instance.toggleDebugKey.JustPressed)
            {
                ToggleDebugMode();
            }
        }


        private void ToggleDebugMode()
        {
            TileBlackoutSystem.DebugMode = !TileBlackoutSystem.DebugMode;
            Main.NewText($"Debug mode is now {(TileBlackoutSystem.DebugMode ? "ENABLED" : "DISABLED")}", 255, 255, 0);
        }


    }
}



