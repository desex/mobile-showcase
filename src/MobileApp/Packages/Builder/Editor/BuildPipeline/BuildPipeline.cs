using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sotbifay.Builder.Editor
{
    public class BuildPipeline<TContext> : IBuildPipeline<TContext>
    {
        private readonly TContext _context;

        private IBuildStep<TContext> _firstBuildStep;
        private IBuildStep<TContext> _lastBuildStep;

        public BuildPipeline(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public static IBuildPipeline<TContext> Create(TContext context) =>
            new BuildPipeline<TContext>(context);

        public IBuildPipeline<TContext> Append(IBuildStep<TContext> buildStep)
        {
            if (buildStep is null)
                return this;

            _firstBuildStep ??= buildStep;

            _lastBuildStep = _lastBuildStep is not null
                ? _lastBuildStep.SetNext(buildStep) 
                : buildStep;

            return this;
        }

        public Task<TContext> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return _firstBuildStep is null
                ? Task.FromResult(_context)
                : _firstBuildStep.ExecuteAsync(_context, cancellationToken);
        }
    }
}