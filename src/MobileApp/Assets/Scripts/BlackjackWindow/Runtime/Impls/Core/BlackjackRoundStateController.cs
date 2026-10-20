using R3;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Enums.Blackjack;

namespace Sotbify.MobileApp.BlackjackWindow.Core
{
    public class BlackjackRoundStateController : IBlackjackRoundStateController
    {
        private readonly ReactiveProperty<EBlackjackAnimationState> _state = new(EBlackjackAnimationState.None);

        public ReadOnlyReactiveProperty<EBlackjackAnimationState> State => _state;
        public EBlackjackAnimationState CurrentState => _state.CurrentValue;

        public void Reset()
           {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void PlayerAnimationsStarted()
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void PlayerAnimationsEnded()
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void DealerAnimationsStarted()
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void DealerAnimationsEnded()
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void RoundEndAnimationsStarted()
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void RoundEndAnimationsEnded()
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}