using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.UI.GeneralElements.Arrow
{
    public class ArrowView : MonoBehaviour
    {
        [SerializeField] private Image _arrowImage;

        [SerializeField] 
        [MinMaxSlider(0f, 100f, true)]
        private Vector2 _arrowOpacity;

        [SerializeField] private float _arrowOffset;
        [SerializeField] private float _arrowDelay;
        [SerializeField] private MotionParameters _arrowParameters;

        private async void Start()
        {
            try
            {
                var arrowInitialOpacity = _arrowOpacity.x;
                var arrowTargetOpacity = _arrowOpacity.y;
                var arrowInitialPosition = _arrowImage.rectTransform.anchoredPosition;
                var arrowTargetPosition = _arrowImage.rectTransform.anchoredPosition + Vector2.up * _arrowOffset;
            
                while (destroyCancellationToken.IsCancellationRequested == false)
                {
                    await UniTask.WaitForSeconds(_arrowDelay, cancellationToken: destroyCancellationToken);

                    var offsetTask = _arrowImage.rectTransform.CreateAnchoredPositionAnimation(
                        arrowInitialPosition, arrowTargetPosition,
                        _arrowParameters,
                        token: destroyCancellationToken
                    );

                    var opacityTask = _arrowImage.CreateOpacityTask(
                        arrowInitialOpacity, arrowTargetOpacity,
                        _arrowParameters,
                        token: destroyCancellationToken);
                
                    await UniTask.WhenAll(offsetTask, opacityTask);

                    await UniTask.WaitForSeconds(_arrowDelay, cancellationToken: destroyCancellationToken);
                
                    var reversedOffsetTask =  _arrowImage.rectTransform.CreateAnchoredPositionAnimation(
                        arrowTargetPosition, arrowInitialPosition,
                        _arrowParameters,
                        token: destroyCancellationToken
                    );
                
                    var reversedOpacityTask = _arrowImage.CreateOpacityTask(
                        arrowTargetOpacity, arrowInitialOpacity,
                        _arrowParameters,
                        token: destroyCancellationToken);
                
                    await UniTask.WhenAll(reversedOffsetTask, reversedOpacityTask);
                }
            }
            catch (Exception)
            {
                // Ignore 🤡
            }
        }
    }   
}