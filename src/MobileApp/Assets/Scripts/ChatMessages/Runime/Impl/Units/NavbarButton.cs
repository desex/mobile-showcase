using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.ChatMessages.Units
{
    [RequireComponent(typeof(Button))]
    public class NavbarButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _message;

        public Observable<string> OnButtonClicked => _button
            .OnClickAsObservable()
            .Select(_message, static(_, message) => message.text);
    }
}