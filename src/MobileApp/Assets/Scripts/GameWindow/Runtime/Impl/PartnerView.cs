using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.PlayWindow
{
    public class PartnerView : MonoBehaviour
    {
        [SerializeField] private Button _openButton;

        public Observable<Unit> OnClickObservable => _openButton.OnClickAsObservable();
    }
}