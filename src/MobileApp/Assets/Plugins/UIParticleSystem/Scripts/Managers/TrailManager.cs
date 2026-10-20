using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using System.Collections.Generic;
using TierStudios.UIParticleSystem.Core;

namespace TierStudios.UIParticleSystem.Managers
{
    public class TrailManager : System.IDisposable
    {
        private Core.UIParticleSystem _system;
        private UIParticleTrailRenderer _trailRenderer;

        private NativeArray<Vector3> _vertices;
        private NativeArray<Color> _colors;
        private NativeArray<Vector2> _uvs;
        private NativeArray<int> _triangles;

        private int _vertCount;
        private int _triCount;

        private const int INITIAL_VERT_CAPACITY = 512;
        private const int INITIAL_TRI_CAPACITY = 1024;

        private Vector2 _cachedPerp;
        private readonly Dictionary<int, Color> _frameColorCache = new Dictionary<int, Color>(32);
        private bool _frameColorCacheBuilt;

        private bool _disposed;

        public TrailManager(Core.UIParticleSystem particleSystem)
        {
            _system = particleSystem;
            AllocateBuffers(INITIAL_VERT_CAPACITY, INITIAL_TRI_CAPACITY);
        }

        #region Buffer Yönetimi

        private void AllocateBuffers(int vertCap, int triCap)
        {
            if (_vertices.IsCreated) _vertices.Dispose();
            if (_colors.IsCreated) _colors.Dispose();
            if (_uvs.IsCreated) _uvs.Dispose();
            if (_triangles.IsCreated) _triangles.Dispose();

            _vertices = new NativeArray<Vector3>(vertCap, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            _colors = new NativeArray<Color>(vertCap, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            _uvs = new NativeArray<Vector2>(vertCap, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            _triangles = new NativeArray<int>(triCap, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

            _disposed = false;
        }

        private void EnsureCapacity(int requiredVerts, int requiredTris)
        {          
            bool buffersValid = _vertices.IsCreated && _colors.IsCreated
                             && _uvs.IsCreated && _triangles.IsCreated;

            if (!buffersValid)
            {
                AllocateBuffers(
                    Mathf.Max(INITIAL_VERT_CAPACITY, requiredVerts),
                    Mathf.Max(INITIAL_TRI_CAPACITY, requiredTris));
                return;
            }

            bool needResize = false;
            int newVertCap = _vertices.Length;
            int newTriCap = _triangles.Length;

            while (newVertCap < requiredVerts) { newVertCap *= 2; needResize = true; }
            while (newTriCap < requiredTris) { newTriCap *= 2; needResize = true; }

            if (needResize)
                AllocateBuffers(newVertCap, newTriCap);
        }

        #endregion

        #region Renderer manager

        private bool HasRenderer
        {
            get
            {
                if (_trailRenderer == null)
                    return false;
                if (_trailRenderer.gameObject == null)
                {
                    _trailRenderer = null;
                    return false;
                }
                return true;
            }
        }

        private bool EnsureRenderer()
        {
            DestroyOrphanTrails();

            if (HasRenderer)
            {
                PlaceTrailAsFirstChild(_trailRenderer.transform);
                return true;
            }
            if (_system == null || !_system.CanModifyChildHierarchy)
                return false;

            var go = new GameObject("UIParticleTrail");
            go.hideFlags = HideFlags.HideAndDontSave;

            var rt = go.AddComponent<RectTransform>();
            PlaceTrailAsFirstChild(rt);

            var ignoreLayout = go.AddComponent<LayoutElement>();
            ignoreLayout.ignoreLayout = true;

            _trailRenderer = go.AddComponent<UIParticleTrailRenderer>();
            _trailRenderer.raycastTarget = false;
            _trailRenderer.maskable = _system.UseMasked;

            Material mat = _system.TrailSettings.trailMaterial != null
                ? _system.TrailSettings.trailMaterial
                : new Material(Shader.Find("UI/Default"));

            _trailRenderer.material = mat;
            _system.BringParticleLayerToFront();
            return true;
        }

        private void PlaceTrailAsFirstChild(Transform trailTransform)
        {
            if (trailTransform == null || _system == null) return;
            if (!_system.CanModifyChildHierarchy)
                return;

            Transform systemTransform = _system.transform;
            if (trailTransform.parent != systemTransform)
                trailTransform.SetParent(systemTransform, false);

            SyncTrailRect(trailTransform as RectTransform);
            if (trailTransform.GetSiblingIndex() != 0)
                trailTransform.SetAsFirstSibling();
            _system.BringParticleLayerToFront();
        }

        private void SyncTrailRect(RectTransform trailRect)
        {
            if (trailRect == null || _system == null) return;

            var sysRect = _system.rectTransform;
            if (sysRect == null) return;

            trailRect.anchorMin = Vector2.zero;
            trailRect.anchorMax = Vector2.one;
            trailRect.pivot = sysRect.pivot;
            trailRect.anchoredPosition = Vector2.zero;
            trailRect.sizeDelta = Vector2.zero;
            trailRect.offsetMin = Vector2.zero;
            trailRect.offsetMax = Vector2.zero;
            trailRect.localRotation = Quaternion.identity;
            trailRect.localScale = Vector3.one;
        }

        private void DestroyOrphanTrails()
        {
            if (_system == null) return;

            Transform system = _system.transform;
            Transform parent = system.parent;

            if (parent != null)
            {
                for (int i = parent.childCount - 1; i >= 0; i--)
                {
                    Transform child = parent.GetChild(i);
                    if (IsOrphanTrail(child))
                        SafeDestroy(child.gameObject);
                }
            }

            for (int i = system.childCount - 1; i >= 0; i--)
            {
                Transform child = system.GetChild(i);
                if (IsOrphanTrail(child))
                    SafeDestroy(child.gameObject);
            }
        }

        private bool IsOrphanTrail(Transform candidate)
        {
            if (candidate == null || candidate.name != "UIParticleTrail")
                return false;
            if (_trailRenderer != null && candidate == _trailRenderer.transform)
                return false;

            var owner = candidate.GetComponentInParent<Core.UIParticleSystem>();
            if (owner != null && owner != _system)
                return false;

            return candidate.GetComponent<UIParticleTrailRenderer>() != null;
        }

        public void RefreshMaskable()
        {
            if (!HasRenderer) return;

            _trailRenderer.maskable = _system.UseMasked;
            _trailRenderer.RecalculateMasking();

            var renderer = _trailRenderer.GetComponent<CanvasRenderer>();
            if (renderer != null)
                renderer.SetMaterial(_trailRenderer.materialForRendering, _trailRenderer.mainTexture);
        }

        private void DestroyRenderer()
        {
            if (_trailRenderer != null)
            {
                SafeDestroy(_trailRenderer.gameObject);
                _trailRenderer = null;
            }
        }

        #endregion

        #region Partikül Trail Güncelleme

        public void UpdateParticleTrail(Particle particle, float deltaTime)
        {
            if (particle.trail == null)
            {
                particle.trail = new ParticleTrail(_system.TrailSettings.trailMaxPoints);
                particle.trail.width = _system.TrailSettings.trailWidth;
                ApplyTrailColor(particle);
                particle.trail.AddPoint(GetParticlePosition(particle));
            }

            var trail = particle.trail;
            var colorMode = _system.TrailSettings.trailColorMode;

            if (colorMode == UIParticleSystem.Enums.TrailColorMode.InheritParticleColor ||
        colorMode == UIParticleSystem.Enums.TrailColorMode.InheritTextureColor)
            {
                ApplyTrailColor(particle);
            }

            particle.trailUpdateTimer += deltaTime;
            if (particle.trailUpdateTimer >= _system.TrailSettings.trailUpdateInterval)
            {
                Vector2 pos = GetParticlePosition(particle);
                float minDistSqr = _system.TrailSettings.trailMinimumVertexDistance
                                   * _system.TrailSettings.trailMinimumVertexDistance;
                bool shouldAdd = true;

                if (trail.GetPointCount() > 0)
                {
                    Vector2 last = trail.GetLastPosition();
                    float dx = pos.x - last.x, dy = pos.y - last.y;
                    if (dx * dx + dy * dy < minDistSqr)
                        shouldAdd = false;
                }

                if (shouldAdd)
                {
                    trail.AddPoint(pos);
                    if (trail.GetPointCount() > _system.TrailSettings.trailMaxPoints)
                        trail.TrimToMaxPoints(_system.TrailSettings.trailMaxPoints);
                }

                particle.trailUpdateTimer = 0f;
            }

            trail.UpdateTimes(deltaTime, _system.TrailSettings.trailLifetime);
        }

        #endregion

        #region Mesh Build

        public void RebuildTrailMesh()
        {
            var particles = _system.ActiveParticles;
            int particleCount = particles.Count;

            int estimatedVerts = 0;
            int estimatedTris = 0;
            bool anyTrailData = false;

            for (int i = 0; i < particleCount; i++)
            {
                var p = particles[i];
                if (p == null || !p.isAlive || p.trail == null || !p.trail.HasValidTrail()) continue;

                anyTrailData = true;
                int pts = p.trail.GetPointCount();
                int segments = pts;
                estimatedVerts += segments * 4;
                estimatedTris += segments * 6;
            }

            if (!anyTrailData)
            {
                if (HasRenderer)
                    _trailRenderer.Clear();
                return;
            }

            if (!EnsureRenderer()) return;

            EnsureCapacity(estimatedVerts, estimatedTris);

            _vertCount = 0;
            _triCount = 0;

            for (int i = 0; i < particleCount; i++)
            {
                var p = particles[i];
                if (p != null && p.isAlive && p.trail != null && p.trail.HasValidTrail())
                    BuildSegments(p.trail, p);
            }

            ApplyMesh();
        }

        private void BuildSegments(ParticleTrail trail, Particle particle)
        {
            int pointCount = trail.GetPointCount();
            if (pointCount < 2) return;

            Vector2 currentPos = GetParticlePosition(particle);
            Vector2 lastTrailPos = trail.GetLastPosition();
            float hdx = currentPos.x - lastTrailPos.x;
            float hdy = currentPos.y - lastTrailPos.y;
            bool hasLiveHead = (hdx * hdx + hdy * hdy) > 0.5f;

            int renderPointCount = hasLiveHead ? pointCount + 1 : pointCount;
            if (renderPointCount < 2) return;

            float sizeRatio = particle.initialSize2D.x > 0.001f
                ? Mathf.Clamp(particle.size2D.x / particle.initialSize2D.x, 0f, 10f)
                : 1f;

            float invCount = 1f / (renderPointCount - 1);
            float trailLifetime = _system.TrailSettings.trailLifetime;

            for (int i = 0; i < renderPointCount - 1; i++)
            {
                Vector2 cur, nxt;
                float timeCur, timeNxt;

                if (i < pointCount - 1)
                {
                    cur = trail.GetPoint(i);
                    nxt = trail.GetPoint(i + 1);
                    timeCur = trail.GetTime(i);
                    timeNxt = trail.GetTime(i + 1);
                }
                else if (hasLiveHead && i == pointCount - 1)
                {
                    cur = trail.GetPoint(pointCount - 1);
                    nxt = currentPos;
                    timeCur = trail.GetTime(pointCount - 1);
                    timeNxt = 0f;
                }
                else break;

                float segDx = nxt.x - cur.x;
                float segDy = nxt.y - cur.y;
                float sqrDist = segDx * segDx + segDy * segDy;
                if (sqrDist < 0.01f) continue;

                float invLen = 1f / Mathf.Sqrt(sqrDist);
                _cachedPerp.x = -segDy * invLen;
                _cachedPerp.y = segDx * invLen;

                float progressCur = i * invCount;
                float progressNxt = (i + 1) * invCount;

                float wCur = Mathf.Max(0.1f,
                    trail.width * sizeRatio * _system.TrailSettings.trailWidthCurve.Evaluate(progressCur) * 0.5f);
                float wNxt = Mathf.Max(0.1f,
                    trail.width * sizeRatio * _system.TrailSettings.trailWidthCurve.Evaluate(progressNxt) * 0.5f);

                float pxCur = _cachedPerp.x * wCur, pyCur = _cachedPerp.y * wCur;
                float pxNxt = _cachedPerp.x * wNxt, pyNxt = _cachedPerp.y * wNxt;

                int v = _vertCount;
                _vertices[v] = new Vector3(cur.x + pxCur, cur.y + pyCur, 0f);
                _vertices[v + 1] = new Vector3(cur.x - pxCur, cur.y - pyCur, 0f);
                _vertices[v + 2] = new Vector3(nxt.x + pxNxt, nxt.y + pyNxt, 0f);
                _vertices[v + 3] = new Vector3(nxt.x - pxNxt, nxt.y - pyNxt, 0f);

                _uvs[v] = new Vector2(progressCur, 1f);
                _uvs[v + 1] = new Vector2(progressCur, 0f);
                _uvs[v + 2] = new Vector2(progressNxt, 1f);
                _uvs[v + 3] = new Vector2(progressNxt, 0f);

                Color c1 = Color.Lerp(trail.startColor, trail.endColor,
                               Mathf.Clamp01(timeCur / trailLifetime));
                Color c2 = Color.Lerp(trail.startColor, trail.endColor,
                               Mathf.Clamp01(timeNxt / trailLifetime));

                _colors[v] = _colors[v + 1] = c1;
                _colors[v + 2] = _colors[v + 3] = c2;

                int t = _triCount;
                _triangles[t] = v;
                _triangles[t + 1] = v + 2;
                _triangles[t + 2] = v + 1;
                _triangles[t + 3] = v + 1;
                _triangles[t + 4] = v + 2;
                _triangles[t + 5] = v + 3;

                _vertCount += 4;
                _triCount += 6;
            }
        }

        private void ApplyMesh()
        {
            if (_trailRenderer == null) return;

            Mesh mesh = _trailRenderer.TrailMesh;
            mesh.Clear();

            if (_vertCount == 0)
            {
                _trailRenderer.ApplyMesh(mesh);
                return;
            }

            mesh.SetVertices(_vertices, 0, _vertCount);
            mesh.SetColors(_colors, 0, _vertCount);
            mesh.SetUVs(0, _uvs, 0, _vertCount);
            mesh.SetIndices(_triangles, 0, _triCount, MeshTopology.Triangles, 0, false);

            Vector3 min = _vertices[0], max = _vertices[0];
            for (int i = 1; i < _vertCount; i++)
            {
                Vector3 v = _vertices[i];
                if (v.x < min.x) min.x = v.x; else if (v.x > max.x) max.x = v.x;
                if (v.y < min.y) min.y = v.y; else if (v.y > max.y) max.y = v.y;
            }
            min.z = max.z = 0f;
            mesh.bounds = new Bounds((min + max) * 0.5f, max - min + Vector3.one);

            _trailRenderer.ApplyMesh(mesh);
        }

        #endregion

        #region Renk Hesaplama

        private void ApplyTrailColor(Particle particle)
        {
            var trail = particle.trail;
            if (trail == null) return;

            switch (_system.TrailSettings.trailColorMode)
            {
                case UIParticleSystem.Enums.TrailColorMode.Color:
                    {
                        Color c = _system.TrailSettings.trailColor;
                        trail.startColor = c;
                        trail.endColor = c;
                        break;
                    }

                case UIParticleSystem.Enums.TrailColorMode.RandomColor:
                    {
                        Color c = Random.value < 0.5f
                            ? _system.TrailSettings.trailColor
                            : _system.TrailSettings.trailColorSecondary;
                        trail.startColor = c;
                        trail.endColor = c;
                        break;
                    }

                case UIParticleSystem.Enums.TrailColorMode.Gradient:
                    {
                        Gradient g = _system.TrailSettings.trailGradient;
                        trail.startColor = g.Evaluate(0f);
                        trail.endColor = g.Evaluate(1f);
                        break;
                    }

                case UIParticleSystem.Enums.TrailColorMode.RandomGradient:
                    {
                        Gradient g = Random.value < 0.5f
                            ? _system.TrailSettings.trailGradient
                            : _system.TrailSettings.trailGradientSecondary;
                        trail.startColor = g.Evaluate(0f);
                        trail.endColor = g.Evaluate(1f);
                        break;
                    }

                case UIParticleSystem.Enums.TrailColorMode.InheritParticleColor:
                    {
                        Color pc = particle.color;
                        trail.startColor = pc;
                        trail.endColor = new Color(pc.r, pc.g, pc.b, 0f);
                        break;
                    }

                case UIParticleSystem.Enums.TrailColorMode.InheritTextureColor:
                    {
                        Color rawTex = GetFrameColor(particle);
                        Color effective = IsEffectivelyWhite(rawTex) ? particle.color : rawTex * particle.color;
                        trail.startColor = effective;
                        trail.endColor = new Color(effective.r, effective.g, effective.b, 0f);
                        break;
                    }
            }
        }

        private static bool IsEffectivelyWhite(Color c)
            => c.r >= 0.95f && c.g >= 0.95f && c.b >= 0.95f;


        private Color GetFrameColor(Particle particle)
        {
            bool hasSheet = _system.TextureSheetSettings != null
                         && _system.TextureSheetSettings.useTextureSheetAnimation
                         && _system.TextureSheetSettings.textureSheet != null;

            if (hasSheet)
            {
                if (_frameColorCache.TryGetValue(particle.currentFrame, out Color c1)) return c1;
                BuildFrameColorCache();
                if (_frameColorCache.TryGetValue(particle.currentFrame, out Color c2)) return c2;
            }

            return SampleSpriteColor(_system.ParticleSprite);
        }

        private Color SampleSpriteColor(Sprite sprite)
        {
            if (sprite == null) return Color.white;

            int key = sprite.GetInstanceID();
            if (_frameColorCache.TryGetValue(key, out Color cached)) return cached;

            Texture2D tex = sprite.texture;
            if (tex == null) { _frameColorCache[key] = Color.white; return Color.white; }

            Texture2D readable = tex.isReadable ? tex : MakeReadable(tex);
            if (readable == null) { _frameColorCache[key] = Color.white; return Color.white; }

            Rect rect = sprite.textureRect;
            Color result = SampleRegionAverage(readable,
                Mathf.FloorToInt(rect.x), Mathf.FloorToInt(rect.y),
                Mathf.FloorToInt(rect.width), Mathf.FloorToInt(rect.height));

            if (readable != tex)
            {
                if (Application.isPlaying) Object.Destroy(readable);
                else Object.DestroyImmediate(readable);
            }

            _frameColorCache[key] = result;
            return result;
        }

        private void BuildFrameColorCache()
        {
            if (_frameColorCacheBuilt) return;
            _frameColorCacheBuilt = true;

            var tss = _system.TextureSheetSettings;
            if (tss?.textureSheet == null) return;

            int tilesX = tss.tilesX, tilesY = tss.tilesY;
            if (tilesX <= 0 || tilesY <= 0) return;

            Texture2D readable = MakeReadable(tss.textureSheet);
            if (readable == null) return;

            int total = tilesX * tilesY;
            float tileW = tss.textureSheet.width / (float)tilesX;
            float tileH = tss.textureSheet.height / (float)tilesY;

            for (int f = 0; f < total; f++)
            {
                int col = f % tilesX;

                int row = f / tilesX;

                _frameColorCache[f] = SampleRegionAverage(readable,
                    Mathf.FloorToInt(col * tileW),
                    Mathf.FloorToInt(row * tileH),
                    Mathf.Max(1, Mathf.FloorToInt(tileW)),
                    Mathf.Max(1, Mathf.FloorToInt(tileH)));
            }

            if (readable != tss.textureSheet)
            {
                if (Application.isPlaying) Object.Destroy(readable);
                else Object.DestroyImmediate(readable);
            }
        }

        private Texture2D MakeReadable(Texture2D source)
        {
            if (source.isReadable) return source;

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

        private Color SampleRegionAverage(Texture2D tex, int rx, int ry, int rw, int rh)
        {
            float r = 0, g = 0, b = 0;
            int count = 0;
            int stepX = Mathf.Max(1, rw / 5);
            int stepY = Mathf.Max(1, rh / 5);

            for (int x = rx + stepX / 2; x < rx + rw; x += stepX)
                for (int y = ry + stepY / 2; y < ry + rh; y += stepY)
                {
                    if (x < 0 || x >= tex.width || y < 0 || y >= tex.height) continue;
                    Color px = tex.GetPixel(x, y);
                    if (px.a > 0.2f) { r += px.r; g += px.g; b += px.b; count++; }
                }

            return count > 0 ? new Color(r / count, g / count, b / count, 1f) : Color.white;
        }

        #endregion

        #region Public API

        public void Cleanup()
        {
            DestroyRenderer();
            _frameColorCache.Clear();
            _frameColorCacheBuilt = false;
            Dispose();
        }

        public void ClearTrailVisuals()
        {
            _vertCount = 0;
            _triCount = 0;
            if (HasRenderer)
                _trailRenderer.Clear();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_vertices.IsCreated) _vertices.Dispose();
            if (_colors.IsCreated) _colors.Dispose();
            if (_uvs.IsCreated) _uvs.Dispose();
            if (_triangles.IsCreated) _triangles.Dispose();
        }

        #endregion

        #region Yardımcılar

        private Vector2 GetParticlePosition(Particle particle)
        {
            if (_system.EmissionSettings.simulationSpace == UIParticleSystem.Enums.SimulationSpace.Local)
                return particle.position;

            Vector3 local = _trailRenderer != null
                ? _trailRenderer.transform.InverseTransformPoint(particle.position)
                : (Vector3)(Vector2)particle.position;

            return new Vector2(local.x, local.y);
        }

        private static void SafeDestroy(Object obj)
        {
            if (obj == null) return;
            if (Application.isPlaying) { Object.Destroy(obj); return; }

            try { Object.DestroyImmediate(obj); }
            catch (System.InvalidOperationException)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (obj != null) Object.DestroyImmediate(obj);
                };
#endif
            }
        }

        #endregion
    }
}