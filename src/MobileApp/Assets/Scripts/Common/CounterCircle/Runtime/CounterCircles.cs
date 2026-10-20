using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using R3;
using Sirenix.OdinInspector;
using Sotbifay.MobileApp.Common.SnapScroll;
using UnityEngine;
using UnityEngine.Pool;

namespace Sotbifay.MobileApp.Common.CounterCircle
{
    public abstract class CounterCircles : MonoBehaviour
    {
        [SerializeField] private SnapScrollView _snapScroll;
        
        [ListDrawerSettings(ShowIndexLabels = true)]
        [SerializeField] 
        private List<Circle> _counterCircles;

        private IDisposable _disposable;
        private int? _previousIndex;
        
        protected abstract Ease CircleEase { get; set; }
        protected abstract float TransitionDuration { get; set; }

        private void OnEnable()
        {
            _disposable = _snapScroll.SnappedItemIndex
                .SubscribeAwait(this, 
                    static(index, @this, cancellationToken) => 
                        @this.ChangeCircle(index, cancellationToken).AsValueTask());
        }

        private async UniTask ChangeCircle(int targetIndex, CancellationToken cancellationToken)
        {
            using (ListPool<UniTask>.Get(out var taskPool))
            {
                if (_previousIndex.HasValue)
                {
                    var deactivateTask = _counterCircles[_previousIndex.Value].Deactivate(CircleEase, TransitionDuration, cancellationToken);
                    taskPool.Add(deactivateTask);
                }
            
                var activateTask = _counterCircles[targetIndex].Activate(CircleEase, TransitionDuration, cancellationToken);
                taskPool.Add(activateTask);
                _previousIndex = targetIndex;
                
                await UniTask.WhenAll(taskPool);
            }
            
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }
    }
}