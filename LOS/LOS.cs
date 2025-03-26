using Terraria.ModLoader;
using Microsoft.Xna.Framework.Input;


namespace LOSMod
{
    public class LOS : Mod
    {
        public static LOS Instance { get; private set; } // Ensure a static instance


        public ModKeybind toggleDebugKey;


        public override void Load()
        {
            Instance = this; // Set instance on load
            toggleDebugKey = KeybindLoader.RegisterKeybind(this, "Toggle Debug Mode", Keys.F12);
            Logger.Info("Line of Sight Mod Loaded");
        }


        public override void Unload()
        {
            Instance = null; // Clear instance on unload
            toggleDebugKey = null;
            Logger.Info("Line of Sight Mod Unloaded");
        }
    }
}



