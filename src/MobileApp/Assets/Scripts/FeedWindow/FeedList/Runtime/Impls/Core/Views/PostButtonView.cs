using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.FeedWindow.FeedList.Core.Views
{
    public class PostButtonView : MonoBehaviour
    {
        [SerializeField] private Graphic _background;
        [SerializeField] private Button _selfButton;
        [SerializeField] private TMP_Text _buttonText;

        public string ButtonText
        {
            get => _buttonText.text;
            set => _buttonText.SetText(value);
        }
        
        public string ButtonHref { get; set; }

        public Color BackgroundColor
        {
            get => _background.color;
            set => _background.color = value;
        }

        public Color TextColor
        {
            get => _buttonText.color;
            set => _buttonText.color = value;
        }
        
        public Observable<Unit> OnClick => _selfButton.OnClickAsObservable();
    }
}