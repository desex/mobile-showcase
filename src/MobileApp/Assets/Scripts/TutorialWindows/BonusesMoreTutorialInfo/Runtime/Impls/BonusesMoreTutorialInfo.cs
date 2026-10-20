using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.UI.Window;

namespace Sotbify.MobileApp.TutorialWindows.BonusesMoreTutorialInfo
{
    public class BonusesMoreTutorialInfo : IBonusesMoreTutorialInfo
    {
        private readonly IWindowService _windowService;

        public BonusesMoreTutorialInfo(
            IWindowService windowService
        )
        {
            _windowService = windowService;
        }

        public UniTask CloseWindowAsync(CancellationToken cancellationToken = default)
        {
            return _windowService.SafeCloseAsync<IBonusesMoreTutorialInfo>(cancellationToken);
        }
    }
}