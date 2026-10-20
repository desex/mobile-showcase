using System;
using System.Collections.Generic;
using Sotbify.MobileApp.Common.SerializableDictionary;
using Sotbify.MobileApp.Utils.Enums.Blackjack;
using UnityEngine;
using UnityEngine.Localization;

namespace Sotbify.MobileApp.Db.BlackjackWindowConfig
{
    [CreateAssetMenu(menuName = "Settings/BlackjackWindow_Config", fileName = "BlackjackWindow_Config")]
    public sealed class BlackjackWindowConfig : ScriptableObject, IBlackjackWindowConfig
    {
        private readonly Dictionary<EGameResult, string> _tempResultToBoxText = new ();
        
        [SerializeField] private Vector3 _displace = new(40f, 0f, 0f);
        [SerializeField] private float _resultDelay = 1f;
        [SerializeField] private LocalizedString _winText; // ! Вы выиграли !
        [SerializeField] private LocalizedString _tieText; // Ничья!
        [SerializeField] private LocalizedString _lostText; // ! Вы проиграли !
        [SerializeField] private float _marginBetweenButtonAndResultBox = 12f;
        [SerializeField] private GameResultLocalizedStringDictionary _resultToBoxText;
        [SerializeField] private GameResultColorDictionary _fieldResultToColor;
        [SerializeField] private GameResultColorDictionary _textResultToColor;
        [SerializeField] private GameResultColorDictionary _progressStripResultToColor;
        
        public Vector3 Displace => _displace;
        public float ResultDelay => _resultDelay;
        public string WinText => _winText?.GetLocalizedString() ?? string.Empty;
        public string TieText => _tieText?.GetLocalizedString() ?? string.Empty;
        public string LostText => _lostText?.GetLocalizedString() ?? string.Empty;
        public float MarginBetweenButtonAndResultBox => _marginBetweenButtonAndResultBox;

        public IReadOnlyDictionary<EGameResult, string> ResultToBoxText
        {
            get
            {
                // Это необходимо для того чтобы работала смена локализации в runtime
                _tempResultToBoxText.Clear();
                foreach (var (key, localizedString) in _resultToBoxText) 
                    _tempResultToBoxText[key] = localizedString?.GetLocalizedString() ?? string.Empty;

                return _tempResultToBoxText;
            }
        }

        public IReadOnlyDictionary<EGameResult, Color> FieldResultToColor => _fieldResultToColor;
        public IReadOnlyDictionary<EGameResult, Color> TextResultToColor => _textResultToColor;
        public IReadOnlyDictionary<EGameResult, Color> ProgressStripResultToColor => _progressStripResultToColor;
        
        [Serializable]
        private sealed class GameResultLocalizedStringDictionary : SerializableDictionary<EGameResult, LocalizedString> { }
        
        [Serializable]
        private sealed class GameResultColorDictionary : SerializableDictionary<EGameResult, Color> { }
    }
}