using Sotbify.MobileApp.Utils;
using UnityEngine;

namespace Sotbify.MobileApp.Db.CameraPermissionWindowConfig
{
    public interface ICameraPermissionWindowConfig
    {
        public MotionParameters AnimationTransitionMotionParameters { get; }

        public float SuccessWindowShowDelay { get; }
        public Color SuccessRadialColor { get; }
        public Color InitialRadialColor { get; }
        public Color FailedRadialColor { get; }
    }
}