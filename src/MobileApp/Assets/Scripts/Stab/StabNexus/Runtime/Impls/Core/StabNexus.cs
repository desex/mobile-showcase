using LeTai.Asset.TranslucentImage;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.DockWindowSwitcher;
using Sotbify.MobileApp.NexusWindow.Core;

namespace Sotbify.MobileApp.Stab.StabNexus.Core
{
    public class StabNexus : Nexus, IStabNexus
    {
        public StabNexus(
            IStabWindowSwitcher windowSwitcher,
            TranslucentImageSource translucentImageSource,
            ILogger logger
        ) : base(windowSwitcher, translucentImageSource, logger)
        {
            
        }
    }
}