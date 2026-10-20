using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sotbify.MobileApp.TutorialWindows.BonusesMoreTutorialInfo
{
    public interface IBonusesMoreTutorialInfo
    {
        public UniTask CloseWindowAsync(CancellationToken cancellationToken = default);
    }
}