using R3;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Enums.Nexus;

namespace Sotbify.MobileApp.FAQWindow
{
    public interface IFAQ
    {
        public Observable<ENexusCategory> OpenCategoryObservable { get; }
        public Observable<FAQInfo> OpenContent { get; }
        public void OpenCategory(ENexusCategory category);
        public void OpenFAQContent(FAQInfo content);
    }
}