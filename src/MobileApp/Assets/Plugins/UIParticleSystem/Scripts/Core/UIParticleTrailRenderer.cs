using UnityEngine;
using UnityEngine.UI;
using TierStudios.UIParticleSystem.Utilities;

namespace TierStudios.UIParticleSystem.Core
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(CanvasRenderer))]
    public class UIParticleTrailRenderer : MaskableGraphic
    {
        private Mesh _trailMesh;
        public Texture textureOverride;

        public override Texture mainTexture
        {
            get
            {
                if (textureOverride != null)
                    return textureOverride;
                return base.mainTexture;
            }
        }

        public Mesh TrailMesh
        {
            get
            {
                if (_trailMesh == null)
                {
                    _trailMesh = new Mesh();
                    _trailMesh.name = "UIParticleTrailMesh";
                    _trailMesh.MarkDynamic();
                }
                return _trailMesh;
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }

        public void ApplyMesh(Mesh mesh)
        {
            if (!CanAccessRenderer(out CanvasRenderer renderer))
                return;

            renderer.SetMesh(mesh);
            CanvasGroupAlphaUtility.ApplyToRenderer(this);
            SetMaterialDirty();
        }

        public void Clear()
        {
            if (!CanAccessRenderer(out CanvasRenderer renderer))
                return;

            TrailMesh.Clear();
            renderer.SetMesh(TrailMesh);
            CanvasGroupAlphaUtility.ApplyToRenderer(this);
            SetMaterialDirty();
        }

        private bool CanAccessRenderer(out CanvasRenderer renderer)
        {
            renderer = null;
            if (this == null)
                return false;

            renderer = GetComponent<CanvasRenderer>();
            return renderer != null;
        }

        protected override void OnCanvasGroupChanged()
        {
            base.OnCanvasGroupChanged();
            CanvasGroupAlphaUtility.ApplyToRenderer(this);
        }

        protected override void UpdateMaterial()
        {
            base.UpdateMaterial();
            CanvasGroupAlphaUtility.ApplyToRenderer(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_trailMesh != null)
            {
                if (Application.isPlaying) Destroy(_trailMesh);
                else DestroyImmediate(_trailMesh);
                _trailMesh = null;
            }
        }
    }
}