using System.Threading;
using System.Threading.Tasks;

namespace Sotbifay.Builder.Editor
{
    public interface IBuildPipeline<TContext>
    {
        IBuildPipeline<TContext> Append(IBuildStep<TContext> buildStep);
        Task<TContext> ExecuteAsync(CancellationToken cancellationToken = default);
    }
}