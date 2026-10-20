using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Application.States;
using UState;
using VContainer.Unity;

namespace Sotbify.MobileApp.Application
{
    public class Application : StateMachine<IApplication>, IApplication, IInitializable, IAsyncStartable
    {
        private readonly ILogger _logger;

        public Application(
            IStateResolver stateResolver,
            ILogger logger
        ) : base(stateResolver)
        {
            _logger = logger;
        }

        public void Initialize()
        {
            RequestTransition<InitializationState>();
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