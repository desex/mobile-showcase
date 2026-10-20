using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using Sotbify.MobileApp.Common.SerializableDictionary;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Enums.Blackjack;
using Sotbify.MobileApp.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.BlackjackWindow.Core
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _rank;
        [SerializeField] private CanvasGroup _backGroup;
        [SerializeField] private Image[] _suits;
        [SerializeField] private SuitToColor _suitToColor;
        [SerializeField] private SuitToSprite _suitToSprite;

        private ESuit _suit;
        
        public string Rank { set => _rank.text = value; }
        public ESuit Suit
        {
            get => _suit;
            set
            {
                _suit = value;
                _rank.color = _suitToColor[value];
                ActivateVisual(value);
            }
        }

        private void ActivateVisual(ESuit suit)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public UniTask BackOpacityTask(MotionParameters motionParameters, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }

    [Serializable]
    public class SuitToColor : SerializableDictionary<ESuit, Color>
    {
    }
    
    [Serializable]
    public class SuitToSprite : SerializableDictionary<ESuit, Sprite>
    {
    }
}