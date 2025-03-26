// File: TileBlackoutSystem.cs
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using System;
using System.Collections.Generic;
using Terraria.ID;


namespace LOSMod
{
    public static class TileBlackoutSystem
    {
        public static bool DebugMode = true;
        private static Texture2D magicPixel;


        public static HashSet<Point> VisibleTiles { get; private set; } = new();


        public static void Unload()
        {
            magicPixel?.Dispose();
            magicPixel = null;
            VisibleTiles.Clear();
        }


        private static void EnsureMagicPixel()
        {
            if (magicPixel == null)
            {
                magicPixel = new Texture2D(Main.instance.GraphicsDevice, 1, 1);
                magicPixel.SetData(new[] { Color.White });
            }
        }


        private static bool IsOutOfBounds(Point tilePos)
        {
            return tilePos.X < 0 || tilePos.Y < 0 || tilePos.X >= Main.maxTilesX || tilePos.Y >= Main.maxTilesY;
        }


        public static bool IsVisible(int x, int y) => VisibleTiles.Contains(new Point(x, y));


        public static void DrawBlackout(LOSPlayer player)
        {
            if (!DebugMode) return;


            EnsureMagicPixel();
            var spriteBatch = Main.spriteBatch;
            VisibleTiles.Clear();


            int rayCount = 360;
            float maxDistance = 20f;
            Vector2 playerTilePos = player.Player.Center / 16f;


            // Scan for visible tiles
            for (int i = 0; i < rayCount; i++)
            {
                float angle = MathHelper.ToRadians(i * (360f / rayCount));
                Vector2 direction = new((float)Math.Cos(angle), (float)Math.Sin(angle));


                for (float distance = 0; distance < maxDistance; distance += 0.5f)
                {
                    Vector2 currentPos = playerTilePos + direction * distance;
                    Point tilePos = currentPos.ToPoint();


                    if (IsOutOfBounds(tilePos)) break;


                    if (LOSUtils.IsTileBlockingView(tilePos.X, tilePos.Y))
                    {
                        Vector2 tileScreenPos = tilePos.ToVector2() * 16 - Main.screenPosition;
                        spriteBatch.Draw(magicPixel, new Rectangle((int)tileScreenPos.X, (int)tileScreenPos.Y, 16, 16), Color.Red);
                        break;
                    }


                    VisibleTiles.Add(tilePos);


                    Vector2 debugScreenPos = currentPos * 16 - Main.screenPosition;
                    // spriteBatch.Draw(magicPixel, new Rectangle((int)debugScreenPos.X, (int)debugScreenPos.Y, 4, 4), Color.Yellow);
                }
            }


            // Fill entire screen width in tiles for blackout
            int tileStartX = (int)(Main.screenPosition.X / 16f) - 1;
            int tileEndX = (int)((Main.screenPosition.X + Main.screenWidth) / 16f) + 1;
            int tileStartY = (int)(Main.screenPosition.Y / 16f) - 1;
            int tileEndY = (int)((Main.screenPosition.Y + Main.screenHeight) / 16f) + 1;


            for (int x = tileStartX; x <= tileEndX; x++)
            {
                for (int y = tileStartY; y <= tileEndY; y++)
                {
                    Point tilePos = new(x, y);
                    if (IsOutOfBounds(tilePos)) continue;
                    if (VisibleTiles.Contains(tilePos)) continue;


                    Vector2 blackoutScreenPos = tilePos.ToVector2() * 16 - Main.screenPosition;
                    spriteBatch.Draw(magicPixel, new Rectangle((int)blackoutScreenPos.X, (int)blackoutScreenPos.Y, 16, 16), new Color(0, 0, 0, 180));
                }
            }
        }
    }
}





