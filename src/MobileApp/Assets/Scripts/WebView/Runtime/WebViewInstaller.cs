using VContainer;
using VContainer.Unity;

namespace Sotbifay.MobileApp.WebView
{
    public sealed class WebViewInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterFactory<string, IWebView>(_ =>
            {
                return url => new WebView(url);
            }, Lifetime.Singleton);
        }
    }
}