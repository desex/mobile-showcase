using R3;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Enums.Blackjack;

namespace Sotbify.MobileApp.BlackjackWindow
{
    public interface IBlackjackRoundStateController
    {
        ReadOnlyReactiveProperty<EBlackjackAnimationState> State { get; }
        EBlackjackAnimationState CurrentState { get; }

        public void Reset();
        public void PlayerAnimationsStarted();
        public void PlayerAnimationsEnded();
        public void DealerAnimationsStarted();
        public void DealerAnimationsEnded();
        public void RoundEndAnimationsStarted();
        public void RoundEndAnimationsEnded();
        
    }
}