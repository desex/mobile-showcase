using System.Threading;
using Cysharp.Threading.Tasks;

namespace UState
{
    public interface IExecutableState
    {
        UniTask ExecuteAsync(CancellationToken cancellationToken = default);
    }
}