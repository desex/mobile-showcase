using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

using UObject = UnityEngine.Object;

namespace Sotbifay.MobileApp.WebView
{
    public sealed class WebView : IWebView
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        
        private readonly string _url;
        private UniWebView _webView;
        private UniWebViewSafeBrowsing _safeBrowsing;

        public WebView(string url)
        {
            _url = url ?? throw new ArgumentNullException(nameof(url));
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            if(_webView != null)
                throw new InvalidOperationException("WebView has already been showed");
            
            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
            
#if UNITY_EDITOR || UNITY_STANDALONE
            Application.OpenURL(_url);
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellationTokenSource.Token);
#else
            await ShowInternalAsync(cancellationTokenSource.Token);
#endif
        }
        
        public void Dispose()
        {
            if(_webView != null)
                UObject.Destroy(_webView.gameObject);
            
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private async UniTask ShowInternalAsync(CancellationToken cancellationToken)
        {
            if (UniWebViewSafeBrowsing.IsSafeBrowsingSupported)
                await ShowSafeBrowsingAsync(cancellationToken);
            else
                await ShowUniBrowsingAsync(cancellationToken);
        }

        private async UniTask ShowSafeBrowsingAsync(CancellationToken cancellationToken)
        {
            var closeWebViewCompletionSource = new UniTaskCompletionSource();
            
            try
            {
                _safeBrowsing = UniWebViewSafeBrowsing.Create(_url);
                _safeBrowsing.Show();

                _safeBrowsing.OnSafeBrowsingFinished += OnShouldCloseSafe;

                await closeWebViewCompletionSource.Task
                    .AttachExternalCancellation(cancellationToken);
            }
            finally
            {
                _safeBrowsing.OnSafeBrowsingFinished -= OnShouldCloseSafe;
            }

            return;
            
            void OnShouldCloseSafe(UniWebViewSafeBrowsing browsing)
            {
                closeWebViewCompletionSource.TrySetResult();
            }
        }

        private async UniTask ShowUniBrowsingAsync(CancellationToken cancellationToken)
        {
            var closeWebViewCompletionSource = new UniTaskCompletionSource();
            
            try
            {
                var gameObject = new GameObject("WebView");
                UObject.DontDestroyOnLoad(gameObject);
                _webView = gameObject.AddComponent<UniWebView>();
                
                _webView.OnShouldClose += OnShouldClose;
                _webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
                _webView.Load(_url);
                _webView.Show(edge: UniWebViewTransitionEdge.Bottom);
                _webView.EmbeddedToolbar.Show();

                await closeWebViewCompletionSource.Task
                    .AttachExternalCancellation(cancellationToken);
            }
            finally
            {
                _webView.OnShouldClose -= OnShouldClose;
            }

            return;
            
            bool OnShouldClose(UniWebView webview)
            {
                closeWebViewCompletionSource.TrySetResult();
                return true;
            }
        }
    }
}