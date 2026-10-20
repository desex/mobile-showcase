using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.DeletePopupWindow
{
    public class DeletePopupView : BaseWindowView
    {
        [SerializeField] private Button _deleteAccountButton;
        [SerializeField] private Button _closeWindowButton;

        public Observable<Unit> OnDeleteAccountPressed => _deleteAccountButton.OnClickAsObservable();
        public Observable<Unit> OnCloseWindowPressed => _closeWindowButton.OnClickAsObservable();
        
        protected override UniTask ShowInternalAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override UniTask HideInternalAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}