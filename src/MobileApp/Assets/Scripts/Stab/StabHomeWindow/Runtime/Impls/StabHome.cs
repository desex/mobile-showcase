using System;
using R3;
using Sotbify.MobileApp.Stab.StabNexus;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Enums.Nexus;

namespace Stab.StabHomeWindow
{
    public class StabHome : IStabHome, IDisposable
    {
        private readonly IStabNexus _stabNexus;
        private readonly Subject<EHomeItem> _changeScreen = new ();

        public StabHome(IStabNexus stabNexus)
        {
            _stabNexus = stabNexus;
        }

        public Observable<EHomeItem> ChangeScreen => _changeScreen;

        public void Open(EHomeItem eItem)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public void Dispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}