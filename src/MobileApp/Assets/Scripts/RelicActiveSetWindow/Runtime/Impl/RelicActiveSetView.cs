using System.Collections.Generic;
using System.Linq;
using R3;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.RelicActiveSetWindow
{
    public class RelicActiveSetView : FadeWindowView
    {
        [SerializeField] private RectTransform _notEnoughRect;
        [SerializeField] private Button _randomizeButton;
        
        [field: SerializeField] public List<RelicSetLine> Lines { get; private set; }

        public Observable<int> EditRelic => Lines
            .Select(line => line.EditRelicObservable)
            .Merge();

        public Observable<Unit> RandomizeObservable => _randomizeButton.OnClickAsObservable();

        public void UpdateLines(IReadOnlyList<RelicLineModel> modelRelicLines)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ChangeRandomizeAvailability(bool isEnough)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ChangeLockStatusRandomizeButton(bool isLocked)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}