using R3;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using UnityEngine.UI;

namespace Stab.WarrantyWindow
{
    public class WarrantyView : FadeWindowView
    {
        [SerializeField] private Button _claimWarrantyButton;
        [SerializeField] private Button _backButton;
        
        public Observable<Unit> ClaimWarrantyObservable=> _claimWarrantyButton.OnClickAsObservable();
        public Observable<Unit> BackObservable=> _backButton.OnClickAsObservable();
    }
}