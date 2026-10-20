using R3;

namespace Sotbify.MobileApp.DeletePopupWindow
{
    public interface IDeletePopup
    {
        public Observable<Unit> OnDeletePressed { get; }
        public Observable<Unit> OnClosePressed { get; }

        public void Delete();
        public void Close();
    }
}