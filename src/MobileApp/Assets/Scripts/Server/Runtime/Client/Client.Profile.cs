using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.Server.HttpClient.Core.Types;
using Sotbify.MobileApp.Utils.Schemas.Profile;

namespace Sotbify.MobileApp.Server
{
    public partial class Client
    {
        // TODO: перенести пути в Firebase Remote Config

        public async UniTask<ProfileSchema?> LoadProfile(CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var request = requestBuilder
                    .SetMethod(HttpMethod.Get)
                    .SetPath(GetPath("GetProfile"))
                    .Create();
                
                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                if(response.IsSuccess == false || string.IsNullOrEmpty(response.DataAsText))
                    return null;
                
                return await _serializer.DeserializeAsync<ProfileSchema>(response.DataAsText, cancellationToken);
            }
        }
        
        public async UniTask<IReadOnlyList<ProfileCharacterSchema>> LoadCharacters(CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var request = requestBuilder
                    .SetMethod(HttpMethod.Get)
                    .SetPath("mobile/profile-characters")
                    .Create();
                
                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                if(response.IsSuccess == false || string.IsNullOrEmpty(response.DataAsText))
                    return Array.Empty<ProfileCharacterSchema>();
                
                return await _serializer.DeserializeAsync<ProfileCharacterSchema[]>(response.DataAsText, cancellationToken);
            }
        }

        public async UniTask<bool> ActivateCharacter(int characterId, CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var request = requestBuilder
                    .SetMethod(HttpMethod.Post)
                    .SetPath($"mobile/profile-characters/activate/{characterId}")
                    .Create();

                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                return response.IsSuccess;
            }
        }
    }
}