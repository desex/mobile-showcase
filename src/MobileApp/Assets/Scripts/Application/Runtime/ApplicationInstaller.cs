using Sotbify.MobileApp.Application.States;
using UState;
using UState.VContainer;
using VContainer;
using VContainer.Unity;

namespace Sotbify.MobileApp.Application
{
    public sealed class ApplicationInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder
                .RegisterEntryPoint<Application>()
                .WithParameter<IStateResolver>(resolver => new VContainerStateResolver(resolver))
                .As<IApplication>();

            builder.Register<InitializationState>(Lifetime.Singleton);
            builder.Register<AuthenticationState>(Lifetime.Singleton);
        }
    }
}

