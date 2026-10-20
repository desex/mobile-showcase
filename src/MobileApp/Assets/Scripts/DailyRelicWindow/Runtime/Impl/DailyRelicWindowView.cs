using R3;
using Sotbify.MobileApp.UI.Window.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.DailyRelicWindow
{
    public class DailyRelicWindowView : FadeWindowView
    {
        [SerializeField] private RawImage _relicImage;
        [SerializeField] private TMP_Text _dateText;
        [SerializeField] private TMP_Text _relicName;
        [SerializeField] private TMP_Text _relicShortDescription;
        [SerializeField] private Button _okayButton;

        public Observable<Unit> OkayObservable => _okayButton.OnClickAsObservable();

        public Texture Preview { set => _relicImage.texture = value; }
        public string Date { set => _dateText.text = value; }
        public string RelicName { set => _relicName.text = value; }
        public string RelicShortDescription { set => _relicShortDescription.text = value; }
    }
}