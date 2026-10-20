using System.Collections.Generic;
using UnityEngine;

namespace TierStudios.UIParticleSystem.Utilities
{
    public static class SpriteShapeSampler
    {
        private const int MaxSampleDimension = 128;
        private const int MaxCacheEntries = 16;
        private static readonly Vector2[] EmptyPoints = new Vector2[0];

        private struct CacheKey : System.IEquatable<CacheKey>
        {
            public int spriteId;
            public int thresholdQuantized;
            public bool emitFromEdge;

            public CacheKey(int spriteId, float threshold, bool emitFromEdge)
            {
                this.spriteId = spriteId;
                thresholdQuantized = Mathf.RoundToInt(Mathf.Clamp01(threshold) * 1000f);
                this.emitFromEdge = emitFromEdge;
            }

            public bool Equals(CacheKey other)
            {
                return spriteId == other.spriteId
                       && thresholdQuantized == other.thresholdQuantized
                       && emitFromEdge == other.emitFromEdge;
            }

            public override bool Equals(object obj)
            {
                return obj is CacheKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = spriteId;
                    hash = (hash * 397) ^ thresholdQuantized;
                    hash = (hash * 397) ^ (emitFromEdge ? 1 : 0);
                    return hash;
                }
            }
        }

        private static readonly Dictionary<CacheKey, Vector2[]> cache = new Dictionary<CacheKey, Vector2[]>();
        private static readonly Queue<CacheKey> cacheOrder = new Queue<CacheKey>();

        public static Vector2 GetRandomPoint(Sprite sprite, float threshold, bool emitFromEdge, Vector2 size)
        {
            if (!TryGetRandomRelativePoint(sprite, threshold, emitFromEdge, out Vector2 relative))
                return Vector2.zero;

            return new Vector2(relative.x * size.x, relative.y * size.y);
        }

        public static bool TryGetRandomRelativePoint(Sprite sprite, float threshold, bool emitFromEdge, out Vector2 relative)
        {
            Vector2[] points = GetOrBuildPoints(sprite, threshold, emitFromEdge);
            if (points == null || points.Length == 0)
            {
                relative = Vector2.zero;
                return false;
            }

            relative = points[Random.Range(0, points.Length)];
            return true;
        }

        public static void ClearCache()
        {
            cache.Clear();
            cacheOrder.Clear();
        }

        private static Vector2[] GetOrBuildPoints(Sprite sprite, float threshold, bool emitFromEdge)
        {
            if (sprite == null || sprite.texture == null)
                return null;

            var key = new CacheKey(sprite.GetInstanceID(), threshold, emitFromEdge);
            if (cache.TryGetValue(key, out Vector2[] cached))
                return cached;

            Vector2[] points = BuildPoints(sprite, Mathf.Clamp01(threshold), emitFromEdge);
            StoreCache(key, points);
            return points;
        }

        private static void StoreCache(CacheKey key, Vector2[] points)
        {
            if (cache.Count >= MaxCacheEntries && cacheOrder.Count > 0)
            {
                CacheKey oldest = cacheOrder.Dequeue();
                cache.Remove(oldest);
            }

            cache[key] = points;
            cacheOrder.Enqueue(key);
        }

        private static Vector2[] BuildPoints(Sprite sprite, float threshold, bool emitFromEdge)
        {
            Texture2D texture = sprite.texture;
            Rect textureRect = sprite.textureRect;
            int x = Mathf.Clamp(Mathf.FloorToInt(textureRect.x), 0, Mathf.Max(0, texture.width - 1));
            int y = Mathf.Clamp(Mathf.FloorToInt(textureRect.y), 0, Mathf.Max(0, texture.height - 1));
            int w = Mathf.Clamp(Mathf.FloorToInt(textureRect.width), 1, texture.width - x);
            int h = Mathf.Clamp(Mathf.FloorToInt(textureRect.height), 1, texture.height - y);

            Texture2D readable = texture.isReadable ? texture : MakeReadable(texture);
            if (readable == null)
                return EmptyPoints;

            Color[] pixels;
            try
            {
                pixels = readable.GetPixels(x, y, w, h);
            }
            catch
            {
                ReleaseCopy(texture, readable);
                return EmptyPoints;
            }

            ReleaseCopy(texture, readable);

            int maxDim = Mathf.Max(w, h);
            int dw = w;
            int dh = h;
            if (maxDim > MaxSampleDimension)
            {
                float downscale = MaxSampleDimension / (float)maxDim;
                dw = Mathf.Max(1, Mathf.RoundToInt(w * downscale));
                dh = Mathf.Max(1, Mathf.RoundToInt(h * downscale));
            }

            bool[] mask = new bool[dw * dh];
            for (int dy = 0; dy < dh; dy++)
            {
                int sy = Mathf.Min(h - 1, (dy * h) / dh);
                for (int dx = 0; dx < dw; dx++)
                {
                    int sx = Mathf.Min(w - 1, (dx * w) / dw);
                    mask[dy * dw + dx] = pixels[sy * w + sx].a > threshold;
                }
            }

            Rect spriteRect = sprite.rect;
            if (spriteRect.width <= 0f || spriteRect.height <= 0f)
                return EmptyPoints;

            Vector2 pivotN = new Vector2(sprite.pivot.x / spriteRect.width, sprite.pivot.y / spriteRect.height);
            Vector2 offset = sprite.textureRectOffset;
            var points = new List<Vector2>(dw * dh / 4);

            for (int dy = 0; dy < dh; dy++)
            {
                for (int dx = 0; dx < dw; dx++)
                {
                    if (!mask[dy * dw + dx])
                        continue;

                    if (emitFromEdge && !IsEdgePixel(mask, dx, dy, dw, dh))
                        continue;

                    float u = (dx + 0.5f) / dw;
                    float v = (dy + 0.5f) / dh;
                    float spritePixelX = offset.x + u * textureRect.width;
                    float spritePixelY = offset.y + v * textureRect.height;
                    Vector2 relative = new Vector2(
                        spritePixelX / spriteRect.width,
                        spritePixelY / spriteRect.height) - pivotN;
                    points.Add(relative);
                }
            }

            return points.Count > 0 ? points.ToArray() : EmptyPoints;
        }

        private static bool IsEdgePixel(bool[] mask, int x, int y, int width, int height)
        {
            if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                return true;

            return !mask[y * width + (x - 1)]
                   || !mask[y * width + (x + 1)]
                   || !mask[(y - 1) * width + x]
                   || !mask[(y + 1) * width + x];
        }

        private static Texture2D MakeReadable(Texture2D source)
        {
            if (source == null)
                return null;

            RenderTexture rt = RenderTexture.GetTemporary(
                source.width, source.height, 0,
                RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            rt.filterMode = FilterMode.Point;

            RenderTexture prev = RenderTexture.active;
            Graphics.Blit(source, rt);
            RenderTexture.active = rt;

            var copy = new Texture2D(source.width, source.height, TextureFormat.ARGB32, false, false);
            copy.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
            copy.Apply();

            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);
            return copy;
        }

        private static void ReleaseCopy(Texture2D original, Texture2D copy)
        {
            if (copy == null || copy == original)
                return;

            if (Application.isPlaying)
                Object.Destroy(copy);
            else
                Object.DestroyImmediate(copy);
        }
    }
}
