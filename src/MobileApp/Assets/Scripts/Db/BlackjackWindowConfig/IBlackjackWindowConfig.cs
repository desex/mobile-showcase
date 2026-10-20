using System.Collections.Generic;
using Sotbify.MobileApp.Utils.Enums.Blackjack;
using UnityEngine;

namespace Sotbify.MobileApp.Db.BlackjackWindowConfig
{
    public interface IBlackjackWindowConfig
    {
        public Vector3 Displace { get; }
        public float ResultDelay { get; }
        public float MarginBetweenButtonAndResultBox { get; }
        public string WinText { get; }
        public string TieText { get; }
        public string LostText { get; }
        public IReadOnlyDictionary<EGameResult, string> ResultToBoxText { get; }
        public IReadOnlyDictionary<EGameResult, Color> FieldResultToColor { get; }
        public IReadOnlyDictionary<EGameResult, Color> TextResultToColor { get; }
        public IReadOnlyDictionary<EGameResult, Color> ProgressStripResultToColor { get; }
    }
}