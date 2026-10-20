using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sotbify.MobileApp.TutorialWindows.RelicsMoreTutorialInfo
{
    public interface IRelicsMoreTutorialInfo
    {
        public UniTask CloseWindowAsync(CancellationToken cancellationToken = default);
    }
}