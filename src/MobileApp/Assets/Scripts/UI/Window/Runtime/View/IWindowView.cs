using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sotbify.MobileApp.UI.Window.View
{
    public interface IWindowView
    {
        UniTask ShowAsync(CancellationToken cancellationToken = default);
        UniTask HideAsync(CancellationToken cancellationToken = default);
    }
}