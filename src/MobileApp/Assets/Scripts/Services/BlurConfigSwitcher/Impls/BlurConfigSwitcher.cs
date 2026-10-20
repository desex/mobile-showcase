using LeTai.Asset.TranslucentImage;
using Sotbify.MobileApp.Db.BlurConfig;
using Sotbify.MobileApp.Utils.Enums.Nexus;

namespace Sotbify.MobileApp.Services.BlurConfigSwitcher
{
    public class BlurConfigSwitcher : IBlurConfigSwitcher
    {
        private readonly IBlurConfiig _blurConfig;
        private readonly TranslucentImageSource _translucentImageSource;

        public BlurConfigSwitcher(
            IBlurConfiig blurConfig,
            TranslucentImageSource translucentImageSource
        )
        {
            _blurConfig = blurConfig;
            _translucentImageSource = translucentImageSource;
        }

        public void Switch(ENexusCategory nexusCategory)
        {
            _translucentImageSource.BlurConfig = _blurConfig.BlurConfigs[nexusCategory];
        }
    }
}