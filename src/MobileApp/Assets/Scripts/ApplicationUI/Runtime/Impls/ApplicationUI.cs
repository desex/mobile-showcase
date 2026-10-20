using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using UState;
using VContainer.Unity;

namespace Sotbify.MobileApp.ApplicationUI
{
    public class ApplicationUI : StateMachine<IApplicationUI>, IApplicationUI, IAsyncStartable
    {
        private readonly ILogger _logger;

        public ApplicationUI(
            ILogger logger,
            IStateResolver stateResolver
        ) : base(stateResolver)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async UniTask StartAsync(CancellationToken cancellation)
        {
            while (cancellation.IsCancellationRequested == false)
            {
                try
                {
                    await ExecuteAsync(cancellation);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    _logger.Error("Error during FSM execution", e, this);
                }
            }
        }
    }   
}