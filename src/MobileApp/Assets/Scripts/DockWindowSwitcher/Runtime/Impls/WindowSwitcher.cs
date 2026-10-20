using System;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.ApplicationUI;
using Sotbify.MobileApp.ApplicationUI.States;
using Sotbify.MobileApp.Services.BlurConfigSwitcher;
using Sotbify.MobileApp.Utils.Enums.Nexus;

namespace Sotbify.MobileApp.DockWindowSwitcher
{
    public class WindowSwitcher : IWindowSwitcher
    {
        private readonly IApplicationUI _applicationUI;
        private readonly IBlurConfigSwitcher _blurConfigSwitcher;
        private readonly ILogger _logger;

        public WindowSwitcher
        (
            IApplicationUI applicationUI,
            IBlurConfigSwitcher blurConfigSwitcher,
            ILogger logger
        )
        {
            _applicationUI = applicationUI ?? throw new ArgumentNullException(nameof(applicationUI));
            _blurConfigSwitcher = blurConfigSwitcher ?? throw new ArgumentNullException(nameof(blurConfigSwitcher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Switch(ENexusCategory category)
        {
            if(_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Switching: {category}", context: this);
            
            switch (category)
            {
                case ENexusCategory.Home:
                _blurConfigSwitcher.Switch(category);
                    _applicationUI.RequestTransition<HomeWindowState>(true);
                    return;
                
                case ENexusCategory.Collections:
                    _blurConfigSwitcher.Switch(category);
                    _applicationUI.RequestTransition<CollectionWindowState>(true);
                    return;
                
                case ENexusCategory.ActiveSet:
                    _blurConfigSwitcher.Switch(category);
                    _applicationUI.RequestTransition<ActiveSetState>(true);
                    return;
                
                case ENexusCategory.Feed:
                    _blurConfigSwitcher.Switch(category);
                    _applicationUI.RequestTransition<FeedWindowState>(true);
                    return;
                
                case ENexusCategory.Chat:
                    _blurConfigSwitcher.Switch(category);
                    _applicationUI.RequestTransition<ChatWindowState, ChatWindowState.Payload>(new ChatWindowState.Payload
                    {
                        IsFromOverlay = false,
                    }, true);
                    return;
                
                case ENexusCategory.Bonuses:
                    _applicationUI.RequestTransition<BonusesWindowState>(true);
                    return;
                
                case ENexusCategory.Profile:
                    _applicationUI.RequestTransition<ProfileState>(true);
                    return;
                
                case ENexusCategory.NotificationCenter:
                    _applicationUI.RequestTransition<NotificationCenterState>(true);
                    return;
                
                default:
                    throw new ArgumentException($"Unknown EDockCategory: {category}");   
            }
        }
    }
}