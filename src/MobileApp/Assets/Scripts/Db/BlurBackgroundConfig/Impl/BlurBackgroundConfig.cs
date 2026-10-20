using UnityEngine;

namespace Sotbify.MobileApp.Db.BlurBackgroundConfig
{
    [CreateAssetMenu(menuName = "Settings/BlurBackground_Config", fileName = "BlurBackground_Config")]
    public class BlurBackgroundConfig : ScriptableObject, IBlurBackgroundConfig
    {
        [SerializeField] private Material _riobetBackground;

        public Material TargetMaterial =>
#if PARTNER_RIOBET
            _riobetBackground;
#elif UNITY_EDITOR
            _riobetBackground;
#else
            _riobetBackground;
#endif
    }
}