using VContainer;
using VContainer.Unity;

namespace Sotbify.MobileApp.Localization
{
    public sealed class LocalizationProjectInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<LocalizationService>(Lifetime.Singleton)
                .As<ILocalizationService>();
            
            builder.Register<StringLocalizer>(Lifetime.Singleton)
                .As<IStringLocalizer>();
        }
    }
}