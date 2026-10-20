using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sotbifay.MobileApp.WebView
{
    public interface IWebView : IDisposable
    {
        UniTask ShowAsync(CancellationToken cancellationToken = default);
    }
}