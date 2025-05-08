using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using System.Collections.Generic;

namespace LOSMod
{
    public class LOSInterfaceLayer : ModSystem
    {
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mapIndex  = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Map / Minimap"));

            if (mapIndex  != -1)
            {
            layers.Insert(mapIndex, new LegacyGameInterfaceLayer(
                "LOSMod: Blackout Overlay",
                delegate
                {
                    if (TileBlackoutSystem.DebugMode && !Main.gameMenu && !Main.dedServ)
                    {
                        TileBlackoutSystem.DrawFinalOverlay_Internal(); // draw to renderTarget
                        TileBlackoutSystem.DrawFinalOverlayToScreen(Main.spriteBatch); // only draw the final texture here
                    }
                    return true;
                },
                InterfaceScaleType.Game)
                
                );
            }
        }
    }
}
