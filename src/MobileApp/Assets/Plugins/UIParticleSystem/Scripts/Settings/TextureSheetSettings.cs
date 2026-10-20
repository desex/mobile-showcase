using UnityEngine;
using TierStudios.UIParticleSystem.Enums;

namespace TierStudios.UIParticleSystem.Settings
{
    [System.Serializable]
    public class TextureSheetSettings
    {
        public bool useTextureSheetAnimation = false;
        public Texture2D textureSheet;
        public int tilesX = 4;
        public int tilesY = 4;
        public int animationFrames = 16;
        public float animationSpeed = 10f;

        public bool randomStartFrame = false;
        public bool loopAnimation = true;
        public AnimationCycle animationCycle = AnimationCycle.WholeSheet;

        public int customStartFrame = 0;
        public int customEndFrame = 15;
        public int singleRowIndex = 0;
        public int singleColumnIndex = 0;

        public bool useRandomSingleFrame = false;

        public AnimationTimeMode timeMode = AnimationTimeMode.FramesPerSecond;
    }
}