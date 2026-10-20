using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Server.HttpClient.Core;
using Sotbify.MobileApp.Server.HttpClient.Core.Exceptions;
using Sotbify.MobileApp.Server.HttpClient.Core.Types;
using Sotbify.MobileApp.Utils.Schemas;
using Sotbify.MobileApp.Utils.Schemas.Firebase;

namespace Sotbify.MobileApp.Server
{
    public partial class Client
    {
        public async UniTask<bool> TryAuthenticate(string qrContent, CancellationToken token = default)
        {
            try
            {
                SignInSchema = await _server.AuthenticationAsync(qrContent, token);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public async UniTask<bool> TryAuthenticateWithHttpException(string qrContent, CancellationToken token = default)
        {
            try
            {
                SignInSchema = await _server.AuthenticationAsync(qrContent, token);
                return true;
            }
            catch (HttpResponseException)
            {
                throw;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async UniTask RefreshAuthenticationAsync(CancellationToken cancellationToken = default)
        {
            SignInSchema = await _server.AuthenticationAsync(SavedQR, cancellationToken);
        }
        
        public async UniTask UpdateToken(string token, CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var requestSchema = new UpdateTokenRequestSchema
                {
                    Token = token
                };

                var requestBody = await _serializer.SerializeAsync(requestSchema, cancellationToken);
           
                var request = requestBuilder
                    .SetMethod(HttpMethod.Put)
                    .SetPath(GetPath("UpdateToken"))
                    .SetBody(requestBody)
                    .SetJsonContentType()
                    .Create();
                
                await _server.SendAsync(SavedQR, request, cancellationToken);
                
                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.Information("Sended token");
            }
        }

        public async UniTask<bool> TryAuthHelmet(CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var request = requestBuilder
                    .SetMethod(HttpMethod.Get)
                    .SetPath(GetPath("DeviceInfo"))
                    .Create();

                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                var schema = await _serializer.DeserializeAsync<DeviceInfoSchema>(response.DataAsText,
                    cancellationToken);
                return schema.IsActivated;
            }
        }
    }
}