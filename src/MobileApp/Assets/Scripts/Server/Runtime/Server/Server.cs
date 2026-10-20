using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.Localization;
using Sotbify.MobileApp.Server.HttpClient.Core;
using Sotbify.MobileApp.Server.HttpClient.Core.Exceptions;
using Sotbify.MobileApp.Server.HttpClient.Core.Types;
using Sotbify.MobileApp.Server.Serialization;
using Sotbify.MobileApp.Services.RemoteConfig;
using Sotbify.MobileApp.Utils.Schemas;
using UnityEngine.Pool;

namespace Sotbify.MobileApp.Server
{
    public class Server : IServer
    {
        private string AuthPath => _remoteConfigService.GetString("SignIn");
        private readonly IObjectPool<HttpRequestBuilder> _httpRequestBuildersPool;
        private readonly ISerializer _serializer;
        private readonly ILocalizationService _localizationService;
        private readonly IHttpClient _httpClient;

        private AsyncLazy<SignInSchema> _authenticationLazyTask;
        private SignInSchema _signInSchema;
        private readonly IRemoteConfigService _remoteConfigService;

        private string ApiUrl =>
#if DEBUG
            _remoteConfigService.GetString("DevAPI");
#else
            _remoteConfigService.GetString("ProdAPI");
#endif

        public Server
        (
            IObjectPool<HttpRequestBuilder> httpRequestBuildersPool,
            ISerializer serializer,
            ILocalizationService localizationService,
            IHttpClient httpClient, 
            IRemoteConfigService remoteConfigService
        )
        {
            _httpRequestBuildersPool = httpRequestBuildersPool ?? throw new ArgumentNullException(nameof(httpRequestBuildersPool));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _remoteConfigService = remoteConfigService ?? throw new ArgumentNullException(nameof(remoteConfigService));
        }

        public string AccessToken => _signInSchema?.AccessToken ?? string.Empty;
        
        public UniTask<SignInSchema> AuthenticationAsync(string qrContent, CancellationToken cancellationToken = default)
        {
            return SingleRequestAuthenticationInternalAsync(qrContent, cancellationToken)
                .ContinueWith(accessTokenSchema => _signInSchema = accessTokenSchema);
        }

        public async UniTask<HttpResponse> SendAsync(
            string qrContent,
            HttpRequest request,
            CancellationToken cancellationToken = default,
            bool ignoreDefaultRequestSetup = false
        )
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                requestBuilder.CopyFrom(request);

                if (request.Url is null)
                    requestBuilder.SetUrl(new Uri(ApiUrl));

                var isRetry = false;
                do
                {
                    if (ignoreDefaultRequestSetup == false)
                    {
                        _signInSchema ??= await SingleRequestAuthenticationInternalAsync(qrContent, cancellationToken);
                        requestBuilder.SetAuthorization(_signInSchema!.TokenType!, _signInSchema.AccessToken);
                    }

                    var httpRequest = requestBuilder.Create();
                    var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
                    if (isRetry == false && response is { IsSuccess: false, StatusCode: 401 })
                    {
                        _signInSchema = null;
                        isRetry = true;
                        continue;
                    }

                    return response;
                } while (true);
            }
        }

        private async UniTask<SignInSchema> SingleRequestAuthenticationInternalAsync(string qrContent, CancellationToken cancellationToken)
        {
            if (_authenticationLazyTask is not null)
            {
                return await _authenticationLazyTask.Task
                    .AttachExternalCancellation(cancellationToken);
            }

            try
            {
                return await AuthenticationInternalAsync(qrContent, cancellationToken)
                    .ToAsyncLazy();
            }
            finally
            {
                _authenticationLazyTask = null;
            }
        }
        
        private async UniTask<SignInSchema> AuthenticationInternalAsync(string qrContent, CancellationToken cancellationToken)
        {
            var apiUri = new Uri(ApiUrl);

            if (string.IsNullOrEmpty(qrContent))
            {
                throw new ArgumentNullException($"Cannot load qr guid {qrContent}");
            }

            var deviceAuthSchema = new DeviceAuthSchema()
            {
                QRToken = qrContent,
                LocaleCode = _localizationService.LocaleCode
            };

            var json = await _serializer.SerializeAsync(deviceAuthSchema, cancellationToken);
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var request = requestBuilder
                    .SetUrl(apiUri)
                    .SetPath(AuthPath)
                    .SetMethod(HttpMethod.Post)
                    .SetJsonContentType()
                    .SetBody(json)
                    .Create();

                var httpResponse = await _httpClient.SendAsync(request, cancellationToken);
                ThrowHttpResponseExceptionIfHasError(httpResponse);

                return await _serializer.DeserializeAsync<SignInSchema>(httpResponse.DataAsText, cancellationToken);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ThrowHttpResponseExceptionIfHasError(HttpResponse response)
        {
            if (response.IsSuccess)
                return;

            throw new HttpResponseException(response.Error, response.StatusCode);
        }
    }
}