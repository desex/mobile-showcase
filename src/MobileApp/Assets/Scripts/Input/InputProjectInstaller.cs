using VContainer;
using VContainer.Unity;

namespace Sotbify.MobileApp.Input
{
    public class InputProjectInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            RegisterInputActions(builder);
            RegisterAccelerometerSensor(builder);
        }

        private static void RegisterInputActions(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<InputSystemActions>()
                .AsSelf();
            
            builder.RegisterBuildCallback(resolver =>
            {
                var actions = resolver.Resolve<InputSystemActions>();
                actions.Enable();
            });
            
            builder.RegisterDisposeCallback(resolver =>
            {
                var actions = resolver.Resolve<InputSystemActions>();
                actions.Disable();
            });
        }

        private static void RegisterAccelerometerSensor(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<AccelerometerSensor>()
                .AsSelf();
            
            builder.RegisterBuildCallback(resolver =>
            {
                var accelerometerSensor = resolver.Resolve<AccelerometerSensor>();
                accelerometerSensor.Enable();
            });
        }
    }
}