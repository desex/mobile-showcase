using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.Server.HttpClient.Core.Types;
using Sotbify.MobileApp.Utils.Schemas.Bonus;

namespace Sotbify.MobileApp.Server
{
    public partial class Client
    {
        public async UniTask<IReadOnlyList<BonusSchema>> GetPromocodesAsync(CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var unbildedRequest = requestBuilder
                    .SetMethod(HttpMethod.Get)
                    .SetPath(GetPath("GetPromocodes"));

                var request = unbildedRequest.Create();
                
                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                return await _serializer.DeserializeAsync<BonusSchema[]>(response.DataAsText, cancellationToken);
            }
        }
        
        public async UniTask<Dictionary<string, int>> GetBonusFiltersAsync(CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var unbildedRequest = requestBuilder
                    .SetMethod(HttpMethod.Get)
                    .SetPath(GetPath("BonusItemsFilter"));
                
                var request = unbildedRequest.Create();
                
                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                return await _serializer.DeserializeAsync<Dictionary<string, int>>(response.DataAsText, cancellationToken);
            }
        }
    }   
}