using UnityEngine;

namespace Sotbify.MobileApp.ItemRenderer
{
    public interface IItemRenderer
    {
        Vector2Int Resolution { get; set; }
        RenderTexture Texture { get; }
        Transform Container { get; }
        bool RenderingIsActive { get; set; }
    }
}