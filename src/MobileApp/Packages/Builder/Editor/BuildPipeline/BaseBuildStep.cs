using System.Threading;
using System.Threading.Tasks;

namespace Sotbifay.Builder.Editor
{
    public abstract class BaseBuildStep<TContext> : IBuildStep<TContext>
    {
        private IBuildStep<TContext> _next;

        public IBuildStep<TContext> SetNext(IBuildStep<TContext> step)
        {
            _next = step;
            return _next;
        }

        public async Task<TContext> ExecuteAsync(TContext context, CancellationToken cancellationToken = default)
        {
            var executeResult = await ExecuteInternalAsync(context, cancellationToken);
            return _next is not null && executeResult
                ? await _next.ExecuteAsync(context, cancellationToken)
                : context;
        }

        protected abstract Task<bool> ExecuteInternalAsync(TContext context, CancellationToken cancellationToken);
    }
}