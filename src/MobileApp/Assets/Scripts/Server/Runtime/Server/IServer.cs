using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.Server.HttpClient.Core;
using Sotbify.MobileApp.Utils.Schemas;

namespace Sotbify.MobileApp.Server
{
    public interface IServer
    {
        string AccessToken { get; }

        UniTask<HttpResponse> SendAsync(
            string qrContent,
            HttpRequest request,
            CancellationToken cancellationToken = default,
            bool ignoreDefaultRequestSetup = false
        );
        UniTask<SignInSchema> AuthenticationAsync(string qrContent, CancellationToken cancellationToken = default);
    }
}