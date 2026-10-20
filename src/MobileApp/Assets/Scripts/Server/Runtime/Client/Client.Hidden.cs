using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Server.HttpClient.Core;
using Sotbify.MobileApp.Server.HttpClient.Core.Types;
using Sotbify.MobileApp.Utils.Schemas.Password;

namespace Sotbify.MobileApp.Server
{
    public partial class Client
    {
        public async UniTask<bool> HasPassword(CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var requestSchema = new HasPasswordRequestSchema()
                {
                    QRToken = SavedQR
                };
                
                var requestBody = await _serializer.SerializeAsync(requestSchema, cancellationToken);
                
                var request = requestBuilder    
                    .SetMethod(HttpMethod.Post)
                    .SetPath(GetPath("HasPassword"))
                    .SetBody(requestBody)
                    .SetJsonContentType()
                    .Create();

                var response = await _server.SendAsync(SavedQR, request, cancellationToken);

                if (response.IsSuccess == false)
                    _logger.Error("The has password request failed", context: this);
                
                return await _serializer.DeserializeAsync<bool>(response.DataAsText,
                    cancellationToken);
            }
        }
        
        public async UniTask SavePassword(string password, CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var requestSchema = new SavePasswordRequestSchema()
                {
                    Password = password
                };
                
                var requestBody = await _serializer.SerializeAsync(requestSchema, cancellationToken);
                
                var request = requestBuilder
                    .SetMethod(HttpMethod.Post)
                    .SetPath(GetPath("SavePassword"))
                    .SetBody(requestBody)
                    .SetJsonContentType()
                    .Create();

                var response = await _server.SendAsync(SavedQR, request, cancellationToken);

                if (response.IsSuccess == false)
                    _logger.Error("The save password request failed", context: this);
            }
        }
        
        public async UniTask DeletePassword(CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var request = requestBuilder    
                    .SetMethod(HttpMethod.Delete)
                    .SetPath(GetPath("DeletePassword"))
                    .Create();

                var response = await _server.SendAsync(SavedQR, request, cancellationToken);

                if (response.IsSuccess == false)
                    _logger.Error("The delete password request failed", context: this);
            }
        }
    }
}