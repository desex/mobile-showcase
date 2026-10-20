using TierStudios.UIParticleSystem.Settings;
using TierStudios.UIParticleSystem.Core;
using TierStudios.UIParticleSystem.Managers;
using TierStudios.UIParticleSystem.Utilities;
using TierStudios.UIParticleSystem.Enums;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace TierStudios.UIParticleSystem.Core
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("Effects/UI Particle System")]
    [System.Serializable]
    public class UIParticleSystem : MaskableGraphic
    {

        #region Serialized Fields

        [SerializeField] private Sprite particleSprite;
        [SerializeField] private BlendMode blendMode = BlendMode.AlphaBlend;

        [SerializeField] public bool useGravity = false;
        [SerializeField] public Vector2 gravity = new Vector2(0f, -10f);
        [SerializeField] private bool useRandomSpeed = false;
        [SerializeField] private float speed = 100f;
        [SerializeField] private float minSpeed = 50f;
        [SerializeField] private float maxSpeed = 100f;
        [SerializeField] private bool useMasked = false;
        [SerializeField] private float simulationSpeed = 1f;
        [SerializeField] private bool useUnscaledTime = false;

        [SerializeField] private EmissionSettings emissionSettings = new EmissionSettings();
        [SerializeField] private LifetimeSettings lifetimeSettings = new LifetimeSettings();
        [SerializeField] private SizeSettings sizeSettings = new SizeSettings();
        [SerializeField] private ColorSettings colorSettings = new ColorSettings();
        [SerializeField] private RotationSettings rotationSettings = new RotationSettings();
        [SerializeField] private AttractorSettings attractorSettings = new AttractorSettings();
        [SerializeField] private NoiseSettings noiseSettings = new NoiseSettings();
        [SerializeField] private TrailSettings trailSettings = new TrailSettings();
        [SerializeField] private TextureSheetSettings textureSheetSettings = new TextureSheetSettings();
        [SerializeField] private ParticleEventSettings eventSettings = new ParticleEventSettings();
        [SerializeField] private VelocityOverLifetimeSettings velocityOverLifetimeSettings = new VelocityOverLifetimeSettings();
        [SerializeField] private LimitVelocityOverLifetimeSettings limitVelocityOverLifetimeSettings = new LimitVelocityOverLifetimeSettings();
        [SerializeField] private InheritVelocitySettings inheritVelocitySettings = new InheritVelocitySettings();
        [SerializeField] private ForceOverLifetimeSettings forceOverLifetimeSettings = new ForceOverLifetimeSettings();
        [SerializeField] private CollisionSettings collisionSettings = new CollisionSettings();
        [SerializeField] private ColorBySpeedSettings colorBySpeedSettings = new ColorBySpeedSettings();
        [SerializeField] private SizeBySpeedSettings sizeBySpeedSettings = new SizeBySpeedSettings();
        [SerializeField] private SubEmitterSettings subEmitterSettings = new SubEmitterSettings();
        [SerializeField] private SoundEffectSettings soundEffectSettings = new SoundEffectSettings();
        [SerializeField] private RendererSettings rendererSettings = new RendererSettings();

        #endregion


        #region Mobile Optimization Settings

        [Header("Mobile Optimization")]
        [SerializeField] private bool mobileOptimizationMode = false;
        [SerializeField] private int mobileMaxParticles = 50;
        [SerializeField] private float mobileUpdateInterval = 0.033f;
        [SerializeField] private bool disableTrailsOnMobile = true;
        [SerializeField] private bool disableNoiseOnMobile = true;

        #endregion

        #region Managers

        private EmissionManager emissionManager;
        private ParticleUpdater particleUpdater;
        private AttractorManager attractorManager;
        private NoiseManager noiseManager;
        private TextureSheetManager textureSheetManager;
        private VelocityOverLifetimeManager velocityOverLifetimeManager;
        private PerformanceMonitor performanceMonitor;
        private SubEmitterManager subEmitterManager;
        private SoundEffectManager soundEffectManager;

        #endregion

        #region Cached References

        private RectTransform cachedRectTransform;
        private Transform cachedTransform;
        private Canvas cachedCanvas;
        private Camera cachedCamera;
        private Material cachedAdditiveMaterial;

        #endregion

        private NativeArray<Vector3> _meshVertices;
        private NativeArray<Color32> _meshColors;
        private NativeArray<Vector2> _meshUVs;
        private NativeArray<int> _meshTriangles;
        private int _meshBufferCapacity;

        private bool _initialized;
        private int _settingsRevision;

        #region Mesh Render Altyap─▒s─▒

        private Mesh _particleMesh;
        private Mesh _blankMesh;
        private UIParticleTrailRenderer _particleFrontRenderer;
        private bool _bringToFrontScheduled;
        private bool _isTogglingActive;

        private Mesh ParticleMesh
        {
            get
            {
                if (_particleMesh == null)
                {
                    _particleMesh = new Mesh();
                    _particleMesh.name = "UIParticleSystem_Mesh";
                    _particleMesh.MarkDynamic();
                }
                return _particleMesh;
            }
        }

        private Mesh BlankMesh
        {
            get
            {
                if (_blankMesh == null)
                {
                    _blankMesh = new Mesh();
                    _blankMesh.name = "UIParticleSystem_Blank";
                }
                return _blankMesh;
            }
        }

        private bool TrailsDrawBehindParticles =>
            trailSettings != null && trailSettings.useTrails && CanUseTrails;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }

        protected override void OnCanvasGroupChanged()
        {
            base.OnCanvasGroupChanged();
            CanvasGroupAlphaUtility.ApplyToRenderer(this);
            if (_particleFrontRenderer != null)
                CanvasGroupAlphaUtility.ApplyToRenderer(_particleFrontRenderer);
        }

        protected override void UpdateMaterial()
        {
            base.UpdateMaterial();
            CanvasGroupAlphaUtility.ApplyToRenderer(this);
            if (_particleFrontRenderer != null)
                CanvasGroupAlphaUtility.ApplyToRenderer(_particleFrontRenderer);
        }

        public override Texture mainTexture
        {
            get
            {
                if (textureSheetSettings != null
                    && textureSheetSettings.useTextureSheetAnimation
                    && textureSheetSettings.textureSheet != null)
                    return textureSheetSettings.textureSheet;

                if (particleSprite != null) return particleSprite.texture;

                return base.mainTexture;
            }
        }

        private void EnsureMeshBufferCapacity(int requiredParticles)
        {
            int required = requiredParticles * 4;

            bool buffersValid = _meshVertices.IsCreated
                             && _meshColors.IsCreated
                             && _meshUVs.IsCreated
                             && _meshTriangles.IsCreated;

            if (buffersValid && _meshBufferCapacity >= required) return;

            int newCap = Mathf.Max(_meshBufferCapacity * 2, required + 64);

            if (_meshVertices.IsCreated) _meshVertices.Dispose();
            if (_meshColors.IsCreated) _meshColors.Dispose();
            if (_meshUVs.IsCreated) _meshUVs.Dispose();
            if (_meshTriangles.IsCreated) _meshTriangles.Dispose();

            _meshVertices = new NativeArray<Vector3>(newCap, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            _meshColors = new NativeArray<Color32>(newCap, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            _meshUVs = new NativeArray<Vector2>(newCap, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            _meshTriangles = new NativeArray<int>(newCap / 4 * 6, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

            _meshBufferCapacity = newCap;
        }

        private void RebuildParticleMesh()
        {
            int count = activeParticles.Count;
            Mesh mesh = ParticleMesh;

            if (count == 0)
            {
                mesh.Clear();
                SubmitParticleMesh(mesh);
                return;
            }

            EnsureMeshBufferCapacity(count);

            Vector4 outerUV;
            if (particleSprite != null)
            {
                Vector4 trimmedUV = UnityEngine.Sprites.DataUtility.GetOuterUV(particleSprite);
                Vector4 padding = UnityEngine.Sprites.DataUtility.GetPadding(particleSprite);
                
                if (padding != Vector4.zero)
                {
                    Vector2 originalSize = particleSprite.rect.size;
                    float trimmedWidth = originalSize.x - (padding.x + padding.z);
                    float trimmedHeight = originalSize.y - (padding.y + padding.w);

                    float uMin = trimmedUV.x;
                    float vMin = trimmedUV.y;
                    float uMax = trimmedUV.z;
                    float vMax = trimmedUV.w;

                    float du = uMax - uMin;
                    float dv = vMax - vMin;

                    if (trimmedWidth > 0f)
                    {
                        uMin = uMin - (padding.x / trimmedWidth) * du;
                        uMax = uMax + (padding.z / trimmedWidth) * du;
                    }
                    if (trimmedHeight > 0f)
                    {
                        vMin = vMin - (padding.y / trimmedHeight) * dv;
                        vMax = vMax + (padding.w / trimmedHeight) * dv;
                    }

                    outerUV = new Vector4(uMin, vMin, uMax, vMax);
                }
                else
                {
                    outerUV = trimmedUV;
                }
            }
            else
            {
                outerUV = new Vector4(0f, 0f, 1f, 1f);
            }

            bool useSheet = textureSheetSettings != null && textureSheetSettings.useTextureSheetAnimation;
            bool isWorldSpace = emissionSettings != null
                             && emissionSettings.simulationSpace == Enums.SimulationSpace.World;

            Matrix4x4 worldToLocal = isWorldSpace
                ? cachedTransform.worldToLocalMatrix
                : Matrix4x4.identity;

            int vert = 0;
            int tri = 0;

            for (int i = 0; i < count; i++)
            {
                Particle p = activeParticles[i];
                if (!p.isAlive) continue;

                if (isWorldSpace)
                {
                    _meshVertices[vert] = worldToLocal.MultiplyPoint3x4(p.points[0]);
                    _meshVertices[vert + 1] = worldToLocal.MultiplyPoint3x4(p.points[1]);
                    _meshVertices[vert + 2] = worldToLocal.MultiplyPoint3x4(p.points[2]);
                    _meshVertices[vert + 3] = worldToLocal.MultiplyPoint3x4(p.points[3]);
                }
                else
                {
                    _meshVertices[vert] = p.points[0];
                    _meshVertices[vert + 1] = p.points[1];
                    _meshVertices[vert + 2] = p.points[2];
                    _meshVertices[vert + 3] = p.points[3];
                }

                Color32 c = p.color;
                _meshColors[vert] = _meshColors[vert + 1] = _meshColors[vert + 2] = _meshColors[vert + 3] = c;

                Vector2 uvMin, uvMax;
                if (useSheet)
                {
                    uvMin = GetFrameUVMin(p.currentFrame);
                    uvMax = GetFrameUVMax(p.currentFrame);
                }
                else
                {
                    uvMin = new Vector2(outerUV.x, outerUV.y);
                    uvMax = new Vector2(outerUV.z, outerUV.w);
                }

                _meshUVs[vert] = new Vector2(uvMin.x, uvMax.y);
                _meshUVs[vert + 1] = new Vector2(uvMax.x, uvMax.y);
                _meshUVs[vert + 2] = new Vector2(uvMax.x, uvMin.y);
                _meshUVs[vert + 3] = new Vector2(uvMin.x, uvMin.y);

                _meshTriangles[tri] = vert;
                _meshTriangles[tri + 1] = vert + 1;
                _meshTriangles[tri + 2] = vert + 2;
                _meshTriangles[tri + 3] = vert;
                _meshTriangles[tri + 4] = vert + 2;
                _meshTriangles[tri + 5] = vert + 3;

                vert += 4;
                tri += 6;
            }

            mesh.Clear();
            if (vert > 0)
            {
                mesh.SetVertices(_meshVertices, 0, vert);
                mesh.SetColors(_meshColors, 0, vert);
                mesh.SetUVs(0, _meshUVs, 0, vert);
                mesh.SetIndices(_meshTriangles, 0, tri, MeshTopology.Triangles, 0, false);
            }
            mesh.RecalculateBounds();
            SubmitParticleMesh(mesh);
        }

        private void SubmitParticleMesh(Mesh mesh)
        {
            if (!CanModifyChildHierarchy)
            {
                if (canvasRenderer != null)
                    canvasRenderer.SetMesh(mesh);
                return;
            }

            if (TrailsDrawBehindParticles)
            {
                EnsureParticleFrontRenderer();
                if (_particleFrontRenderer == null)
                {
                    if (canvasRenderer != null)
                        canvasRenderer.SetMesh(mesh);
                    return;
                }

                if (canvasRenderer != null)
                    canvasRenderer.SetMesh(BlankMesh);

                _particleFrontRenderer.color = color;
                _particleFrontRenderer.material = material;
                _particleFrontRenderer.maskable = useMasked;
                _particleFrontRenderer.textureOverride = mainTexture;
                _particleFrontRenderer.ApplyMesh(mesh);
                if (_particleFrontRenderer.canvasRenderer != null)
                    _particleFrontRenderer.canvasRenderer.SetMaterial(
                        _particleFrontRenderer.materialForRendering, mainTexture);
                BringParticleLayerToFront();
            }
            else
            {
                DestroyParticleFrontRenderer();
                if (canvasRenderer != null)
                    canvasRenderer.SetMesh(mesh);
                CanvasGroupAlphaUtility.ApplyToRenderer(this);
            }

            SetMaterialDirty();
        }

        internal bool CanModifyChildHierarchy
        {
            get
            {
                if (this == null)
                    return false;
                if (_isTogglingActive)
                    return false;
                if (!isActiveAndEnabled)
                    return false;
                if (!gameObject.activeInHierarchy)
                    return false;
                Transform parent = transform.parent;
                return parent == null || parent.gameObject.activeInHierarchy;
            }
        }

        internal void BringParticleLayerToFront()
        {
            if (_particleFrontRenderer == null || _particleFrontRenderer.transform == null)
                return;

            SyncLayerRect(_particleFrontRenderer.rectTransform);

            if (!CanModifyChildHierarchy)
            {
                if (isActiveAndEnabled)
                    ScheduleBringParticleLayerToFront();
                return;
            }

            ApplyParticleLayerSiblingOrder();
        }

        private void ScheduleBringParticleLayerToFront()
        {
            if (_bringToFrontScheduled)
                return;

            _bringToFrontScheduled = true;
#if UNITY_EDITOR
            EditorApplication.delayCall += DeferredBringParticleLayerToFront;
#else
            if (isActiveAndEnabled)
                StartCoroutine(DeferredBringParticleLayerToFrontRoutine());
            else
                _bringToFrontScheduled = false;
#endif
        }

        private System.Collections.IEnumerator DeferredBringParticleLayerToFrontRoutine()
        {
            yield return null;
            DeferredBringParticleLayerToFront();
        }

        private void DeferredBringParticleLayerToFront()
        {
            _bringToFrontScheduled = false;
            if (this == null)
                return;
            if (!CanModifyChildHierarchy)
                return;
            if (_particleFrontRenderer == null || _particleFrontRenderer.transform == null)
                return;

            SyncLayerRect(_particleFrontRenderer.rectTransform);
            ApplyParticleLayerSiblingOrder();
        }

        private void CancelScheduledBringParticleLayerToFront()
        {
            if (!_bringToFrontScheduled)
                return;

            _bringToFrontScheduled = false;
#if UNITY_EDITOR
            EditorApplication.delayCall -= DeferredBringParticleLayerToFront;
#endif
        }

        private void ApplyParticleLayerSiblingOrder()
        {
            Transform layer = _particleFrontRenderer != null ? _particleFrontRenderer.transform : null;
            if (layer == null || layer.parent != transform)
                return;
            if (layer.GetSiblingIndex() == transform.childCount - 1)
                return;

            layer.SetAsLastSibling();
        }

        private void EnsureParticleFrontRenderer()
        {
            if (_particleFrontRenderer != null && _particleFrontRenderer.gameObject != null)
            {
                _particleFrontRenderer.raycastTarget = false;
                _particleFrontRenderer.maskable = useMasked;
                BringParticleLayerToFront();
                return;
            }

            var existing = GetComponentsInChildren<UIParticleTrailRenderer>(true);
            for (int i = 0; i < existing.Length; i++)
            {
                var renderer = existing[i];
                if (renderer == null || renderer.gameObject.name != "UIParticleQuads")
                    continue;

                _particleFrontRenderer = renderer;
                _particleFrontRenderer.raycastTarget = false;
                _particleFrontRenderer.maskable = useMasked;
                BringParticleLayerToFront();
                return;
            }

            if (!CanModifyChildHierarchy)
                return;

            var go = new GameObject("UIParticleQuads");
            go.hideFlags = HideFlags.HideAndDontSave;

            var rt = go.AddComponent<RectTransform>();
            rt.SetParent(transform, false);

            var ignoreLayout = go.AddComponent<LayoutElement>();
            ignoreLayout.ignoreLayout = true;

            _particleFrontRenderer = go.AddComponent<UIParticleTrailRenderer>();
            _particleFrontRenderer.raycastTarget = false;
            _particleFrontRenderer.maskable = useMasked;
            _particleFrontRenderer.color = color;
            _particleFrontRenderer.material = material;
            _particleFrontRenderer.textureOverride = mainTexture;
            BringParticleLayerToFront();
        }

        private void SyncLayerRect(RectTransform layer)
        {
            if (layer == null || rectTransform == null) return;

            layer.anchorMin = Vector2.zero;
            layer.anchorMax = Vector2.one;
            layer.pivot = rectTransform.pivot;
            layer.anchoredPosition = Vector2.zero;
            layer.sizeDelta = Vector2.zero;
            layer.offsetMin = Vector2.zero;
            layer.offsetMax = Vector2.zero;
            layer.localRotation = Quaternion.identity;
            layer.localScale = Vector3.one;
        }

        private void DestroyParticleFrontRenderer()
        {
            if (_particleFrontRenderer != null)
            {
                if (_particleFrontRenderer.gameObject != null)
                {
                    if (Application.isPlaying) Destroy(_particleFrontRenderer.gameObject);
                    else DestroyImmediate(_particleFrontRenderer.gameObject);
                }
                _particleFrontRenderer = null;
            }

            if (transform == null) return;
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (child != null && child.name == "UIParticleQuads")
                {
                    if (Application.isPlaying) Destroy(child.gameObject);
                    else DestroyImmediate(child.gameObject);
                }
            }
        }

        private Vector2 GetFrameUVMin(int frame)
        {
            int cols = Mathf.Max(1, textureSheetSettings.tilesX);
            int rows = Mathf.Max(1, textureSheetSettings.tilesY);
            return new Vector2((frame % cols) / (float)cols, (frame / cols) / (float)rows);
        }

        private Vector2 GetFrameUVMax(int frame)
        {
            int cols = Mathf.Max(1, textureSheetSettings.tilesX);
            int rows = Mathf.Max(1, textureSheetSettings.tilesY);
            return new Vector2((frame % cols + 1) / (float)cols, (frame / cols + 1) / (float)rows);
        }

        #endregion

        #region State

        private List<Particle> activeParticles = new List<Particle>(256);
        private bool isEmitting = false;
        private bool isPaused = false;
        private bool wasEverStarted = false;
        private UnityEngine.Coroutine delayedInitCoroutine = null;

        #endregion

        #region Timing

        private float lastUpdateTime = 0f;
        private const float MIN_UPDATE_INTERVAL = 0.020f;
        private int performanceCheckFrame = 0;

        #endregion

        #region Object Pooling

        private ParticleEventArgs cachedEventArgs;
        private ParticleEventData cachedEventData;
        private ParticlePool particlePool;

        public ParticlePool Pool => particlePool;

        #endregion

        #region Events

        public System.Action<ParticleEventArgs> OnParticleSpawned;
        public System.Action<ParticleEventArgs> OnParticleDied;
        public System.Action<ParticleEventArgs> OnParticleCollision;
        public System.Action<UIParticleSystem> OnEmissionStarted;
        public System.Action<UIParticleSystem> OnEmissionStopped;
        public System.Action<UIParticleSystem> OnSystemPaused;
        public System.Action<UIParticleSystem> OnSystemResumed;
        public System.Action<UIParticleSystem> OnParticlesCleared;

        #endregion

        #region Properties

        public EmissionSettings EmissionSettings => emissionSettings;
        public LifetimeSettings LifetimeSettings => lifetimeSettings;
        public SizeSettings SizeSettings => sizeSettings;
        public ColorSettings ColorSettings => colorSettings;
        public RotationSettings RotationSettings => rotationSettings;
        public AttractorSettings AttractorSettings => attractorSettings;
        public NoiseSettings NoiseSettings => noiseSettings;
        public TrailSettings TrailSettings => trailSettings;
        public TextureSheetSettings TextureSheetSettings => textureSheetSettings;
        public ParticleEventSettings EventSettings => eventSettings;
        public VelocityOverLifetimeSettings VelocityOverLifetimeSettings => velocityOverLifetimeSettings;
        public LimitVelocityOverLifetimeSettings LimitVelocityOverLifetimeSettings => limitVelocityOverLifetimeSettings;
        public InheritVelocitySettings InheritVelocitySettings => inheritVelocitySettings;
        public ForceOverLifetimeSettings ForceOverLifetimeSettings => forceOverLifetimeSettings;
        public CollisionSettings CollisionSettings => collisionSettings;
        public ColorBySpeedSettings ColorBySpeedSettings => colorBySpeedSettings;
        public SizeBySpeedSettings SizeBySpeedSettings => sizeBySpeedSettings;
        public SubEmitterSettings SubEmitterSettings => subEmitterSettings;
        public SoundEffectSettings SoundEffectSettings => soundEffectSettings;
        public RendererSettings RendererSettings => rendererSettings;

        public List<Particle> ActiveParticles => activeParticles;
        public bool IsEmitting { get => isEmitting; set => isEmitting = value; }
        public bool IsPaused { get => isPaused; set => isPaused = value; }
        public bool WasEverStarted { get => wasEverStarted; set => wasEverStarted = value; }
        internal int SettingsRevision => _settingsRevision;

        public Sprite ParticleSprite => particleSprite;

        public Material ParticleMaterial
        {
            get
            {
                if (blendMode != BlendMode.Additive) return null;

                if (cachedAdditiveMaterial != null) return cachedAdditiveMaterial;

                cachedAdditiveMaterial = Resources.Load<Material>("Additive_UIParticleSystem");

#if UNITY_EDITOR
                if (cachedAdditiveMaterial == null)
                {
                    string[] guids = AssetDatabase.FindAssets("Additive_UIParticleSystem t:Material");
                    if (guids.Length > 0)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                        cachedAdditiveMaterial = AssetDatabase.LoadAssetAtPath<Material>(assetPath);               
                    }
                    else
                        Debug.LogWarning("[UIParticleSystem] Additive_UIParticleSystem.mat not found in project!");
                }
#endif
                return cachedAdditiveMaterial;
            }
        }

        public BlendMode CurrentBlendMode
        {
            get => blendMode;
            set
            {
                if (blendMode == value) return;
                blendMode = value;
                cachedAdditiveMaterial = null;
                ApplyBlendMaterial();
            }
        }
        private void ApplyBlendMaterial()
        {
            material = blendMode == BlendMode.Additive ? ParticleMaterial : null;

            if (canvasRenderer != null)
                canvasRenderer.SetMaterial(materialForRendering, mainTexture);

            if (_particleFrontRenderer != null)
            {
                _particleFrontRenderer.material = material;
                if (_particleFrontRenderer.canvasRenderer != null)
                    _particleFrontRenderer.canvasRenderer.SetMaterial(
                        _particleFrontRenderer.materialForRendering, mainTexture);
            }
        }

        public bool UseRandomSpeed => useRandomSpeed;
        public bool UseMasked => useMasked;
        public float SimulationSpeed
        {
            get => Mathf.Max(0f, simulationSpeed);
            set => simulationSpeed = value;
        }
        public bool UseUnscaledTime
        {
            get => useUnscaledTime;
            set => useUnscaledTime = value;
        }

        public bool MobileOptimizationMode
        {
            get => mobileOptimizationMode;
            set => mobileOptimizationMode = value;
        }

        public int MobileMaxParticles
        {
            get => mobileMaxParticles;
            set => mobileMaxParticles = Mathf.Max(1, value);
        }

        public Transform Transform
        {
            get
            {
                if (cachedTransform == null)
                    cachedTransform = this == null ? null : transform;
                return cachedTransform;
            }
        }
        public Canvas CachedCanvas => cachedCanvas;
        public Camera CachedCamera => cachedCamera;

        public float Speed => Mathf.Max(speed, 0.01f);
        public float MinSpeed => Mathf.Max(minSpeed, 0.01f);
        public float MaxSpeed => Mathf.Max(maxSpeed, 0.01f);

        public int EffectiveMaxParticles => mobileOptimizationMode
            ? Mathf.Min(emissionSettings.maxParticles, mobileMaxParticles)
            : emissionSettings.maxParticles;

        public bool CanUseTrails => !mobileOptimizationMode || !disableTrailsOnMobile;
        public bool CanUseNoise => !mobileOptimizationMode || !disableNoiseOnMobile;

        public SubEmitterManager SubEmitterManagerInstance => subEmitterManager;
        public SoundEffectManager SoundEffectManagerInstance => soundEffectManager;

        #endregion

        #region Event Triggers

        public void TriggerParticleSpawned(Particle particle, int particleIndex = -1)
        {
            bool needsEvent = eventSettings.EnableEvents;
            bool needsSub = subEmitterSettings.EnableSubEmitters;
            bool needsSound = soundEffectSettings.EnableSoundEffects;

            if (!needsEvent && !needsSub && !needsSound) return;

            if (needsEvent)
            {
                if (OnParticleSpawned != null)
                {
                    if (cachedEventArgs == null)
                        cachedEventArgs = new ParticleEventArgs(particle, this);
                    else
                        cachedEventArgs.UpdateData(particle, this);

                    OnParticleSpawned.Invoke(cachedEventArgs);
                }

                if (eventSettings.onParticleSpawned != null)
                {
                    if (cachedEventData == null)
                        cachedEventData = new ParticleEventData(particle, particleIndex);
                    else
                        cachedEventData.UpdateData(particle, particleIndex);

                    eventSettings.onParticleSpawned.Invoke(cachedEventData);
                }
            }

            if (needsSub && subEmitterManager != null)
                subEmitterManager.OnParticleBirth(particle);

            if (needsSound && soundEffectManager != null)
                soundEffectManager.OnParticleBirth(particle);
        }

        public void TriggerParticleDied(Particle particle, int particleIndex = -1)
        {
            bool needsEvent = eventSettings.EnableEvents;
            bool needsSub = subEmitterSettings.EnableSubEmitters;
            bool needsSound = soundEffectSettings.EnableSoundEffects;

            if (!needsEvent && !needsSub && !needsSound) return;

            if (needsEvent)
            {
                if (OnParticleDied != null)
                {
                    if (cachedEventArgs == null)
                        cachedEventArgs = new ParticleEventArgs(particle, this);
                    else
                        cachedEventArgs.UpdateData(particle, this);

                    OnParticleDied.Invoke(cachedEventArgs);
                }

                if (eventSettings.onParticleDied != null)
                {
                    if (cachedEventData == null)
                        cachedEventData = new ParticleEventData(particle, particleIndex);
                    else
                        cachedEventData.UpdateData(particle, particleIndex);

                    eventSettings.onParticleDied.Invoke(cachedEventData);
                }
            }

            if (needsSub && subEmitterManager != null)
                subEmitterManager.OnParticleDeath(particle);

            if (needsSound && soundEffectManager != null && gameObject != null && gameObject.activeInHierarchy)
                soundEffectManager.OnParticleDeath(particle);
        }

        public void TriggerParticleCollision(Particle particle, int particleIndex = -1)
        {
            bool needsEvent = eventSettings.EnableEvents;
            bool needsSub = subEmitterSettings.EnableSubEmitters;
            bool needsSound = soundEffectSettings.EnableSoundEffects;

            if (!needsEvent && !needsSub && !needsSound) return;

            if (needsEvent)
            {
                if (OnParticleCollision != null)
                {
                    if (cachedEventArgs == null)
                        cachedEventArgs = new ParticleEventArgs(particle, this);
                    else
                        cachedEventArgs.UpdateData(particle, this);

                    OnParticleCollision.Invoke(cachedEventArgs);
                }

                if (eventSettings.onParticleCollision != null)
                {
                    if (cachedEventData == null)
                        cachedEventData = new ParticleEventData(particle, particleIndex);
                    else
                        cachedEventData.UpdateData(particle, particleIndex);

                    eventSettings.onParticleCollision.Invoke(cachedEventData);
                }
            }

            if (needsSub && subEmitterManager != null)
                subEmitterManager.OnParticleCollision(particle);

            if (needsSound && soundEffectManager != null && gameObject != null && gameObject.activeInHierarchy)
                soundEffectManager.OnParticleCollision(particle);
        }

        public void TriggerEmissionStarted()
        {
            if (!eventSettings.EnableEvents && !soundEffectSettings.EnableSoundEffects) return;

            if (eventSettings.EnableEvents)
            {
                OnEmissionStarted?.Invoke(this);
                eventSettings.onEmissionStarted?.Invoke(this);
            }

            soundEffectManager?.OnSystemStart();
        }

        public void TriggerEmissionStopped()
        {
            if (!eventSettings.EnableEvents && !soundEffectSettings.EnableSoundEffects) return;

            if (eventSettings.EnableEvents)
            {
                OnEmissionStopped?.Invoke(this);
                eventSettings.onEmissionStopped?.Invoke(this);
            }

            soundEffectManager?.OnSystemStop();
        }

        public void TriggerSystemPaused()
        {
            if (!eventSettings.EnableEvents) return;
            OnSystemPaused?.Invoke(this);
            eventSettings.onSystemPaused?.Invoke(this);
        }

        public void TriggerSystemResumed()
        {
            if (!eventSettings.EnableEvents) return;
            OnSystemResumed?.Invoke(this);
            eventSettings.onSystemResumed?.Invoke(this);
        }

        public void TriggerParticlesCleared()
        {
            if (!eventSettings.EnableEvents) return;
            OnParticlesCleared?.Invoke(this);
            eventSettings.onParticlesCleared?.Invoke(this);
        }

        #endregion

        #region Initialization

        private void EnsureInitialized()
        {
            if (this == null)
                return;

            if (emissionManager == null || particleUpdater == null ||
                attractorManager == null || noiseManager == null ||
                textureSheetManager == null || performanceMonitor == null ||
                subEmitterManager == null)
            {
                InitializeManagers();
            }

            if (activeParticles == null)
                activeParticles = new List<Particle>(256);

            InitializeDefaultSettings();
            _initialized = true;
        }

        public void EnsureInitializedPublic() => EnsureInitialized();

        protected override void Awake()
        {
            base.Awake();
            CacheReferences();
            EnsureInitialized();
            ApplyBlendMaterial();
            maskable = useMasked;

#if UNITY_ANDROID || UNITY_IOS
            if (!Application.isEditor)
                mobileOptimizationMode = true;
#endif
        }

        private void CacheReferences()
        {
            cachedTransform = transform;
            cachedRectTransform = GetComponent<RectTransform>();
            cachedCanvas = GetComponentInParent<Canvas>();

            if (cachedCanvas != null && cachedCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                cachedCamera = cachedCanvas.worldCamera ?? Camera.main;
        }

        protected override void OnEnable()
        {
            _isTogglingActive = true;
            try
            {
                base.OnEnable();
                EnsureInitialized();

                maskable = useMasked;
                RecalculateMasking();

                if (Application.isPlaying)
                {
                    delayedInitCoroutine = StartCoroutine(DelayedInitialization());
                }
                else
                {
                    ClearAllParticles(this);
                    emissionManager?.StopEmitting();
                    isPaused = false;
                }

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    EditorApplication.update += OnEditorUpdate;
                    Selection.selectionChanged += OnSelectionChanged;
                    CheckSelection();
                }
#endif
            }
            finally
            {
                _isTogglingActive = false;
            }

            ScheduleBringParticleLayerToFront();
        }

        private System.Collections.IEnumerator DelayedInitialization()
        {
            yield return null;

            if (delayedInitCoroutine == null) yield break;

            if (wasEverStarted)
            {
                delayedInitCoroutine = null;
                yield break;
            }

            ClearAllParticles(this);
            emissionManager?.StopEmitting();
            isPaused = false;

            if (!emissionSettings.onlyInEditor && emissionSettings.emitOnStart)
                StartEmitting();

            delayedInitCoroutine = null;
        }

        private void CancelDelayedInitialization()
        {
            if (delayedInitCoroutine == null) return;
            StopCoroutine(delayedInitCoroutine);
            delayedInitCoroutine = null;
        }

        private void InitializeManagers()
        {
            if (this == null || transform == null)
                return;

            emissionManager = new EmissionManager(this);
            particleUpdater = new ParticleUpdater(this);
            attractorManager = new AttractorManager(this);
            noiseManager = new NoiseManager(this);
            textureSheetManager = new TextureSheetManager(this);
            performanceMonitor = new PerformanceMonitor(this);
            velocityOverLifetimeManager = new VelocityOverLifetimeManager(this);
            subEmitterManager = new SubEmitterManager(this);
            soundEffectManager = new SoundEffectManager(this);

            int poolCapacity = Mathf.Max(mobileOptimizationMode ? mobileMaxParticles : emissionSettings.maxParticles, 50);

            if (particlePool == null)
                particlePool = new ParticlePool(poolCapacity);
        }

        private void InitializeDefaultSettings()
        {
            if (particleSprite == null)
                LoadDefaultSprite();

            attractorSettings ??= new AttractorSettings();
            colorSettings ??= new ColorSettings();
            emissionSettings ??= new EmissionSettings();
            eventSettings ??= new ParticleEventSettings();
            subEmitterSettings ??= new SubEmitterSettings();
            velocityOverLifetimeSettings ??= new VelocityOverLifetimeSettings();
            limitVelocityOverLifetimeSettings ??= new LimitVelocityOverLifetimeSettings();
            forceOverLifetimeSettings ??= new ForceOverLifetimeSettings();
            collisionSettings ??= new CollisionSettings();
            soundEffectSettings ??= new SoundEffectSettings();

            if (attractorSettings.attractorFalloff == null || attractorSettings.attractorFalloff.keys.Length == 0)
                attractorSettings.attractorFalloff = AnimationCurve.Linear(0f, 1f, 1f, 0f);

            if (colorSettings.colorGradient == null || colorSettings.colorGradient.colorKeys.Length == 0)
            {
                colorSettings.colorGradient = new Gradient();
                colorSettings.colorGradient.SetKeys(
                    new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                    new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
            }

            if (colorSettings.colorGradient2 == null || colorSettings.colorGradient2.colorKeys.Length == 0)
            {
                colorSettings.colorGradient2 = new Gradient();
                colorSettings.colorGradient2.SetKeys(
                    new[] { new GradientColorKey(Color.red, 0f), new GradientColorKey(Color.yellow, 1f) },
                    new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
            }

            if (emissionSettings.bursts == null)
                emissionSettings.bursts = new List<Burst>();
        }

        public void LoadDefaultSprite()
        {
#if UNITY_EDITOR
            Sprite defaultSprite = Resources.Load<Sprite>("DefaultParticleSprite");
            if (defaultSprite == null)
            {
                string[] spriteGuids = AssetDatabase.FindAssets("DefaultParticleSprite t:Sprite");
                if (spriteGuids.Length > 0)
                    defaultSprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(spriteGuids[0]));
            }

            if (defaultSprite == null)
            {
                defaultSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
                if (defaultSprite == null)
                {
                    const string spritePath = "Assets/TierStudios/UIParticleSystem/InternalAssets/DefaultParticleSprite.png";
                    Texture2D circleTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(spritePath);
                    if (circleTexture == null)
                    {
                        CreateDefaultParticleTexture();
                        circleTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(spritePath);
                    }

                    if (circleTexture != null)
                    {
                        string assetPath = AssetDatabase.GetAssetPath(circleTexture);
                        defaultSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                    }
                }
            }

            if (defaultSprite != null)
            {
                particleSprite = defaultSprite;
                EditorUtility.SetDirty(this);
            }
#else
            var defaultSprite = Resources.Load<Sprite>("DefaultParticleSprite");
            if (defaultSprite != null)
                particleSprite = defaultSprite;
#endif
        }

#if UNITY_EDITOR
        private void CreateDefaultParticleTexture()
        {
            const string internalAssetsFolder = "Assets/TierStudios/UIParticleSystem/InternalAssets";
            if (!AssetDatabase.IsValidFolder(internalAssetsFolder))
            {
                if (AssetDatabase.IsValidFolder("Assets/TierStudios/UIParticleSystem"))
                    AssetDatabase.CreateFolder("Assets/TierStudios/UIParticleSystem", "InternalAssets");
            }

            const int size = 64;
            var texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
            var pixels = new Color[size * size];
            var center = new Vector2(size / 2f, size / 2f);
            float radius = size / 2f - 2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    pixels[y * size + x] = dist <= radius
                        ? new Color(1f, 1f, 1f, 1f - Mathf.Clamp01((dist - (radius - 4f)) / 4f))
                        : Color.clear;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            const string texturePath = internalAssetsFolder + "/DefaultParticleSprite.png";
            System.IO.File.WriteAllBytes(Path.Combine(Directory.GetParent(Application.dataPath).FullName, texturePath.Replace('/', Path.DirectorySeparatorChar)), texture.EncodeToPNG());
            DestroyImmediate(texture);
            AssetDatabase.Refresh();

            var importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.filterMode = FilterMode.Bilinear;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                AssetDatabase.ImportAsset(texturePath);
            }
        }

        private bool lastKnownTrailState = false;

        protected override void OnValidate()
        {
            _settingsRevision++;
            particleUpdater?.InvalidateSampledData();

            bool needsMaskUpdate = (maskable != useMasked);
            bool currentTrailState = (trailSettings != null && trailSettings.useTrails);
            bool needsTrailCleanup = (lastKnownTrailState && !currentTrailState);

            if (needsMaskUpdate || needsTrailCleanup)
            {
                EditorApplication.delayCall += () =>
                {
                    if (this == null) return;

                    if (needsMaskUpdate)
                    {
                        maskable = useMasked;
                        RecalculateMasking();
                        if (canvasRenderer != null)
                            canvasRenderer.SetMaterial(materialForRendering, mainTexture);
                        particleUpdater?.GetTrailManager()?.RefreshMaskable();
                    }

                    if (needsTrailCleanup)
                    {
                        particleUpdater?.GetTrailManager()?.Cleanup();
                        DestroyParticleFrontRenderer();
                    }

                    lastKnownTrailState = currentTrailState;
                };
            }
        }

#endif

        #endregion

        #region Lifecycle

        protected override void OnDisable()
        {
            _isTogglingActive = true;
            try
            {
                base.OnDisable();
                CancelScheduledBringParticleLayerToFront();
                CancelDelayedInitialization();
                StopEmitting();
                if (particleUpdater != null)
                {
                    particleUpdater.Cleanup();
                    particleUpdater = null;
                }
                subEmitterManager?.Cleanup();
                subEmitterManager = null;
                ClearAllParticles(this);
                DestroyParticleFrontRenderer();

                if (_meshVertices.IsCreated) _meshVertices.Dispose();
                if (_meshColors.IsCreated) _meshColors.Dispose();
                if (_meshUVs.IsCreated) _meshUVs.Dispose();
                if (_meshTriangles.IsCreated) _meshTriangles.Dispose();
                _meshBufferCapacity = 0;
                _initialized = false;

#if UNITY_EDITOR
                EditorApplication.update -= OnEditorUpdate;
                Selection.selectionChanged -= OnSelectionChanged;
#endif
            }
            finally
            {
                _isTogglingActive = false;
            }
        }

        internal void UnregisterEditorPreview()
        {
#if UNITY_EDITOR
            EditorApplication.update -= OnEditorUpdate;
            Selection.selectionChanged -= OnSelectionChanged;
#endif
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CancelScheduledBringParticleLayerToFront();
            particlePool?.Clear();
            soundEffectManager?.Clear();
            
            if (particleUpdater != null)
            {
                particleUpdater.Cleanup();
                particleUpdater = null;
            }

            subEmitterManager?.Cleanup();
            subEmitterManager = null;

            DestroyParticleFrontRenderer();

            if (_particleMesh != null)
            {
                if (Application.isPlaying) Destroy(_particleMesh);
                else DestroyImmediate(_particleMesh);
                _particleMesh = null;
            }

            if (_blankMesh != null)
            {
                if (Application.isPlaying) Destroy(_blankMesh);
                else DestroyImmediate(_blankMesh);
                _blankMesh = null;
            }

            if (_meshVertices.IsCreated) _meshVertices.Dispose();
            if (_meshColors.IsCreated) _meshColors.Dispose();
            if (_meshUVs.IsCreated) _meshUVs.Dispose();
            if (_meshTriangles.IsCreated) _meshTriangles.Dispose();

            _initialized = false;
        }

        protected override void Start()
        {
            EnsureInitialized();

            if (Application.isPlaying && !emissionSettings.onlyInEditor && emissionSettings.emitOnStart)
            {
                StartEmitting();
                isPaused = false;
            }
        }

        private void Update()
        {
            if (!Application.isPlaying || emissionSettings.onlyInEditor || isPaused) return;
            if (!isEmitting && activeParticles.Count == 0) return;

            if (!_initialized) EnsureInitialized();

            float currentTime = useUnscaledTime ? Time.unscaledTime : Time.time;
            float effectiveInterval = mobileOptimizationMode ? mobileUpdateInterval : MIN_UPDATE_INTERVAL;
            if (currentTime - lastUpdateTime < effectiveInterval) return;

            float deltaTime = Mathf.Min(currentTime - lastUpdateTime, 0.1f) * SimulationSpeed;
            UpdateSystem(deltaTime);
            lastUpdateTime = currentTime;
        }


        private void UpdateSystem(float deltaTime)
        {
            if (!isEmitting && activeParticles.Count == 0) return;

            emissionManager?.UpdateEmitterMotion(deltaTime);

            if (isEmitting && emissionManager != null)
                emissionManager.UpdateEmission(deltaTime);

            if (activeParticles.Count > 0 && particleUpdater != null)
                particleUpdater.UpdateParticles(deltaTime);

            if (soundEffectManager != null && soundEffectSettings.EnableSoundEffects)
                soundEffectManager.Update(deltaTime);

            RebuildParticleMesh();

            if (performanceMonitor != null)
            {
                performanceCheckFrame++;
                int checkInterval = mobileOptimizationMode ? 120 : 60;
                if (performanceCheckFrame >= checkInterval)
                {
                    performanceMonitor.CheckPerformance();
                    performanceCheckFrame = 0;
                }
            }
        }

        internal void SimulatePrewarmStep(float deltaTime)
        {
            particleUpdater?.UpdateParticlesForPrewarm(deltaTime);
        }

        #endregion

        #region Editor Preview

#if UNITY_EDITOR
        private void OnEditorUpdate()
        {
            if (this == null || !isActiveAndEnabled)
                return;
            if (Application.isPlaying)
                return;

            EnsureInitialized();
            if (emissionManager == null) return;

            float currentTime = (float)EditorApplication.timeSinceStartup;
            float deltaTime = currentTime - emissionManager.LastUpdateTime;
            emissionManager.LastUpdateTime = currentTime;

            if (deltaTime > 1f) deltaTime = 0.016f;

            bool shouldRun = emissionSettings != null && emissionSettings.previewInEditor
                             && (IsSelected() || IsInMultiSelection() || IsRelatedToSelection());

            if (shouldRun && isEmitting && !isPaused)
            {
                UpdateSystem(deltaTime * SimulationSpeed);
                SceneView.RepaintAll();
            }
        }

        private void OnSelectionChanged() => CheckSelection();

        private void CheckSelection()
        {
            if (this == null || Application.isPlaying) return;
            EnsureInitialized();
            if (emissionManager == null) return;

            bool selected = IsSelected() || IsInMultiSelection() || IsRelatedToSelection();
            emissionManager.HandleEditorSelection(selected);
        }

        private bool IsRelatedToSelection()
        {
            if (gameObject == null || cachedTransform == null ||
                Selection.gameObjects == null || Selection.gameObjects.Length == 0)
                return false;

            foreach (var obj in Selection.gameObjects)
            {
                if (obj == null) continue;

                var ps = obj.GetComponent<UIParticleSystem>();
                if (ps == null) continue;

                if (ps == this) return true;
                if (IsParentOf(cachedTransform, ps.Transform)) return true;
                if (IsParentOf(ps.Transform, cachedTransform)) return true;
                if (IsInSameHierarchy(ps.Transform, cachedTransform)) return true;
            }
            return false;
        }

        private bool IsInSameHierarchy(Transform a, Transform b)
        {
            if (a == null || b == null) return false;

            for (Transform p = a.parent; p != null; p = p.parent)
            {
                if (p.GetComponent<UIParticleSystem>() != null && IsChildOf(p, b))
                    return true;
            }

            for (Transform p = b.parent; p != null; p = p.parent)
            {
                if (p.GetComponent<UIParticleSystem>() != null && IsChildOf(p, a))
                    return true;
            }

            return false;
        }

        private static bool IsChildOf(Transform parent, Transform child)
        {
            if (parent == null || child == null) return false;
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform c = parent.GetChild(i);
                if (c == child || IsChildOf(c, child)) return true;
            }
            return false;
        }

        private static bool IsParentOf(Transform potentialParent, Transform potentialChild)
        {
            if (potentialParent == null || potentialChild == null) return false;
            for (Transform c = potentialChild.parent; c != null; c = c.parent)
                if (c == potentialParent) return true;
            return false;
        }

        private bool IsSelected()
            => gameObject != null && Selection.activeGameObject == gameObject;

        private bool IsInMultiSelection()
        {
            if (gameObject == null || Selection.gameObjects == null || Selection.gameObjects.Length <= 1)
                return false;

            foreach (var obj in Selection.gameObjects)
                if (obj == gameObject) return true;

            return false;
        }

        private void OnDrawGizmos()
        {
            if (IsSelected())
            {
                ParticleUtilities.DrawEmissionGizmos(this);
                ParticleUtilities.DrawCollisionGizmos(this);
                attractorManager?.DrawAttractorGizmos();
            }
        }
#endif

        #endregion

        #region Public API

        public void StartEmitting()
        {
            EnsureInitialized();
            CancelDelayedInitialization();

            if (emissionManager == null) return;

            wasEverStarted = true;
            MarkReadyToSimulate();
            emissionManager.StartEmitting();
            TriggerEmissionStarted();
        }

        public void StopEmitting()
        {
            if (emissionManager == null) return;
            emissionManager.StopEmitting();
            TriggerEmissionStopped();
        }

        public void PauseSystem()
        {
            isPaused = true;
            TriggerSystemPaused();
        }

        public void ResumeSystem()
        {
            isPaused = false;
            emissionManager?.ResetEmitterMotion();
            TriggerSystemResumed();
        }

        public void RestartSystem()
        {
            EnsureInitialized();
            CancelDelayedInitialization();

            if (emissionManager == null) return;

            wasEverStarted = true;
            MarkReadyToSimulate();
            subEmitterManager?.ResetLifetimeTriggers();
            emissionManager.RestartSystem();
            TriggerEmissionStarted();
        }

        private void MarkReadyToSimulate()
        {
            isPaused = false;
            float now = useUnscaledTime ? Time.unscaledTime : Time.time;
            lastUpdateTime = now - MIN_UPDATE_INTERVAL;
        }

        public void EmitBurst(int count)
        {
            EnsureInitialized();
            if (count <= 0) return;

            int toEmit = Mathf.Min(count, Mathf.Max(0, EffectiveMaxParticles - activeParticles.Count));
            for (int i = 0; i < toEmit; i++)
                ParticleUtilities.EmitParticle(this);
        }

        internal Vector3 GetEmitterVelocityInSimSpace()
            => emissionManager != null ? emissionManager.GetEmitterVelocityInSimSpace() : Vector3.zero;

        internal Vector3 GetEmitterDeltaInSimSpace()
            => emissionManager != null ? emissionManager.GetEmitterDeltaInSimSpace() : Vector3.zero;

        public void SetMobileOptimization(bool enabled, int maxParticles = 50, float updateInterval = 0.033f)
        {
            mobileOptimizationMode = enabled;
            mobileMaxParticles = Mathf.Max(1, maxParticles);
            mobileUpdateInterval = Mathf.Max(0.016f, updateInterval);

            if (enabled && activeParticles.Count > mobileMaxParticles)
            {
                int toRemove = activeParticles.Count - mobileMaxParticles;
                for (int i = 0; i < toRemove; i++)
                {
                    int last = activeParticles.Count - 1;
                    if (last < 0) break;
                    particlePool?.Return(activeParticles[last]);
                    activeParticles.RemoveAt(last);
                }
            }
        }

        public void CleanupTrails()
        {
            particleUpdater?.GetTrailManager()?.Cleanup();
        }

        public void RefreshTrailSettings()
        {
            EnsureInitialized();
            if (CanUseTrails)
                particleUpdater?.GetTrailManager()?.Cleanup();
        }

        public int GetActiveParticleCount() => activeParticles?.Count ?? 0;

        public void UpdateParticlesMaterial()
        {
            ApplyBlendMaterial();

#if UNITY_EDITOR
            if (!Application.isPlaying)
                EditorUtility.SetDirty(this);
#endif
        }

        public void UpdateParticlesMaskedState()
        {
            maskable = useMasked;

            RecalculateMasking();

            if (canvasRenderer != null)
                canvasRenderer.SetMaterial(materialForRendering, mainTexture);

            if (_particleFrontRenderer != null)
            {
                _particleFrontRenderer.maskable = useMasked;
                _particleFrontRenderer.RecalculateMasking();
                if (_particleFrontRenderer.canvasRenderer != null)
                    _particleFrontRenderer.canvasRenderer.SetMaterial(
                        _particleFrontRenderer.materialForRendering, mainTexture);
            }

            particleUpdater?.GetTrailManager()?.RefreshMaskable();

#if UNITY_EDITOR
            if (!Application.isPlaying)
                EditorUtility.SetDirty(this);
#endif
        }

        #endregion

        #region Particle Management

        public static void ClearAllParticles(UIParticleSystem system, bool skipSoundEffects = false)
        {
            for (int i = system.activeParticles.Count - 1; i >= 0; i--)
            {
                var particle = system.activeParticles[i];
                if (particle == null) continue;

                if (!skipSoundEffects)
                {
                    system.TriggerParticleDied(particle, i);
                }
                else if (system.eventSettings != null && system.eventSettings.EnableEvents
                         && system.OnParticleDied != null)
                {
                    if (system.cachedEventArgs == null)
                        system.cachedEventArgs = new ParticleEventArgs(particle, system);
                    else
                        system.cachedEventArgs.UpdateData(particle, system);

                    system.OnParticleDied.Invoke(system.cachedEventArgs);
                }

                particle.trail?.Clear();
                system.particlePool?.Return(particle);
            }

            system.activeParticles.Clear();
            system.particleUpdater?.GetTrailManager()?.ClearTrailVisuals();

            if (system._particleMesh != null)
            {
                system._particleMesh.Clear();
                system.SubmitParticleMesh(system._particleMesh);
            }

            system.TriggerParticlesCleared();
        }

        #endregion
    }
}
