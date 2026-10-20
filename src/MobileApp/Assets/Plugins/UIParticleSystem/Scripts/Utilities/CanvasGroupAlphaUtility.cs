using UnityEngine;
using UnityEngine.UI;

namespace TierStudios.UIParticleSystem.Utilities
{
    public static class CanvasGroupAlphaUtility
    {
        public static float GetInheritedAlpha(Transform transform)
        {
            float alpha = 1f;
            Transform current = transform;
            while (current != null)
            {
                var group = current.GetComponent<CanvasGroup>();
                if (group != null)
                {
                    alpha *= group.alpha;
                    if (group.ignoreParentGroups)
                        break;
                }
                current = current.parent;
            }
            return alpha;
        }

        public static void ApplyToRenderer(Graphic graphic)
        {
            if (graphic == null)
                return;

            var renderer = graphic.GetComponent<CanvasRenderer>();
            if (renderer == null)
                return;

            Color c = graphic.color;
            c.a *= GetInheritedAlpha(graphic.transform);
            renderer.SetColor(c);
        }
    }
}
