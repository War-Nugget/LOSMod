// File: TileBlackoutSystem.cs
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LOSMod
{
    public static class TileBlackoutSystem
    {
        public static bool DebugMode = true;
        private static Texture2D magicPixel;
        private static RenderTarget2D blackoutTarget;
        private static SpriteBatch privateBatch;

        public static HashSet<Point> VisibleTiles { get; private set; } = new();
        public static HashSet<Point> OccludedTiles { get; private set; } = new();

        public static void Unload()
        {
            magicPixel?.Dispose();
            blackoutTarget?.Dispose();
            privateBatch?.Dispose();
            magicPixel = null;
            blackoutTarget = null;
            privateBatch = null;
            VisibleTiles.Clear();
            OccludedTiles.Clear();
        }

        private static void EnsureResources()
        {
            if (magicPixel == null)
            {
                magicPixel = new Texture2D(Main.instance.GraphicsDevice, 1, 1);
                magicPixel.SetData(new[] { Color.White });
            }

            if (blackoutTarget == null || blackoutTarget.Width != Main.screenWidth || blackoutTarget.Height != Main.screenHeight)
            {
                blackoutTarget?.Dispose();
                blackoutTarget = new RenderTarget2D(Main.instance.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            }

            if (privateBatch == null)
            {
                privateBatch = new SpriteBatch(Main.instance.GraphicsDevice);
            }
        }

        private static bool IsOutOfBounds(Point tilePos)
        {
            return tilePos.X < 0 || tilePos.Y < 0 || tilePos.X >= Main.maxTilesX || tilePos.Y >= Main.maxTilesY;
        }

        public static void DrawBlackout(LOSPlayer player)
        {
            if (!DebugMode || Main.dedServ || Main.gameMenu) return;

            EnsureResources();
            VisibleTiles.Clear();
            OccludedTiles.Clear();

            int rayCount = 720;
            float maxDistance = 30f;
            float rayStep = 0.25f; // finer resolution for smoother shadow shape
            Vector2 origin = player.Player.Center / 16f;

            for (int i = 0; i < rayCount; i++)
            {
                float angle = MathHelper.TwoPi * i / rayCount; // better than radians(360/rayCount)
                Vector2 direction = new((float)Math.Cos(angle), (float)Math.Sin(angle));
                bool blocked = false;

                for (float d = 0f; d < maxDistance; d += rayStep)
                {
                    Vector2 pos = origin + direction * d;
                    Point tile = pos.ToPoint(); // preserves finer granularity

                    if (IsOutOfBounds(tile)) break;

                    if (!blocked)
                    {
                        if (LOSUtils.IsTileBlockingView(tile.X, tile.Y))
                        {
                            blocked = true;
                            VisibleTiles.Add(tile); // ✅ first blocking tile is visible
                            continue;
                        }
                        VisibleTiles.Add(tile);
                    }
                    else
                    {
                        OccludedTiles.Add(tile);
                    }
                }
            }

            var device = Main.instance.GraphicsDevice;
            device.SetRenderTarget(blackoutTarget);
            device.Clear(Color.Transparent);

            privateBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            int startX = (int)(Main.screenPosition.X / 16f) - 1;
            int endX = (int)((Main.screenPosition.X + Main.screenWidth) / 16f) + 1;
            int startY = (int)(Main.screenPosition.Y / 16f) - 1;
            int endY = (int)((Main.screenPosition.Y + Main.screenHeight) / 16f) + 1;

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Point tile = new(x, y);
                    if (IsOutOfBounds(tile)) continue;

                    if (!VisibleTiles.Contains(tile))
                    {
                        Vector2 screenPos = tile.ToVector2() * 16 - Main.screenPosition;
                        Rectangle rect = new((int)screenPos.X, (int)screenPos.Y, 16, 16);

                        // Distance from player in tiles
                        float distance = Vector2.Distance(tile.ToVector2(), origin);
                        float fade = MathHelper.Clamp(distance / maxDistance, 0f, 1f);
                        int alpha = (int)MathHelper.Lerp(100f, 255f, fade);
                        Color shade = new Color(0, 0, 0, alpha);

                        privateBatch.Draw(magicPixel, rect, shade);
                    }
                }
            }

            privateBatch.End();
            device.SetRenderTarget(null);
        }


        public static void DrawFinalOverlay(SpriteBatch spriteBatch)
        {
            if (!DebugMode || Main.dedServ || Main.gameMenu || blackoutTarget == null) return;
            spriteBatch.Draw(blackoutTarget, Vector2.Zero, Color.White);
        }

    }
}
