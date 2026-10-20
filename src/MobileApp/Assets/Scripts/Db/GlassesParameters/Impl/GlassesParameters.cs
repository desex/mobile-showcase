using Sirenix.OdinInspector;
using Sotbify.MobileApp.Utils;
using UnityEngine;

namespace Sotbify.MobileApp.Db.GlassesParameters
{
    [CreateAssetMenu(menuName = "Settings/" + nameof(GlassesParameters), fileName = nameof(GlassesParameters))]
    public class GlassesParameters : ScriptableParameters, IGlassesParameters
    {
        private const string TutorialFoldout = "Tutorial";
        [field: SerializeField]
        public MotionParameters CameraFadeMotionParameters { get; private set; }

        [field: SerializeField]
        public MotionParameters SuccessMotionParameters { get; private set; }
        
        [field: SerializeField]
        public MotionParameters FailedMotionParameters { get; private set; }

        [field: SerializeField]
        public MotionParameters ScanningElementsMotionParameters { get; private set; }

        [field: SerializeField]
        public MotionParameters PendingDisableMotionParameters { get; private set; }

        [field: SerializeField] 
        public Color SuccessColor { get; private set; }

        [field: SerializeField]
        public Color FailedColor{ get; private set; }

        [field: SerializeField]
        public float FrameSizeDelta { get; private set; }

        [field: SerializeField]
        [field: FoldoutGroup(TutorialFoldout)]
        public Vector2 FirstGlassesSizeDelta { get; private set; }
        
        [field: SerializeField]
        [field: FoldoutGroup(TutorialFoldout)]
        public MotionParameters FirstStepMotionParameters { get; private set; }
        
        [field: SerializeField]
        [field: FoldoutGroup(TutorialFoldout)]
        public Vector2 SecondGlassesSizeDelta { get; private set; }
        
        [field: SerializeField]
        [field: FoldoutGroup(TutorialFoldout)]
        public MotionParameters SecondStepMotionParameters { get; private set; }

        [field: SerializeField]
        [field: FoldoutGroup(TutorialFoldout)]
        public Vector2 PikesSizeDelta { get; private set; }

        [field: SerializeField]
        [field: FoldoutGroup(TutorialFoldout)]
        public float GlassesMoveDelta { get; private set; }
    }
}