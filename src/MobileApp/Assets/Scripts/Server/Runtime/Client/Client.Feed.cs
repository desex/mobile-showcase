using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.Server.HttpClient.Core.Types;
using Sotbify.MobileApp.Utils.Schemas.Feed;

namespace Sotbify.MobileApp.Server
{
    public partial class Client
    {
        public async UniTask<IReadOnlyList<FeedItemSchema>> GetHomeItemsAsync(IEnumerable<int> partnerIds, int limit = 5, int offset = 0, CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                requestBuilder
                    .SetMethod(HttpMethod.Get)
                    .SetPath(GetPath("FeedHistory"))
                    .SetParameter("limit", limit.ToString())
                    .SetParameter("offset", offset.ToString());

                foreach (var id in partnerIds)
                    requestBuilder.AddParameter("partner_ids", id.ToString());

                var request = requestBuilder.Create();
                
                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                return await _serializer.DeserializeAsync<FeedItemSchema[]>(response.DataAsText, cancellationToken);
            }
        }

        public async UniTask<Dictionary<string, int>> GetHomeFiltersAsync(CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var request = requestBuilder
                    .SetMethod(HttpMethod.Get)
                    .SetPath(GetPath("HomeItemsFilter"))
                    .Create();
                
                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                return await _serializer.DeserializeAsync<Dictionary<string, int>>(response.DataAsText, cancellationToken);
            }
        }
    }
}