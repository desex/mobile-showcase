using R3;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using UnityEngine.UI;

namespace Stab.StabProfileWindow
{
    public class StabProfileView : FadeWindowView
    {
        [SerializeField] private Button _openWarrantyButton;
        [SerializeField] private Button _openAchievementsButton;
        [SerializeField] private Button _openSettingsButton;
        [SerializeField] private Button _openPolicyButton;
        [SerializeField] private Button _openTermsButton;
        [SerializeField] private Button _deleteAccountButton;

        public Observable<Unit> OpenWarrantyObservable => _openWarrantyButton.OnClickAsObservable();
        public Observable<Unit> OpenAchievementsObservable => _openAchievementsButton.OnClickAsObservable();
        public Observable<Unit> OpenSettingsObservable => _openSettingsButton.OnClickAsObservable();
        public Observable<Unit> OpenPolicyObservable => _openPolicyButton.OnClickAsObservable();
        public Observable<Unit> OpenTermsObservable => _openTermsButton.OnClickAsObservable();
        public Observable<Unit> DeleteAccountObservable => _deleteAccountButton.OnClickAsObservable();
    }
}