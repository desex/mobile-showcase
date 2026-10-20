using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using Best.HTTP;
using Best.HTTP.Hosts.Settings;
using Best.HTTP.HostSetting;
using Best.HTTP.Response;
using Best.HTTP.Shared;
using Best.HTTP.Shared.PlatformSupport.Memory;
using Best.HTTP.Shared.PlatformSupport.Network.DNS.Cache;
using Cysharp.Threading.Tasks;
using Sotbifay.Launcher.Server;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Server.HttpClient.Core;
using Sotbify.MobileApp.Server.HttpClient.Core.Types;
using Sotbify.MobileApp.Server.HttpClient.Implementations;
using UnityEngine;
using UnityEngine.Pool;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.Server
{
    public sealed class BestHttpClient : IHttpClient, IDisposable
    {
        private const string MockDirectoryName = "HttpMocks";
        private const string MockExtension = ".json";

        private const long MaxDownloadBufferSize = 2048 * 2048;
        private const int MaxActiveRequestsCount = 30;

        public event Action OnRequestTimeout;
        public event Action OnRequestSucceed;

        private readonly SemaphoreSlim _semaphoreSlim = new(MaxActiveRequestsCount, MaxActiveRequestsCount);

        private readonly ObjectPool<StringBuilder> _stringBuilderPool =
            new(static () => new StringBuilder(), actionOnRelease: static sb => sb.Clear());

        private readonly ILogger _logger;

        public BestHttpClient(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async UniTask<HttpResponse> SendAsync(HttpRequest request, CancellationToken cancellationToken = default)
        {
#if UNITY_EDITOR
            var mockResponse = await MockRequestOrNullAsync(request, cancellationToken);
            if (mockResponse.HasValue)
            {
                LogRequestResult(request, mockResponse.Value, true);
                OnRequestSucceed?.Invoke();
                return mockResponse.Value;
            }
#endif
            var httpRequest = CreateRequest(request);
            AddHeaders(request, httpRequest);
            LogRequestCreated(request);

            try
            {
                using var response = await httpRequest.GetHTTPResponseAsync(cancellationToken);
                var convertedResponse = ConvertResponse(response);
                LogRequestResult(request, convertedResponse);
                OnRequestSucceed?.Invoke();
                return convertedResponse;
            }
            catch (TimeoutException timeoutException)
            {
                LogRequestError(request, timeoutException);
                OnRequestTimeout?.Invoke();
                throw;
            }
            catch (AsyncHTTPException asyncHttpException)
            {
                LogRequestError(request, asyncHttpException);
                if (asyncHttpException.StatusCode == 0)
                {
                    OnRequestTimeout?.Invoke();
                }

                throw;
            }
            catch (Exception e)
            {
                LogRequestError(request, e);
                throw;
            }
        }

        public async UniTask<HttpResponse> DownloadAsync
        (
            HttpRequest request,
            Stream stream,
            IProgress<DownloadReport> progress = null,
            CancellationToken cancellationToken = default
        )
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            var lastBytesDownloaded = 0L;
            var lastProgressHandle = DateTime.Now;
            var httpRequest = CreateRequest(request);

            AddHeaders(request, httpRequest);
            LogRequestCreated(request);

            CancellationTokenSource downloadCancellationTokenSource = null;
            var downloadSettings = httpRequest.DownloadSettings;
            var downloadStreamCompletionSource = new UniTaskCompletionSource<DownloadContentStream>();
            try
            {
                downloadSettings.OnDownloadProgress += ProgressHandler;
                downloadSettings.OnDownloadStarted += OnDownloadStarted;

                // ReSharper disable once PossiblyMistakenUseOfCancellationToken
                using var response = await httpRequest.GetHTTPResponseAsync(cancellationToken);
                var convertedResponse = ConvertResponse(response);

                // ReSharper disable once PossiblyMistakenUseOfCancellationToken
                await using var downloadStream =
                    await downloadStreamCompletionSource.Task.AttachExternalCancellation(cancellationToken);
                if (downloadStream.CompletedWith is not null)
                    throw downloadStream.CompletedWith;

                LogRequestResult(request, convertedResponse);

                return convertedResponse;
            }
            catch (Exception e)
            {
                LogRequestError(request, e);
                throw;
            }
            finally
            {
                downloadSettings.OnDownloadProgress -= ProgressHandler;
                downloadSettings.OnDownloadStarted -= OnDownloadStarted;

                downloadCancellationTokenSource?.Dispose();
            }

            void ProgressHandler(HTTPRequest _, long bytesDownloaded, long bytesLength)
            {
                if (progress is null)
                    return;

                var now = DateTime.Now;
                var deltaTime = now - lastProgressHandle;

                var deltaBytesDownloaded = Mathf.Max(0, bytesDownloaded - lastBytesDownloaded);
                var bytesPerSecond = Math.Max(0, deltaBytesDownloaded / deltaTime.TotalSeconds);

                var report = new DownloadReport(bytesDownloaded, bytesLength, bytesPerSecond);
                progress.Report(report);

                lastBytesDownloaded = bytesDownloaded;
                lastProgressHandle = now;
            }

            // ReSharper disable once AsyncVoidMethod
            [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
            async void OnDownloadStarted(HTTPRequest req, HTTPResponse resp,
                DownloadContentStream downloadContentStream)
            {
                lastBytesDownloaded = 0;

                downloadCancellationTokenSource?.Cancel();
                downloadCancellationTokenSource?.Dispose();
                downloadCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                try
                {
                    await HandleDownloadAsync(downloadContentStream, stream, downloadCancellationTokenSource.Token);
                    downloadStreamCompletionSource.TrySetResult(downloadContentStream);
                }
                catch (OperationCanceledException e)
                {
                    if (cancellationToken.IsCancellationRequested)
                        downloadStreamCompletionSource.TrySetException(e);
                }
                catch (Exception e)
                {
                    downloadStreamCompletionSource.TrySetException(e);
                }
            }
        }

        public async UniTask ResetConnectionsAsync(CancellationToken cancellationToken = default)
        {
            await UniTask.SwitchToMainThread(cancellationToken);

            try
            {
                if(BestHttpUtils.HttpManager_SetIsQuitting(true) == false)
                    _logger.Warning("Failed to set IsQuitting to true", context: this);
                
                if (BestHttpUtils.RequestEventHelper_AbortAllRequests() == false) 
                    _logger.Warning("Failed to abort all requests", context: this);
                
                await UniTask.DelayFrame(5, cancellationToken: cancellationToken);
                
                if (BestHttpUtils.RequestEventHelper_RemoveAllRegisteredRequests() == false) 
                    _logger.Warning("Failed to remove all registered requests", context: this);
                
                if(BestHttpUtils.RequestEventHelper_Clear() == false)
                    _logger.Warning("Failed to clear request events", context: this);
                
                if (BestHttpUtils.ConnectionEvents_Clear()) 
                    _logger.Warning("Failed to clear connection events", context: this);
                
                HostManager.Shutdown();
                await UniTask.DelayFrame(5, cancellationToken: cancellationToken);
                HostManager.Clear();
                
                DNSCache.Clear();
                HTTPManager.PerHostSettings.Clear();
                HTTPManager.PerHostSettings.Add("*", new HostSettings());
                
                HTTPManager.LocalCache = null;
                
                await UniTask.NextFrame(cancellationToken: cancellationToken);
                
                if(BestHttpUtils.HttpManager_SetIsQuitting(false) == false)
                    _logger.Warning("Failed to set IsQuitting to false", context: this);
                
                if (BestHttpUtils.HttpManager_SetIsSetupCalled(false) == false)
                    _logger.Warning("Failed to set IsSetupCalled to false", context: this);
                
                HTTPManager.Setup();
                _logger.Information("HTTP connections reset completed", context: this);
            }
            catch (Exception e)
            {
                _logger.Error("Failed to reset connections", e, this);

                try
                {
                    if(BestHttpUtils.HttpManager_SetIsQuitting(false) && BestHttpUtils.HttpManager_SetIsSetupCalled(false))
                        HTTPManager.Setup();
                }
                catch
                {
                    // Ignore 🤡
                }
            }
        }

        public void Dispose()
        {
            _semaphoreSlim.Dispose();
            _stringBuilderPool.Dispose();
        }

        private async UniTask<HttpResponse?> MockRequestOrNullAsync(
            HttpRequest request,
            CancellationToken cancellationToken
        )
        {
            var mocksDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", MockDirectoryName));
            if (Directory.Exists(mocksDirectory) == false)
                return null;

            var normalizedRequestPath = request.Path.TrimStart('/', '\\')
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);

            var localPath = $"{normalizedRequestPath}.{request.Method}{MockExtension}";
            var mockFilePath = Path.Combine(mocksDirectory, localPath);

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Check mock: {mockFilePath}", context: this);

            if (File.Exists(mockFilePath) == false)
                return null;

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Found mock: {mockFilePath}", context: this);

            var data = await File.ReadAllBytesAsync(mockFilePath, cancellationToken);
            var json = Encoding.UTF8.GetString(data ?? Array.Empty<byte>());

            return new HttpResponse(200, data, json, string.Empty);
        }

        private static HTTPRequest CreateRequest(HttpRequest request)
        {
            var urlBuilder = new UriBuilder(request.Url);

            if (string.IsNullOrEmpty(request.Path) == false)
                urlBuilder.WithPathSegment(request.Path);

            if (request.Parameters.Count > 0)
            {
                foreach (var (key, value) in request.Parameters)
                    urlBuilder.WithParameter(key, value);
            }

            switch (request.Method)
            {
                case HttpMethod.Get:
                {
                    var httpRequest = HTTPRequest.CreateGet(urlBuilder.ToString());
                    httpRequest.DownloadSettings.ContentStreamMaxBuffered = MaxDownloadBufferSize;
                    return httpRequest;
                }
                case HttpMethod.Head:
                {
                    var httpRequest = new HTTPRequest(urlBuilder.ToString(), HTTPMethods.Head)
                    {
                        DownloadSettings =
                        {
                            ContentStreamMaxBuffered = MaxDownloadBufferSize
                        },
                    };
                    return httpRequest;
                }
                case HttpMethod.Post:
                {
                    var httpRequest = HTTPRequest.CreatePost(urlBuilder.ToString());
                    httpRequest.UploadSettings.UploadStream = new RawJsonDataStream(request.Body);
                    return httpRequest;
                }
                case HttpMethod.Put:
                {
                    var httpRequest = HTTPRequest.CreatePut(urlBuilder.ToString());
                    httpRequest.UploadSettings.UploadStream = new RawJsonDataStream(request.Body);
                    return httpRequest;
                }
                case HttpMethod.Patch:
                {
                    var httpRequest = new HTTPRequest(urlBuilder.ToString(), HTTPMethods.Patch)
                    {
                        UploadSettings =
                        {
                            UploadStream = new RawJsonDataStream(request.Body)
                        }
                    };
                    return httpRequest;
                }
                case HttpMethod.Delete:
                {
                    var httpRequest = new HTTPRequest(urlBuilder.ToString(), HTTPMethods.Delete)
                    {
                        UploadSettings =
                        {
                            UploadStream = new RawJsonDataStream(request.Body)
                        }
                    };
                    return httpRequest;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void AddHeaders(HttpRequest request, HTTPRequest httpRequest)
        {
            foreach (var (key, value) in request.Headers)
                httpRequest.SetHeader(key, value);
        }

        private static HttpResponse ConvertResponse(HTTPResponse response)
        {
            return new HttpResponse
            (
                response.StatusCode,
                response.Data,
                response.DataAsText,
                response.IsSuccess == false ? response.Message : string.Empty
            );
        }

        private static UniTask HandleDownloadAsync(DownloadContentStream downloadStream, Stream stream,
            CancellationToken cancellationToken)
        {
            return UniTask.RunOnThreadPool(() =>
            {
                while (downloadStream.IsCompleted == false && cancellationToken.IsCancellationRequested == false)
                {
                    if (downloadStream.TryTake(out var buffer) == false)
                        continue;

                    try
                    {
                        stream.Write(buffer, buffer.Offset, buffer.Count);
                        stream.Flush();
                    }
                    finally
                    {
                        BufferPool.Release(buffer);
                    }
                }
            }, true, cancellationToken);
        }

        private void LogRequestCreated(HttpRequest request)
        {
            if (_logger.IsEnabled(LogLevel.Debug) == false)
                return;

            using (_stringBuilderPool.Get(out var stringBuilder))
            {
                stringBuilder
#if UNITY_EDITOR
                    .Append("Request <color=#FFED63>created</color>:").AppendLine()
#else
                    .Append("Request created:").AppendLine()
#endif
                    .Append(request);

                _logger.Debug(stringBuilder.ToString(), context: this);
            }
        }

        private void LogRequestResult(HttpRequest request, HttpResponse response, bool isMock = false)
        {
            if (_logger.IsEnabled(LogLevel.Debug) == false)
                return;

            using (_stringBuilderPool.Get(out var stringBuilder))
            {
                stringBuilder
#if UNITY_EDITOR
                    .Append("Request <color=#68FF63>successful</color>:").AppendLine()
#else
                    .Append("Request successful:").AppendLine()
#endif
                    .Append(request).AppendLine()
                    .Append("Is mock: ").Append(isMock).AppendLine()
                    .Append("Response:").AppendLine()
                    .Append(response);

                _logger.Debug(stringBuilder.ToString(), context: this);
            }
        }

        private void LogRequestError(HttpRequest request, Exception exception = null)
        {
            if (_logger.IsEnabled(LogLevel.Error) == false)
                return;

            using (_stringBuilderPool.Get(out var stringBuilder))
            {
                stringBuilder
#if UNITY_EDITOR
                    .Append("Request <color=#FF6363>failed</color>.").AppendLine()
#else
                    .Append("Request failed.").AppendLine()
#endif
                    .Append(request);

                _logger.Error(stringBuilder.ToString(), exception, context: this);
            }
        }
    }
}