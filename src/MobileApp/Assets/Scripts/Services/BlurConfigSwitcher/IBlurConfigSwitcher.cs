using Sotbify.MobileApp.Utils.Enums.Nexus;

namespace Sotbify.MobileApp.Services.BlurConfigSwitcher
{
    public interface IBlurConfigSwitcher
    {
        public void Switch(ENexusCategory nexusCategory);
    }
}