using Terraria.ModLoader;
using Terraria;

namespace LOSMod
{
    public class LOS : Mod
    {
        public override void Load()
        {
            Logger.Info("Line of Sight Mod Loaded");
        }

        public override void Unload()
        {
            Logger.Info("Line of Sight Mod Unloaded");
        }

        public override void AddRecipes()
        {
            // Add a custom chat command for debugging
            On.Terraria.Main.DoChatInput += (orig) =>
            {
                orig(); // Call the original method

                // Check if the player typed "/toggle_debug"
                if (Main.chatText.StartsWith("/toggle_debug"))
                {
                    TileBlackoutSystem.DebugMode = !TileBlackoutSystem.DebugMode;
                    Main.NewText($"Debug mode {(TileBlackoutSystem.DebugMode ? "enabled" : "disabled")}", 255, 255, 0);

                    // Clear the chat text after the command
                    Main.chatText = "";
                }
            };
        }
    }
}
