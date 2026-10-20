using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.UI.Window;

namespace Sotbify.MobileApp.TutorialWindows.RelicsMoreTutorialInfo
{
    public class RelicsMoreTutorialInfo : IRelicsMoreTutorialInfo
    {
        private readonly IWindowService _windowService;

        public RelicsMoreTutorialInfo(
            IWindowService windowService
        )
        {
            _windowService = windowService;
        }

        public UniTask CloseWindowAsync(CancellationToken cancellationToken = default)
        {
            return _windowService.SafeCloseAsync<IRelicsMoreTutorialInfo>(cancellationToken);
        }
    }
}