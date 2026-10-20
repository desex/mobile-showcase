using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.Server.HttpClient.Core;
using Sotbify.MobileApp.Server.HttpClient.Core.Types;
using Sotbify.MobileApp.Utils.Schemas.Relics;

namespace Sotbify.MobileApp.Server
{
    public partial class Client
    {
        public async UniTask<RelicDescriptionSchema?> GetDailyRelicAsync(CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var request = requestBuilder
                    .SetMethod(HttpMethod.Post)

                    // TODO: нужно вынести в remote config?
// #if UNITY_EDITOR || DEMO
//                     .SetPath("mobile/artifacts/daily/demo")
// #else
                    .SetPath("mobile/artifacts/daily")
// #endif
                    .Create();

                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                if (response.IsSuccess == false)
                    return null;

                if (string.IsNullOrEmpty(response.DataAsText) || response.DataAsText == "null")
                    return null;

                return await _serializer.DeserializeAsync<RelicDescriptionSchema>(response.DataAsText, cancellationToken);
            }
        }

        public async UniTask<RelicsSchema> LoadRelicsAsync(CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var request = requestBuilder
                    .SetMethod(HttpMethod.Get)
                    .SetPath(GetPath("GetRelics"))
                    .Create();

                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                return await _serializer.DeserializeAsync<RelicsSchema>(response.DataAsText, cancellationToken);
            }
        }

        public async UniTask<RelicDescriptionSchema> GetRelicDescriptionAsync
        (
            int relicId,
            CancellationToken cancellationToken = default
        )
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var relicsDescriptionUrl = GetPath("GetRelicDescription");
                var request = requestBuilder
                    .SetMethod(HttpMethod.Get)
                    .SetPath(relicsDescriptionUrl + relicId)
                    .Create();

                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                return await _serializer.DeserializeAsync<RelicDescriptionSchema>(response.DataAsText, cancellationToken);
            }
        }

        public async UniTask<RelicSchema[]> LoadActiveSetAsync
        (
            CancellationToken cancellationToken = default
        )
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var request = requestBuilder
                    .SetMethod(HttpMethod.Get)
                    .SetPath(GetPath("ActiveSet"))
                    .Create();

                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                return await _serializer.DeserializeAsync<RelicSchema[]>(response.DataAsText, cancellationToken);
            }
        }

        public async UniTask<IReadOnlyList<RelicSchema>> RandomizeActiveSetAsync
        (
            CancellationToken cancellationToken = default
        )
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var request = requestBuilder
                    .SetMethod(HttpMethod.Post)
                    .SetPath(GetPath("RandomizeActive"))
                    .Create();

                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                return await _serializer.DeserializeAsync<RelicSchema[]>(response.DataAsText, cancellationToken);
            }
        }

        public async UniTask<RelicDescriptionSchema> EquipRelicAsync
        (
            int relicId,
            int position,
            CancellationToken cancellationToken = default
        )
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var requestSchema = new EquipRelicRequest()
                {
                    ArtifactId = relicId,
                    Position = position,
                };

                var requestBody = await _serializer.SerializeAsync(requestSchema, cancellationToken);

                var request = requestBuilder
                    .SetMethod(HttpMethod.Patch)
                    .SetPath(GetPath("ActivateRelic"))
                    .SetBody(requestBody)
                    .SetJsonContentType()
                    .Create();

                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                return await _serializer.DeserializeAsync<RelicDescriptionSchema>(response.DataAsText, cancellationToken);
            }
        }
    }
}