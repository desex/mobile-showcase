using System.Threading;
using Cysharp.Threading.Tasks;

namespace UState
{
    public interface IExitableState
    {
        UniTask ExitAsync(CancellationToken cancellationToken = default);
    }
}