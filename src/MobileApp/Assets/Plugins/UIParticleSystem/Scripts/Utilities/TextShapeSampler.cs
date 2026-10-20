using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TierStudios.UIParticleSystem.Utilities
{
    public static class TextShapeSampler
    {
        private const int MaxGlyphSample = 48;
        private const int MaxCacheEntries = 16;
        private static readonly Vector2[] EmptyPoints = new Vector2[0];

        private struct CacheKey : System.IEquatable<CacheKey>
        {
            public int graphicId;
            public int textHash;
            public int fontId;
            public int fontSize;
            public int width;
            public int height;
            public int layoutHash;
            public int thresholdQuantized;
            public bool emitFromEdge;

            public bool Equals(CacheKey other)
            {
                return graphicId == other.graphicId
                       && textHash == other.textHash
                       && fontId == other.fontId
                       && fontSize == other.fontSize
                       && width == other.width
                       && height == other.height
                       && layoutHash == other.layoutHash
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
                    int hash = graphicId;
                    hash = (hash * 397) ^ textHash;
                    hash = (hash * 397) ^ fontId;
                    hash = (hash * 397) ^ fontSize;
                    hash = (hash * 397) ^ width;
                    hash = (hash * 397) ^ height;
                    hash = (hash * 397) ^ layoutHash;
                    hash = (hash * 397) ^ thresholdQuantized;
                    hash = (hash * 397) ^ (emitFromEdge ? 1 : 0);
                    return hash;
                }
            }
        }

        private static readonly Dictionary<CacheKey, Vector2[]> cache = new Dictionary<CacheKey, Vector2[]>();
        private static readonly Queue<CacheKey> cacheOrder = new Queue<CacheKey>();
        private static readonly Dictionary<int, Color[]> atlasCache = new Dictionary<int, Color[]>();
        private static readonly Dictionary<int, Vector2Int> atlasSizeCache = new Dictionary<int, Vector2Int>();

        public static bool IsSupported(Graphic graphic)
        {
            return graphic is Text || graphic is TMP_Text;
        }

        public static bool TryGetRandomLocalPoint(Graphic graphic, float threshold, bool emitFromEdge, out Vector2 local)
        {
            Vector2[] points = GetOrBuildPoints(graphic, threshold, emitFromEdge);
            if (points == null || points.Length == 0)
            {
                local = Vector2.zero;
                return false;
            }

            local = points[Random.Range(0, points.Length)];
            return true;
        }

        public static void ClearCache()
        {
            cache.Clear();
            cacheOrder.Clear();
            atlasCache.Clear();
            atlasSizeCache.Clear();
        }

        private static Vector2[] GetOrBuildPoints(Graphic graphic, float threshold, bool emitFromEdge)
        {
            if (graphic == null)
                return null;

            CacheKey key = BuildKey(graphic, threshold, emitFromEdge);
            if (cache.TryGetValue(key, out Vector2[] cached))
                return cached;

            Vector2[] points = BuildPoints(graphic, Mathf.Clamp01(threshold), emitFromEdge);
            StoreCache(key, points);
            return points;
        }

        private static CacheKey BuildKey(Graphic graphic, float threshold, bool emitFromEdge)
        {
            var key = new CacheKey
            {
                graphicId = graphic.GetInstanceID(),
                thresholdQuantized = Mathf.RoundToInt(Mathf.Clamp01(threshold) * 1000f),
                emitFromEdge = emitFromEdge
            };

            Rect rect = graphic.rectTransform != null ? graphic.rectTransform.rect : new Rect();
            key.width = Mathf.RoundToInt(rect.width);
            key.height = Mathf.RoundToInt(rect.height);

            if (graphic is Text uiText)
            {
                key.textHash = uiText.text != null ? uiText.text.GetHashCode() : 0;
                key.fontId = uiText.font != null ? uiText.font.GetInstanceID() : 0;
                key.fontSize = uiText.fontSize;
                key.layoutHash = ((int)uiText.alignment)
                    ^ ((int)uiText.horizontalOverflow << 8)
                    ^ ((int)uiText.verticalOverflow << 12)
                    ^ (uiText.resizeTextForBestFit ? 1 : 0);
            }
            else if (graphic is TMP_Text tmp)
            {
                key.textHash = tmp.text != null ? tmp.text.GetHashCode() : 0;
                key.fontId = tmp.font != null ? tmp.font.GetInstanceID() : 0;
                key.fontSize = Mathf.RoundToInt(tmp.fontSize);
                key.layoutHash = ((int)tmp.alignment)
                    ^ ((int)tmp.overflowMode << 8)
                    ^ (tmp.enableWordWrapping ? 1 : 0)
                    ^ (Mathf.RoundToInt(tmp.characterSpacing) << 12);
            }

            return key;
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

        private static Vector2[] BuildPoints(Graphic graphic, float threshold, bool emitFromEdge)
        {
            var points = new List<Vector2>(256);

            if (graphic is Text uiText)
                SampleUIText(uiText, threshold, emitFromEdge, points);
            else if (graphic is TMP_Text tmp)
                SampleTMP(tmp, threshold, emitFromEdge, points);

            return points.Count > 0 ? points.ToArray() : EmptyPoints;
        }

        private static void SampleUIText(Text text, float threshold, bool emitFromEdge, List<Vector2> points)
        {
            if (text.font == null || string.IsNullOrEmpty(text.text))
                return;

            Vector2 extents = text.rectTransform.rect.size;
            TextGenerationSettings settings = text.GetGenerationSettings(extents);
            TextGenerator generator = text.cachedTextGenerator;
            generator.Populate(text.text, settings);

            IList<UIVertex> verts = generator.verts;
            if (verts == null || verts.Count < 4)
                return;

            Texture fontTex = text.mainTexture;
            if (!TryGetAtlasPixels(fontTex, out Color[] pixels, out int texW, out int texH))
                return;

            float ppu = text.pixelsPerUnit;
            if (ppu <= 0.0001f)
                ppu = 1f;

            for (int i = 0; i + 3 < verts.Count; i += 4)
            {
                Vector2 p0 = (Vector2)verts[i].position / ppu;
                Vector2 p1 = (Vector2)verts[i + 1].position / ppu;
                Vector2 p2 = (Vector2)verts[i + 2].position / ppu;
                Vector2 p3 = (Vector2)verts[i + 3].position / ppu;
                Vector2 uv0 = new Vector2(verts[i].uv0.x, verts[i].uv0.y);
                Vector2 uv1 = new Vector2(verts[i + 1].uv0.x, verts[i + 1].uv0.y);
                Vector2 uv2 = new Vector2(verts[i + 2].uv0.x, verts[i + 2].uv0.y);
                Vector2 uv3 = new Vector2(verts[i + 3].uv0.x, verts[i + 3].uv0.y);
                SampleGlyphQuad(p0, p1, p2, p3, uv0, uv1, uv2, uv3, pixels, texW, texH, threshold, emitFromEdge, points);
            }
        }

        private static void SampleTMP(TMP_Text tmp, float threshold, bool emitFromEdge, List<Vector2> points)
        {
            if (tmp.font == null || string.IsNullOrEmpty(tmp.text))
                return;

            tmp.ForceMeshUpdate(true, true);
            TMP_TextInfo info = tmp.textInfo;
            if (info == null || info.characterCount == 0)
                return;

            for (int i = 0; i < info.characterCount; i++)
            {
                TMP_CharacterInfo c = info.characterInfo[i];
                if (!c.isVisible)
                    continue;

                Texture tex = null;
                if (c.material != null)
                    tex = c.material.mainTexture;
                if (tex == null && tmp.font != null)
                    tex = tmp.font.atlasTexture;

                if (!TryGetAtlasPixels(tex, out Color[] pixels, out int texW, out int texH))
                    continue;

                SampleGlyphQuad(
                    c.bottomLeft, c.topLeft, c.topRight, c.bottomRight,
                    c.vertex_BL.uv, c.vertex_TL.uv, c.vertex_TR.uv, c.vertex_BR.uv,
                    pixels, texW, texH, threshold, emitFromEdge, points);
            }
        }

        private static void SampleGlyphQuad(
            Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3,
            Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3,
            Color[] pixels, int texW, int texH,
            float threshold, bool emitFromEdge, List<Vector2> points)
        {
            float maxW = Mathf.Max(Vector2.Distance(p0, p3), Vector2.Distance(p1, p2));
            float maxH = Mathf.Max(Vector2.Distance(p0, p1), Vector2.Distance(p3, p2));
            if (maxW < 0.5f || maxH < 0.5f)
                return;

            int dw = Mathf.Clamp(Mathf.RoundToInt(maxW), 4, MaxGlyphSample);
            int dh = Mathf.Clamp(Mathf.RoundToInt(maxH), 4, MaxGlyphSample);
            bool[] mask = new bool[dw * dh];

            for (int y = 0; y < dh; y++)
            {
                float v = (y + 0.5f) / dh;
                for (int x = 0; x < dw; x++)
                {
                    float u = (x + 0.5f) / dw;
                    Vector2 uv = Bilerp(uv0, uv1, uv2, uv3, u, v);
                    mask[y * dw + x] = SampleAlpha(pixels, texW, texH, uv) > threshold;
                }
            }

            for (int y = 0; y < dh; y++)
            {
                float v = (y + 0.5f) / dh;
                for (int x = 0; x < dw; x++)
                {
                    if (!mask[y * dw + x])
                        continue;
                    if (emitFromEdge && !IsEdgePixel(mask, x, y, dw, dh))
                        continue;

                    float u = (x + 0.5f) / dw;
                    points.Add(Bilerp(p0, p1, p2, p3, u, v));
                }
            }
        }

        private static Vector2 Bilerp(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float u, float v)
        {
            Vector2 left = Vector2.Lerp(a, b, v);
            Vector2 right = Vector2.Lerp(d, c, v);
            return Vector2.Lerp(left, right, u);
        }

        private static float SampleAlpha(Color[] pixels, int width, int height, Vector2 uv)
        {
            if (pixels == null || width <= 0 || height <= 0)
                return 0f;

            float fx = Mathf.Repeat(uv.x, 1f) * (width - 1);
            float fy = Mathf.Repeat(uv.y, 1f) * (height - 1);
            int x = Mathf.Clamp(Mathf.FloorToInt(fx), 0, width - 1);
            int y = Mathf.Clamp(Mathf.FloorToInt(fy), 0, height - 1);
            return pixels[y * width + x].a;
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

        private static bool TryGetAtlasPixels(Texture texture, out Color[] pixels, out int width, out int height)
        {
            pixels = null;
            width = 0;
            height = 0;
            if (texture == null)
                return false;

            int id = texture.GetInstanceID();
            if (atlasCache.TryGetValue(id, out pixels) && atlasSizeCache.TryGetValue(id, out Vector2Int size))
            {
                width = size.x;
                height = size.y;
                return pixels != null && pixels.Length > 0;
            }

            Texture2D source = texture as Texture2D;
            if (source == null)
                return false;

            Texture2D readable = source.isReadable ? source : MakeReadable(source);
            if (readable == null)
                return false;

            try
            {
                pixels = readable.GetPixels();
                width = readable.width;
                height = readable.height;
            }
            catch
            {
                ReleaseCopy(source, readable);
                return false;
            }

            ReleaseCopy(source, readable);

            if (pixels == null || pixels.Length == 0)
                return false;

            atlasCache[id] = pixels;
            atlasSizeCache[id] = new Vector2Int(width, height);
            return true;
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
