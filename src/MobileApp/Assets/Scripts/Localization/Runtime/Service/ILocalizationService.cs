using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sotbify.MobileApp.Localization
{
    public interface ILocalizationService
    {
        string LocaleCode { get; set; }
        
        UniTask InitializeAsync(CancellationToken cancellationToken = default);
    }
}