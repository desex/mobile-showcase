using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sotbify.MobileApp.Server.Serialization
{
    public interface ISerializer
    {
        UniTask<string> SerializeAsync(object obj, CancellationToken cancellationToken = default);
        UniTask<T> DeserializeAsync<T>(string body, CancellationToken cancellationToken = default);
    }
}