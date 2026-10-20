using System;
using R3;
using Sotbify.MobileApp.Stab.StabNexus;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Enums.Nexus;

namespace Stab.WarrantyWindow
{
    public class Warranty : IWarranty, IDisposable
    {
        private readonly Subject<Unit> _close = new();
        private readonly IStabNexus _stabNexus;

        public Warranty(IStabNexus stabNexus)
        {
            _stabNexus = stabNexus;
        }

        public Observable<Unit> CloseObservable => _close;

        public void GoChat()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Close()
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