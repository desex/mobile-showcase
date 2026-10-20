using Sotbify.MobileApp.Utils;
using UnityEngine;

namespace Sotbify.MobileApp.Db.GlassesParameters
{
    public interface IGlassesParameters
    {
        public MotionParameters CameraFadeMotionParameters { get; }
        public MotionParameters SuccessMotionParameters { get; }
        public MotionParameters FailedMotionParameters { get; }
        public MotionParameters ScanningElementsMotionParameters { get; }
        public MotionParameters PendingDisableMotionParameters { get; }
        public Color SuccessColor { get; }
        public Color FailedColor { get; }
        public float FrameSizeDelta { get; }
        public Vector2 FirstGlassesSizeDelta { get; }
        public MotionParameters FirstStepMotionParameters { get; }
        public Vector2 SecondGlassesSizeDelta { get; }
        public MotionParameters SecondStepMotionParameters { get; }
        public Vector2 PikesSizeDelta { get; }
        public float GlassesMoveDelta { get; }
    }
}