using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sotbify.MobileApp.Services.Browser
{
    public interface IBrowser
    {
        public UniTask OpenLink(string url, CancellationToken cancellationToken = default);
    }
}