using System;
using Best.HTTP.Request.Authenticators;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Common.Preferences;
using Sotbify.MobileApp.Server.HttpClient.Core;
using Sotbify.MobileApp.Server.Serialization;
using Sotbify.MobileApp.Services.RemoteConfig;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Schemas;
using UnityEngine.Pool;

namespace Sotbify.MobileApp.Server
{
    public partial class Client : IAuthenticatorProvider
    {
        private readonly IServer _server;
        private readonly ILogger _logger;
        private readonly IObjectPool<HttpRequestBuilder> _httpRequestBuildersPool;
        private readonly IPreferences _preferences;
        private readonly ISerializer _serializer;
        private readonly IRemoteConfigService _remoteConfigService;
        
        private string ApiUrl =>
#if DEBUG
            _remoteConfigService.GetString("DevAPI");
#else
            _remoteConfigService.GetString("ProdAPI");
#endif

        private string SavedQR => _preferences.GetString(SavePath.QRSavePath);
        public SignInSchema SignInSchema { get; private set; }
        private Uri RiobetUri => new (_remoteConfigService.GetString("RiobetAPI"));

        public Client
        (
            IServer server,
            ILogger logger,
            IObjectPool<HttpRequestBuilder> httpRequestBuildersPool,
            IPreferences preferences,
            ISerializer serializer,
            IRemoteConfigService remoteConfigService
        )
        {
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpRequestBuildersPool = httpRequestBuildersPool ?? throw new ArgumentNullException(nameof(httpRequestBuildersPool));
            _preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _remoteConfigService = remoteConfigService;
        }

        public IAuthenticator GetAuthenticator()
            => new BearerTokenAuthenticator(_server.AccessToken);
        
        private string GetPath(string key)
            => _remoteConfigService.GetString(key);
    }
}