using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using Sotbifay.MobileApp.WebView;

namespace Sotbify.MobileApp.Services.Browser
{
    public class Browser : IBrowser, IDisposable
    {
        private readonly Func<string, IWebView> _webViewFactory;
        private readonly ILogger _logger;
        
        private CancellationTokenSource _tokenSource;

        public Browser(
            Func<string, IWebView> webViewFactory,
            ILogger logger
        )
        {
            _webViewFactory = webViewFactory;
            _logger = logger;
        }
        
        public async UniTask OpenLink(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                using var webView = _webViewFactory.Invoke(url);
                _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.Information("Opened web view");

                await webView.ShowAsync(_tokenSource.Token);

                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.Information("Web view closed");
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                    _logger.Error(e.ToString());
            }
        }

        public void Dispose()
        {
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
        }
    }
}