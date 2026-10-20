using System;
using LitMotion.Animation;
using R3;
using R3.Triggers;
using Sotbify.MobileApp.Common.SerializableDictionary;
using Sotbify.MobileApp.Utils.Enums.Blackjack;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace Sotbify.MobileApp.ChatMessages.Views
{
    public class BlackjackView : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private GameResultGameObjectDictionary _resultToVisual;
        [SerializeField] private LitMotionAnimation _animation;
        
        public Observable<Unit> PlayObservable => _playButton.OnPointerDownAsObservable().Select(_ => Unit.Default);

        public void SetResultVisual(EGameResult gameResult)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void OnEnable()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void OnDisable()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        [Serializable]
        private sealed class GameResultGameObjectDictionary : SerializableDictionary<EGameResult, GameObject> {}
    }
}