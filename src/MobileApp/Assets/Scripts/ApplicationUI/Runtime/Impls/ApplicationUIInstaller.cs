using Sotbify.MobileApp.ApplicationUI.States;
using Sotbify.MobileApp.ApplicationUI.States.Stab;
using UState;
using UState.VContainer;
using VContainer;
using VContainer.Unity;

namespace Sotbify.MobileApp.ApplicationUI
{
    public class ApplicationUIInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder
                .RegisterEntryPoint<ApplicationUI>()
                .WithParameter<IStateResolver>(resolver => new VContainerStateResolver(resolver))
                .As<IApplicationUI>();

            builder.RegisterEntryPoint<ApplicationUIAnalyticsHandler>();

            builder.Register<MainPolicyState>(Lifetime.Singleton);
            
            builder.Register<GreetingsWindowState>(Lifetime.Singleton);
            
            builder.Register<ScanWindowState>(Lifetime.Singleton);
            
            builder.Register<LoadingScreenState>(Lifetime.Singleton);
            builder.Register<CheckPasswordWindowState>(Lifetime.Singleton);
            builder.Register<DailyRelicsState>(Lifetime.Singleton);

            builder.Register<FeedWindowState>(Lifetime.Singleton);
            builder.Register<HomeWindowState>(Lifetime.Singleton);
            builder.Register<NotificationCenterState>(Lifetime.Singleton);
            builder.Register<ProfileState>(Lifetime.Singleton);
            builder.Register<HiddenModeWindowState>(Lifetime.Singleton);
            builder.Register<PinCreationWindowState>(Lifetime.Singleton);
            builder.Register<TwoFactorWindowState>(Lifetime.Singleton);
            builder.Register<ProfileCharacterChooseState>(Lifetime.Singleton);
            builder.Register<CollectionWindowState>(Lifetime.Singleton);
            builder.Register<ActiveSetState>(Lifetime.Singleton);
            builder.Register<RelicDescriptionWindowState>(Lifetime.Singleton);
            builder.Register<FAQWindowState>(Lifetime.Singleton);
            builder.Register<FAQCategoryState>(Lifetime.Singleton);
            builder.Register<NewsWindowState>(Lifetime.Singleton);

            builder.Register<BonusesWindowState>(Lifetime.Singleton);

            builder.Register<ChatWindowState>(Lifetime.Singleton);
            builder.Register<BlackjackWindowState>(Lifetime.Singleton);
            builder.Register<CloseBlackjackPopupState>(Lifetime.Singleton);

            builder.Register<GameWindowState>(Lifetime.Singleton);
            
            //Stab
            builder.Register<StabMainInitializeState>(Lifetime.Singleton);
            builder.Register<GuideContentWindowState>(Lifetime.Singleton);
            builder.Register<AchievementsWindowState>(Lifetime.Singleton);
            builder.Register<StabChatWindowState>(Lifetime.Singleton);
            builder.Register<SessionsWindowState>(Lifetime.Singleton);
            builder.Register<SettingsWindowState>(Lifetime.Singleton);
            builder.Register<StabHomeState>(Lifetime.Singleton);
            builder.Register<DeleteAccountState>(Lifetime.Singleton);
            builder.Register<StabProfileWindowState>(Lifetime.Singleton);
            builder.Register<VRGuideWindowState>(Lifetime.Singleton);
            builder.Register<WarrantyWindowState>(Lifetime.Singleton);
        }
    }
}

