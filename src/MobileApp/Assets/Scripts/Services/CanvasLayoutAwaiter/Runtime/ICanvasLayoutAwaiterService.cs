using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sotbify.MobileApp.Services.CanvasLayoutAwaiter
{
    public interface ICanvasLayoutAwaiterService
    {
        public UniTask WaitForNextLayoutAsync(CancellationToken cancellationToken = default);
    }
}