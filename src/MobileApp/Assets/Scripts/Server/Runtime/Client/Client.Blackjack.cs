using System;
using System.Threading;
using Best.HTTP;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Server.HttpClient.Core;
using Sotbify.MobileApp.Server.HttpClient.Core.Types;
using Sotbify.MobileApp.Utils.Schemas.Blackjack;

namespace Sotbify.MobileApp.Server
{
    public partial class Client
    {
        public async UniTask<BlackjackResponseSchema> StartRound(CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var request = requestBuilder
                    .SetMethod(HttpMethod.Post)
                    .SetPath(GetPath("StartGame"))
                    .Create();

                var response = await _server.SendAsync(SavedQR, request, cancellationToken);

                if (response.StatusCode == 404)
                    return null;

                return await _serializer.DeserializeAsync<BlackjackResponseSchema>(response.DataAsText, cancellationToken);
            }
        }
        
        public async UniTask<BlackjackResponseSchema> Hit(CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var request = requestBuilder
                    .SetMethod(HttpMethod.Post)
                    .SetPath(GetPath("Hit"))
                    .Create();

                var response = await _server.SendAsync(SavedQR, request, cancellationToken);

                if (response.StatusCode == 404)
                    return null;

                return await _serializer.DeserializeAsync<BlackjackResponseSchema>(response.DataAsText, cancellationToken);
            }
        }
        
        public async UniTask<BlackjackResponseSchema> Stand(CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var request = requestBuilder
                    .SetMethod(HttpMethod.Post)
                    .SetPath(GetPath("Stand"))
                    .Create();

                var response = await _server.SendAsync(SavedQR, request, cancellationToken);

                if (response.StatusCode == 404)
                    return null;

                return await _serializer.DeserializeAsync<BlackjackResponseSchema>(response.DataAsText, cancellationToken);
            }
        }
        
        public async UniTask<HttpResponse> CancelGame(CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var request = requestBuilder
                    .SetMethod(HttpMethod.Post)
                    .SetPath(GetPath("EndGame"))
                    .Create();
                
                var response = await _server.SendAsync(SavedQR, request, cancellationToken);

                if (response.StatusCode == 404)
                    return response;

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    if (response.IsSuccess)
                        _logger.Information("Ended round successfully", context: this);
                    else
                        _logger.Error("Failed to finish round", new Exception(response.Error), this);
                }

                return response;
            }
        }

        public async UniTask<BlackjackRequestSchema> GetPokerRequest(int requestId, CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var request = requestBuilder
                    .SetMethod(HttpMethod.Get)
                    .SetPath(GetPath("GetBlackjackRequest") + requestId)
                    .Create();
                
                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                return await _serializer.DeserializeAsync<BlackjackRequestSchema>(response.DataAsText, cancellationToken);
            }
        }
    }
}