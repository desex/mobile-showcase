using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Sotbify.MobileApp.Server.HttpClient.Core;
using Sotbify.MobileApp.Server.HttpClient.Core.Types;
using Sotbify.MobileApp.Server.SSE;
using Sotbify.MobileApp.Utils.Schemas.Chat;

namespace Sotbify.MobileApp.Server
{
    public partial class Client
    {
        public async UniTask<ChatMessagePaginationSchema> GetChatHistoryAsync(
            int? cursor,
            int limit = 10,
            CancellationToken cancellationToken = default
        )
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                requestBuilder
                    .SetMethod(HttpMethod.Get)
                    .SetParameter("limit", limit.ToString())
                    .SetPath(GetPath("GetChatHistory"));

                if (cursor.HasValue)
                    requestBuilder.SetParameter("cursor", cursor.Value.ToString());

                var request = requestBuilder.Create();
                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                return await _serializer.DeserializeAsync<ChatMessagePaginationSchema>(response.DataAsText,
                    cancellationToken);
            }
        }
        
        public async UniTask<ChatMessageResponseSchema> SendChatMessageAsync(string message,
            CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var requestSchema = new ChatMessageRequestSchema()
                {
                    Message = message
                };

                var requestBody = await _serializer.SerializeAsync(requestSchema, cancellationToken);

                requestBuilder
                    .SetMethod(HttpMethod.Post)
                    .SetPath(GetPath("SendMessage"))
                    .SetBody(requestBody)
                    .SetJsonContentType();

                var request = requestBuilder.Create();
                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                return await _serializer.DeserializeAsync<ChatMessageResponseSchema>(response.DataAsText,
                    cancellationToken);
            }
        }
        
        public async UniTask ReadMessageAsync(int messageId, CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var requestSchema = new ChatMarkReadSchema()
                {
                    LastMessageId = messageId
                };

                var requestBody = await _serializer.SerializeAsync(requestSchema, cancellationToken);

                requestBuilder
                    .SetMethod(HttpMethod.Post)
                    .SetPath(GetPath("ReadMessages"))
                    .SetBody(requestBody)
                    .SetJsonContentType();

                var request = requestBuilder.Create();
                await _server.SendAsync(SavedQR, request, cancellationToken);
            }
        }
        
        public async UniTask<int> GetUnreadCountAsync(CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                requestBuilder
                    .SetMethod(HttpMethod.Get)
                    .SetPath(GetPath("GetUnreadCount"))
                    .SetJsonContentType();

                var request = requestBuilder.Create();
                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                return JsonConvert.DeserializeObject<UnreadCountSchema>(response.DataAsText).Count;
            }
        }

        public async UniTask<ManagerSchema> GetManagerInfo(CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                requestBuilder
                    .SetMethod(HttpMethod.Get)
                    .SetPath(GetPath("GetManagerInfo"))
                    .SetJsonContentType();

                var request = requestBuilder.Create();
                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                return JsonConvert.DeserializeObject<ManagerSchema>(response.DataAsText);
            }
        }
        
        public ISSEFacade CreateChatSse()
        {
            var uri = new UriBuilder(ApiUrl)
                .WithPathSegment(GetPath("ChatSSE"))
                .Uri;

            return new SSEFacade(uri, this, _logger);
        }
    }   
}