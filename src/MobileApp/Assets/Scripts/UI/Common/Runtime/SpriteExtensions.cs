using UnityEngine;

namespace Sotbifay.Launcher.UI.Common
{
    public static class SpriteExtensions
    {
        public static Sprite CreateSprite(this Texture2D texture)
        {
            return Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f
            );
        }
    }
}