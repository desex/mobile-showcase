using LitMotion;
using Sirenix.OdinInspector;
using Sotbify.MobileApp.Utils;
using UnityEngine;

namespace Sotbify.MobileApp.Db.MainWindowConfig.Impl
{
    [CreateAssetMenu(menuName = "Settings/MainWindow_Config", fileName = "MainWindow_Config")]
    
    public class MainWindowConfig : ScriptableObject, IMainWindowConfig
    {
        private const string AnimationsFoldout = "Animations";

        [field: SerializeField] 
        public float FeedPreloadIndent { get; private set; } = 0.1f;

        [field: SerializeField]
        [field: FoldoutGroup(AnimationsFoldout)]
        [field: MinMaxSlider(-50f, 50f, true)]
        public Vector2 MoveRange { get; private set; }
        
        [field: SerializeField]
        [field: FoldoutGroup(AnimationsFoldout)]
        [field: MinMaxSlider(0, 360f, true)]
        public Vector2 RotationRange { get; private set; }
        
        [field: SerializeField]
        [field: FoldoutGroup(AnimationsFoldout)]
        [field: MinMaxSlider(0f, 5f, true)]
        public Vector2 MotionDurationRange { get; private set; }

        [field: SerializeField]
        [field: FoldoutGroup(AnimationsFoldout)]
        public float DelayBetweenMotions { get; private set; }

        [field: SerializeField]
        [field: FoldoutGroup(AnimationsFoldout)]
        public Ease MoveEase { get; private set; }

        [field: SerializeField]
        [field: FoldoutGroup(AnimationsFoldout)]
        public MotionParameters ScrollMotionParameters { get; private set; }
    }
}