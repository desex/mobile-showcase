using System.Threading;
using System.Threading.Tasks;

namespace Sotbifay.Builder.Editor
{
    public interface IBuildStep<TContext>
    {
        IBuildStep<TContext> SetNext(IBuildStep<TContext> step);
        Task<TContext> ExecuteAsync(TContext context, CancellationToken cancellationToken = default);
    }
}