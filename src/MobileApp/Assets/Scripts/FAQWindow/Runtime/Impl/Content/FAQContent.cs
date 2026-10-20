using System;
using R3;

namespace Sotbify.MobileApp.FAQWindow.Content
{
    public class FAQContent : IFAQContent, IDisposable
    {
        private readonly ReactiveProperty<FAQInfo> _content = new();
        public Observable<Unit> BackButtonPressed { get; set; }
        
        public ReadOnlyReactiveProperty<FAQInfo> Content => _content;

        public void Initialize(FAQInfo content)
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