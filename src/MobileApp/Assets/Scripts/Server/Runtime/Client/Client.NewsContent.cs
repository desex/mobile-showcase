using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Server.HttpClient.Core;
using Sotbify.MobileApp.Server.HttpClient.Core.Types;
using Sotbify.MobileApp.Utils.Schemas.Feed;

namespace Sotbify.MobileApp.Server
{
    public partial class Client
    {
        public async UniTask<FeedContentSchema> GetNewsContent(int targetContent, CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var request = requestBuilder
                    .SetMethod(HttpMethod.Get)
                    .SetPath(GetPath("NewsContent") + targetContent)
                    .Create();

                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                var result = await _serializer.DeserializeAsync<FeedContentSchema>(response.DataAsText, cancellationToken);
                return result;
            }
        }

        public async UniTask MarkNewsReadAsync(int newsId, CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var request = requestBuilder
                    .SetMethod(HttpMethod.Post)
                    .SetPath(GetPath("NewsContent") + newsId + "/read")
                    .SetJsonContentType()
                    .Create();

                var isCompleted = await _server.SendAsync(SavedQR, request, cancellationToken);

                if (_logger.IsEnabled(LogLevel.Information) && isCompleted.IsSuccess)
                    _logger.Information($"News {newsId} marked as read", context: this);
            }
        }
    }
}