using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.Server.HttpClient.Core.Types;
using Sotbify.MobileApp.Utils.Schemas;

namespace Sotbify.MobileApp.Server
{
    public partial class Client
    {
        public async UniTask<PartnersSchema[]> GetPartners(CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var request = requestBuilder
                    .SetMethod(HttpMethod.Get)
                    .SetPath(GetPath("GetPartners"))
                    .Create();
                
                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                return await _serializer.DeserializeAsync<PartnersSchema[]>(response.DataAsText, cancellationToken);
            }
        }
    }
}