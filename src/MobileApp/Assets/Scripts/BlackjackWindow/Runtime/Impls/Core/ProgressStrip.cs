using System.Threading;
using Coffee.UIEffects;
using Cysharp.Threading.Tasks;
using LitMotion;
using Sirenix.OdinInspector;
using Sotbify.MobileApp.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.BlackjackWindow.Core
{
    public class ProgressStrip : MonoBehaviour
    {
        [SerializeField] private Image _targetGraphic;
        [SerializeField] private UIEffect _progressEffect;

        [MinMaxSlider(0.01f, 1f, true)] 
        [SerializeField] 
        private Vector2 _motionRange;
        
        [SerializeField] private MotionParameters _motionParameters;
        
        private CancellationTokenSource _cancellationTokenSource;

        public void ResetColor()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask PlayAnimation(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ChangeColor(Color color)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}