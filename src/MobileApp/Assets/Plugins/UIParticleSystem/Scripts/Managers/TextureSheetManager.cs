using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TierStudios.UIParticleSystem.Core;
using TierStudios.UIParticleSystem.Enums;

namespace TierStudios.UIParticleSystem.Managers
{
    public class TextureSheetManager
    {
        private Core.UIParticleSystem system;
        private Dictionary<int, Sprite> spriteCache = new Dictionary<int, Sprite>();
        private Texture2D cachedTexture = null;
        private int cachedTilesX = 0;
        private int cachedTilesY = 0;

        public TextureSheetManager(Core.UIParticleSystem particleSystem)
        {
            system = particleSystem;
        }

        public void UpdateParticleAnimation(Particle particle, float deltaTime)
        {
            if (!particle.hasAnimation || system.TextureSheetSettings.animationSpeed <= 0)
                return;

            if (system.TextureSheetSettings.useRandomSingleFrame)
                return;

            int frameCount = particle.endFrame - particle.startFrame + 1;
            if (frameCount <= 1)
                return;

            int targetFrame;

            if (system.TextureSheetSettings.timeMode == AnimationTimeMode.LifetimeNormalized)
            {
                float normalizedAge = particle.GetNormalizedAge();

                if (system.TextureSheetSettings.loopAnimation)
                {
                    float totalProgress = normalizedAge * system.TextureSheetSettings.animationSpeed;
                    float cycleProgress = totalProgress - Mathf.Floor(totalProgress);
                    int frameOffset = Mathf.Clamp(Mathf.FloorToInt(cycleProgress * frameCount), 0, frameCount - 1);
                    targetFrame = particle.startFrame + frameOffset;
                }
                else
                {
                    int frameOffset = Mathf.Clamp(Mathf.FloorToInt(normalizedAge * frameCount), 0, frameCount - 1);
                    targetFrame = particle.startFrame + frameOffset;
                }
            }
            else
            {
                particle.animationTime += deltaTime;
                float frameTime = 1f / system.TextureSheetSettings.animationSpeed;
                float animationProgress = particle.animationTime / frameTime;

                if (system.TextureSheetSettings.loopAnimation)
                {
                    int frameOffset = Mathf.FloorToInt(animationProgress) % frameCount;
                    targetFrame = particle.startFrame + frameOffset;
                }
                else
                {
                    int frameOffset = Mathf.Min(Mathf.FloorToInt(animationProgress), frameCount - 1);
                    targetFrame = particle.startFrame + frameOffset;
                }
            }

            particle.currentFrame = targetFrame;
        }

        public Sprite CreateSpriteFromTexture(Texture2D texture, int frameIndex)
        {
            if (texture == null || system.TextureSheetSettings.tilesX <= 0 || system.TextureSheetSettings.tilesY <= 0)
                return null;

            if (cachedTexture != texture || cachedTilesX != system.TextureSheetSettings.tilesX || cachedTilesY != system.TextureSheetSettings.tilesY)
            {
                ClearSpriteCache();
                cachedTexture = texture;
                cachedTilesX = system.TextureSheetSettings.tilesX;
                cachedTilesY = system.TextureSheetSettings.tilesY;
            }

            if (spriteCache.TryGetValue(frameIndex, out Sprite cachedSprite))
            {
                return cachedSprite;
            }

            int totalFrames = system.TextureSheetSettings.tilesX * system.TextureSheetSettings.tilesY;
            if (frameIndex >= totalFrames)
                frameIndex = frameIndex % totalFrames;

            float tileWidth = texture.width / (float)system.TextureSheetSettings.tilesX;
            float tileHeight = texture.height / (float)system.TextureSheetSettings.tilesY;

            int x = frameIndex % system.TextureSheetSettings.tilesX;
            int y = system.TextureSheetSettings.tilesY - 1 - (frameIndex / system.TextureSheetSettings.tilesX);

            Rect spriteRect = new Rect(
                x * tileWidth,
                y * tileHeight,
                tileWidth,
                tileHeight
            );

            Vector2 pivot = new Vector2(0.5f, 0.5f);
            Sprite newSprite = Sprite.Create(texture, spriteRect, pivot, 100f);

            if (spriteCache.Count < 256)
            {
                spriteCache[frameIndex] = newSprite;
            }

            return newSprite;
        }

        public void GetAnimationFrameRange(out int startFrame, out int endFrame)
        {
            int totalFrames = Mathf.Min(system.TextureSheetSettings.animationFrames, system.TextureSheetSettings.tilesX * system.TextureSheetSettings.tilesY);

            switch (system.TextureSheetSettings.animationCycle)
            {
                case AnimationCycle.WholeSheet:
                    startFrame = 0;
                    endFrame = totalFrames - 1;
                    break;

                case AnimationCycle.SingleRow:
                    int rowStart = Mathf.Clamp(system.TextureSheetSettings.singleRowIndex, 0, system.TextureSheetSettings.tilesY - 1) * system.TextureSheetSettings.tilesX;
                    startFrame = rowStart;
                    endFrame = Mathf.Min(rowStart + system.TextureSheetSettings.tilesX - 1, totalFrames - 1);
                    break;

                case AnimationCycle.SingleColumn:
                    int colIndex = Mathf.Clamp(system.TextureSheetSettings.singleColumnIndex, 0, system.TextureSheetSettings.tilesX - 1);
                    startFrame = colIndex;
                    endFrame = Mathf.Min(colIndex + (system.TextureSheetSettings.tilesY - 1) * system.TextureSheetSettings.tilesX, totalFrames - 1);
                    break;

                case AnimationCycle.Custom:
                    startFrame = Mathf.Clamp(system.TextureSheetSettings.customStartFrame, 0, totalFrames - 1);
                    endFrame = Mathf.Clamp(system.TextureSheetSettings.customEndFrame, startFrame, totalFrames - 1);
                    break;

                default:
                    startFrame = 0;
                    endFrame = totalFrames - 1;
                    break;
            }
        }

        public void ClearSpriteCache()
        {
            foreach (var sprite in spriteCache.Values)
            {
                if (sprite != null)
                {
                    if (Application.isPlaying)
                        Object.Destroy(sprite);
                    else
                        Object.DestroyImmediate(sprite);
                }
            }
            spriteCache.Clear();
        }
    }
}