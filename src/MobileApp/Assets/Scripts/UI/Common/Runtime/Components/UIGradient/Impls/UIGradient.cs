using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.UI.Common.Components.UIGradient
{
    public class UIGradient : BaseMeshEffect, IUIGradient
    {
        [SerializeField] private Color _colorFrom;
        [SerializeField] private Color _colorTo;
        
        [SerializeField]
        [Range(-180f, 180f)]
        private float _angle;
        [SerializeField] private bool _isRatioIgnored = true;

        public (Color, Color) Gradient => (_colorFrom, _colorTo);

        public void SetGradient(Color newFrom, Color newTo)
        {
            _colorFrom = newFrom;
            _colorTo = newTo;
            graphic.SetVerticesDirty();
        }
        
        public override void ModifyMesh(VertexHelper vertexHelper)
        {
            if (!enabled) 
                return;
            
            var rectTransform = graphic.rectTransform.rect;
            var rotationDirection = UIGradientUtils.RotationDirection(_angle);

            if (!_isRatioIgnored)
                rotationDirection = UIGradientUtils.CompensateAspectRatio(rectTransform, rotationDirection);

            var localPositionMatrix = UIGradientUtils.LocalPositionMatrix(rectTransform, rotationDirection);

            var vertex = default(UIVertex);
            
            for (var i = 0; i < vertexHelper.currentVertCount; i++) 
            {
                vertexHelper.PopulateUIVertex (ref vertex, i);
                var localPosition = localPositionMatrix * vertex.position;
                vertex.color *= Color.Lerp(_colorTo, _colorFrom, localPosition.y);
                vertexHelper.SetUIVertex (vertex, i);
            }
        }
    }   
}