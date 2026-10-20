using Cysharp.Threading.Tasks;

namespace Sotbify.MobileApp.BlackjackWindow
{
    public interface IBlackjackGameService
    {
        UniTask<string> RequestStartRoundJsonAsync();
        UniTask<string> RequestPlayerHitJsonAsync();
        UniTask<string> RequestDealerRevealHiddenJsonAsync();
        UniTask<string> RequestDealerHitJsonAsync();
    }
}