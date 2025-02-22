using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;


namespace LOSMod
{
    public class LOS : Mod
    {
        private ModKeybind toggleDebugKey;

        public override void Load()
        {
            base.Load();

    
            {

                if (Main.chatText.StartsWith("/toggle_debug"))
                {
                    Main.NewText("Debug mode toggled");
                    // Toggle debug mode
                    TileBlackoutSystem.DebugMode = !TileBlackoutSystem.DebugMode;
                    Main.NewText($"Debug mode {(TileBlackoutSystem.DebugMode ? "enabled" : "disabled")}", 255, 255, 0);

                    // Clear the chat input
                    Main.chatText = "";
                }
            };

            // Register a keybind for toggling debug ]
            mode
            toggleDebugKey = KeybindLoader.RegisterKeybind(this, "Toggle Debug Mode", Microsoft.Xna.Framework.Input.Keys.F12);

            Logger.Info("Line of Sight Mod Loaded");
        }

        // public override void Unload()
        // {
        //     base.Unload();

        //     // Clean up keybind references
        //     toggleDebugKey = null;
        //     Logger.Info("Line of Sight Mod Unloaded");
        // }


        private void ToggleDebugMode()
        {
            // Toggle the debug mode
            TileBlackoutSystem.DebugMode = !TileBlackoutSystem.DebugMode;
            Main.NewText($"Debug mode {(TileBlackoutSystem.DebugMode ? "enabled" : "disabled")}", 255, 255, 0);
        }
    }
}
