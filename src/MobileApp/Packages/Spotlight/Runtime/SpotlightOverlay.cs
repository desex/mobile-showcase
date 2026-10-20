using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbifay.Spotlight
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Graphic))]
    [AddComponentMenu("UI/Spotlight/Overlay")]
    public sealed class SpotlightOverlay : Image
    {
        private const int MaxCutoutsCount = 16;
        
        private static readonly int CutoutRectsId = Shader.PropertyToID("_SpotlightCutoutRects");
        private static readonly int CutoutsCountId = Shader.PropertyToID("_SpotlightCutoutsCount");
        private static readonly int CutoutsSmoothnessId = Shader.PropertyToID("_SpotlightCutoutsSmoothness");
        private static readonly int CutoutsRadiusId = Shader.PropertyToID("_SpotlightCutoutsRadius");
        private static readonly int AspectId = Shader.PropertyToID("_SpotlightAspect");

        private readonly List<ISpotlightCutout> _cutouts = new();
        private readonly Vector4[] _cutoutRectsBuffer = new Vector4[MaxCutoutsCount];

        [Range(0, 1)]
        [SerializeField] private float _smoothness = 0.01f;
        [Range(0, 1)]
        [SerializeField] private float _cornerRadius;
        
        private bool _materialIsDirty;

        public float Smoothness
        {
            get => _smoothness;
            set
            {
                if (Mathf.Approximately(_smoothness, value))
                    return;

                _smoothness = value;
                MakeMaterialIsDirty();
            }
        }
        
        public float CornerRadius
        {
            get => _cornerRadius;
            set
            {
                if (Mathf.Approximately(_cornerRadius, value))
                    return;

                _cornerRadius = value;
                MakeMaterialIsDirty();
            }
        }

        public void RegisterCutout(ISpotlightCutout cutout)
        {
            if (_cutouts.Contains(cutout))
                return;

            _cutouts.Add(cutout);
            cutout.Changed += MakeMaterialIsDirty;

            MakeMaterialIsDirty();
        }

        public void UnregisterCutout(ISpotlightCutout cutout)
        {
            if (_cutouts.Remove(cutout))
                cutout.Changed -= MakeMaterialIsDirty;

            MakeMaterialIsDirty();
        }

        public override bool IsRaycastLocationValid(Vector2 eventPosition, Camera eventCamera)
        {
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventPosition, eventCamera, out var worldSpaceEventPosition) ==
                false)
                return false;

            foreach (var cutout in _cutouts)
            {
                var cutoutWorldSpaceRect = cutout.GetWorldSpaceRect();
                if (cutoutWorldSpaceRect.Contains(worldSpaceEventPosition))
                    return false;
            }

            return true;
        }

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();

            if (Application.isPlaying == false) 
                MakeMaterialIsDirty();
        }

#endif

        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (canvas != null)
                canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord2;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            MakeMaterialIsDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            base.OnPopulateMesh(vertexHelper);
            
            var rect = rectTransform.rect;
            var vertex = new UIVertex();

            for (var i = 0; i < vertexHelper.currentVertCount; i++)
            {
                vertexHelper.PopulateUIVertex(ref vertex, i);
                vertex.uv2 = Rect.PointToNormalized(rect, vertex.position);
                vertexHelper.SetUIVertex(vertex, i);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var cutout in _cutouts)
                cutout.Changed -= MakeMaterialIsDirty;

            _cutouts.Clear();
        }

        private void LateUpdate()
        {
            if (_materialIsDirty == false)
                return;

            try
            {
                material.SetInt(CutoutsCountId, _cutouts.Count);
                material.SetFloat(CutoutsSmoothnessId, _smoothness);
                material.SetFloat(CutoutsRadiusId, _cornerRadius);

                var worldSpaceRect = rectTransform.GetWorldSpaceRect(rectTransform.rect);
                material.SetFloat(AspectId, worldSpaceRect.width / worldSpaceRect.height);

                for (var i = 0; i < MaxCutoutsCount; i++)
                {
                    if (i < _cutouts.Count)
                    {
                        var cutout = _cutouts[i];
                        var cutoutWorldSpaceRect = cutout.GetWorldSpaceRect();
                        _cutoutRectsBuffer[i] = GetRectVectorRelative(cutoutWorldSpaceRect, worldSpaceRect);
                    }
                    else
                    {
                        _cutoutRectsBuffer[i] = Vector4.zero;
                    }
                }
                
                material.SetVectorArray(CutoutRectsId, _cutoutRectsBuffer);
            }
            finally
            {
                _materialIsDirty = false;
            }
        }

        private void MakeMaterialIsDirty() =>
            _materialIsDirty = true;

        private static Vector4 GetRectVectorRelative(Rect holeRect, Rect worldRect)
        {
            var xMin = Remap(holeRect.x, worldRect.x, worldRect.x + worldRect.width, 0, 1);
            var yMin = Remap(holeRect.y, worldRect.y, worldRect.y + worldRect.height, 0, 1);
            var xMax = Remap(holeRect.x + holeRect.width, worldRect.x, worldRect.x + worldRect.width, 0, 1);
            var yMax = Remap(holeRect.y + holeRect.height, worldRect.y, worldRect.y + worldRect.height, 0, 1);

            return new Vector4(xMin, yMin, xMax, yMax);
        }

        private static float Remap(float value, float from1, float to1, float from2, float to2)
            => (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}