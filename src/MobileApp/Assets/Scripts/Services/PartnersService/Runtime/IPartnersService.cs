using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.Utils.Data;
using Sotbify.MobileApp.Utils.Enums;

namespace Sotbify.MobileApp.Services.PartnersService
{
    public interface IPartnersService
    {
        public IReadOnlyDictionary<string, PartnerData> Partners { get; }
        public EInitializingState InitializingState { get; }

        public UniTask InitializeAsync(CancellationToken cancellationToken = default);
    }
}