using VContainer;
using VContainer.Unity;

namespace Sotbify.MobileApp.Common.Preferences
{
    public sealed class PreferencesInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder
                .RegisterEntryPoint<UnityPlayerPreferences>()
                .As<IPreferences>();
        }
    }
}