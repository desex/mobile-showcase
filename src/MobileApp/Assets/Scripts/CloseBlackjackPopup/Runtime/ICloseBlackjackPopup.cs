using R3;

namespace Sotbify.MobileApp.CloseBlackjackPopup
{
    public interface ICloseBlackjackPopup
    {
        public Observable<Unit> OnContinuePressed { get; }
        public Observable<Unit> OnCancelPressed { get; }

        public void Continue();
        public void Cancel();
    }
}