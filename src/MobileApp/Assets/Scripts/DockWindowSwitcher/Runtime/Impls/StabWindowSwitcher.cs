using System;
using Sotbify.MobileApp.ApplicationUI;
using Sotbify.MobileApp.ApplicationUI.States.Stab;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Enums.Nexus;

namespace Sotbify.MobileApp.DockWindowSwitcher
{
    public class StabWindowSwitcher : IStabWindowSwitcher
    {
        private readonly IApplicationUI _applicationUI;

        public StabWindowSwitcher(IApplicationUI applicationUI)
        {
            _applicationUI = applicationUI;
        }
        
        public void Switch(ENexusCategory category)
        {
            switch (category)
            {
                case ENexusCategory.Home:
                    _applicationUI.RequestTransition<StabHomeState>(true);
                    return;
                
                case ENexusCategory.Sessions:
                    _applicationUI.RequestTransition<SessionsWindowState>(true);
                    return;
                
                case ENexusCategory.Chat:
                    _applicationUI.RequestTransition<StabChatWindowState>(true);
                    return;
                
                case ENexusCategory.Profile:
                    _applicationUI.RequestTransition<StabProfileWindowState>(true);
                    return;
                
                default:
                    throw new ArgumentException($"Unknown EDockCategory: {category}");   
            }
        }
    }
}