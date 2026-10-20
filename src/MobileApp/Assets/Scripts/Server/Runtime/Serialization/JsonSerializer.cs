using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sotbify.MobileApp.Server.Serialization
{
    public sealed class JsonSerializer : ISerializer
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new()
        {
            Converters = new JsonConverter[]
            {
                new StringEnumConverter()
            }
        };
        
        public UniTask<string> SerializeAsync(object obj, CancellationToken cancellationToken = default)
        {
            if(obj is null)
                return UniTask.FromResult(string.Empty);
            
            var state = new Tuple<object, JsonSerializerSettings>(obj, _jsonSerializerSettings);
            return UniTask.RunOnThreadPool(innerState =>
            {
                var (o, serializerSettings) = (Tuple<object, JsonSerializerSettings>)innerState;
                return JsonConvert.SerializeObject(o, settings: serializerSettings);
            }, state, cancellationToken: cancellationToken);
        }
        public UniTask<T> DeserializeAsync<T>(string body, CancellationToken cancellationToken = default)
        {
            if(string.IsNullOrEmpty(body))
                return UniTask.FromResult<T>(default!);
            
            var state = new Tuple<string, JsonSerializerSettings>(body, _jsonSerializerSettings);
            
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return UniTask.RunOnThreadPool(innerState =>
            {
                var (innerJson, serializerSettings) = (Tuple<string, JsonSerializerSettings>)innerState;
                return JsonConvert.DeserializeObject<T>(innerJson, serializerSettings);
            }, state, cancellationToken: cancellationToken);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }
    }
}