using Terraria.ModLoader;

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
    }
}
