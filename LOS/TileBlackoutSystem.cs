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
        private static RenderTarget2D lightingTarget;

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

            if (lightingTarget == null || lightingTarget.Width != Main.screenWidth || lightingTarget.Height != Main.screenHeight)
            {
                lightingTarget?.Dispose();
                lightingTarget = new RenderTarget2D(Main.instance.GraphicsDevice, Main.screenWidth, Main.screenHeight);
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

        private static readonly BlendState MultiplyBlend = new BlendState
        {
            ColorSourceBlend = Blend.DestinationColor,
            ColorDestinationBlend = Blend.Zero,
            ColorBlendFunction = BlendFunction.Add
        };


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



        
// FIX THIS BLOCK  ---------------------------------------------------------------------------------------------------
        public static void DrawFinalOverlay_Internal()
        {
            if (!DebugMode || Main.dedServ || Main.gameMenu) return;

            EnsureResources();
            EnsureRadialTexture();

            var device = Main.instance.GraphicsDevice;

            // Draw blackout and radial light into a render target
            device.SetRenderTarget(lightingTarget);
            device.Clear(Color.Transparent);

            privateBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);



            // Apply radial light using Multiply
            Vector2 playerCenter = Main.LocalPlayer.Center - Main.screenPosition;
            Vector2 origin = new Vector2(radialLight.Width, radialLight.Height) / 2f;
            privateBatch.End();

            // 1. Fill render target with black
            privateBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            privateBatch.Draw(magicPixel, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black);
            privateBatch.End();

            // 2. Apply radial light using custom multiply blend
            privateBatch.Begin(SpriteSortMode.Immediate, MultiplyBlend); // <-- uses your custom MultiplyBlend
            privateBatch.Draw(radialLight, playerCenter, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
            privateBatch.End();



            device.SetRenderTarget(null); // reset back to screen
   
        }


        private static Texture2D radialLight;

        private static void EnsureRadialTexture()
        {
            if (radialLight != null) return;

            int size = 256;
            radialLight = new Texture2D(Main.instance.GraphicsDevice, size, size);
            Color[] data = new Color[size * size];
            Vector2 center = new Vector2(size / 2f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center) / (size / 2f);
                    float intensity = MathHelper.Clamp(1f - dist, 0f, 1f); // <-- reversed for brightness at center
                    intensity *= intensity; // smooth curve

                    // White color with reduced intensity (black edges, white center)
                    data[y * size + x] = new Color(intensity, intensity, intensity, 1f); // <-- alpha = 1

                }
            }

            radialLight.SetData(data);
        }
        public static void DrawFinalOverlayToScreen(SpriteBatch spriteBatch)
        {
            if (lightingTarget == null) return;

            // Only use Main.spriteBatch here to copy your pre-rendered texture to the screen
            spriteBatch.Draw(lightingTarget, Vector2.Zero, Color.White);
        }
    }
}
