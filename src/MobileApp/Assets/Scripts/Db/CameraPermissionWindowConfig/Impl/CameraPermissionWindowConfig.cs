using Sotbify.MobileApp.Utils;
using UnityEngine;

namespace Sotbify.MobileApp.Db.CameraPermissionWindowConfig
{
    [CreateAssetMenu(menuName = "Settings/CameraPermissionWindow_Config", fileName = "CameraPermissionWindow_Config")]
    public class CameraPermissionWindowConfig : ScriptableObject, ICameraPermissionWindowConfig
    {
        [field: SerializeField]
        public MotionParameters AnimationTransitionMotionParameters { get; private set; }

        [field: SerializeField]
        public float SuccessWindowShowDelay { get; private set; }

        [field: SerializeField]
        public Color SuccessRadialColor { get; private set; }
        
        [field: SerializeField]
        public Color InitialRadialColor { get; private set; }
        
        [field: SerializeField]
        public Color FailedRadialColor { get; private set; }
    }
}