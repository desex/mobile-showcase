using R3;

namespace Sotbify.MobileApp.FAQWindow
{
    public interface IFAQContent
    {
        public Observable<Unit> BackButtonPressed { get; set; }
        public ReadOnlyReactiveProperty<FAQInfo> Content { get; }
        public void Initialize(FAQInfo content);
    }
}