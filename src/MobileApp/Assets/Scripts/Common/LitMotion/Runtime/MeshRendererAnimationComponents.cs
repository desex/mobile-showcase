using System;
using LitMotion.Animation;
using UnityEngine;

namespace Sotbify.MobileApp.Common.LitMotion
{
    [Serializable]
    [LitMotionAnimationComponentMenu("Mesh Renderer/Color")]
    public sealed class MeshRendererMaterialColorAnimationComponents : ColorPropertyAnimationComponent<MeshRenderer>
    {
        [SerializeField] private bool _useSharedMaterial = true;
        [SerializeField] private int _materialIndex;
        [SerializeField] private string _colorPropertyName = "_Color";
        
        protected override Color GetValue(MeshRenderer target)
        {
            var materials = _useSharedMaterial ? target.sharedMaterials : target.materials;
            if(_materialIndex < 0 || _materialIndex >= materials.Length)
                return Color.clear;

            var material = materials[_materialIndex];
            return material.GetColor(_colorPropertyName);
        }

        protected override void SetValue(MeshRenderer target, in Color value)
        {
            var materials = _useSharedMaterial ? target.sharedMaterials : target.materials;
            if(_materialIndex < 0 || _materialIndex >= materials.Length)
                return;

            var material = materials[_materialIndex];
            material.SetColor(_colorPropertyName, value);
        }
    }
}