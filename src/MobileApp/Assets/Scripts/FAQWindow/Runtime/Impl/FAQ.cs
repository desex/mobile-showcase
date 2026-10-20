using System;
using R3;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Enums.Nexus;

namespace Sotbify.MobileApp.FAQWindow
{
    public class FAQ : IFAQ, IDisposable
    {
        private readonly Subject<ENexusCategory> _openCategory = new();
        private readonly Subject<FAQInfo> _openContent = new();
        
        public Observable<ENexusCategory> OpenCategoryObservable => _openCategory;
        public Observable<FAQInfo> OpenContent => _openContent;

        public void OpenCategory(ENexusCategory category)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void OpenFAQContent(FAQInfo content)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Dispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}