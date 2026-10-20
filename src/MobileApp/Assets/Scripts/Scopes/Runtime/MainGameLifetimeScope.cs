using System.Collections.Generic;
using LeTai.Asset.TranslucentImage;
using MainPolicyWindow;
using RenderHeads.Media.AVProVideo;
using Sotbify.MobileApp.AllRelicsWindow;
using Sotbify.MobileApp.ApplicationUI;
using Sotbify.MobileApp.BlackjackWindow;
using Sotbify.MobileApp.BlurredBackgroundWindow;
using Sotbify.MobileApp.BonusesWindow;
using Sotbify.MobileApp.Chat;
using Sotbify.MobileApp.ChatMessages;
using Sotbify.MobileApp.CheckPasswordWindow;
using Sotbify.MobileApp.ChooseCharacterWindow;
using Sotbify.MobileApp.CloseBlackjackPopup;
using Sotbify.MobileApp.DailyRelicWindow;
using Sotbify.MobileApp.DeletePopupWindow;
using Sotbify.MobileApp.DockWindowSwitcher;
using Sotbify.MobileApp.FAQWindow;
using Sotbify.MobileApp.FeedWindow.FeedList;
using Sotbify.MobileApp.FeedWindow.FeedListOverlay;
using Sotbify.MobileApp.GlassesWindow;
using Sotbify.MobileApp.GreetingsWindow;
using Sotbify.MobileApp.HomeWindow;
using Sotbify.MobileApp.HiddenModeWindow;
using Sotbify.MobileApp.PinCreationWindow;
using Sotbify.MobileApp.TwoFactorWindow;
using Sotbify.MobileApp.LoadingScreen;
using Sotbify.MobileApp.NewRelicWindow;
using Sotbify.MobileApp.NewsWindow;
using Sotbify.MobileApp.NexusWindow;
using Sotbify.MobileApp.NotificationCenterWindow;
using Sotbify.MobileApp.PlayWindow;
using Sotbify.MobileApp.ProfileCharacterChoose;
using Sotbify.MobileApp.ProfileWindow;
using Sotbify.MobileApp.RelicActiveSetWindow;
using Sotbify.MobileApp.RelicChooseWindow;
using Sotbify.MobileApp.RelicListWindow;
using Sotbify.MobileApp.RelicsDescriptionWindow;
using Sotbify.MobileApp.ScanWindow;
using Sotbify.MobileApp.Services.BlurConfigSwitcher;
using Sotbify.MobileApp.Services.FirebaseCM.Runtime;
using Sotbify.MobileApp.Services.FirebaseCM.Runtime.Impls;
using Sotbify.MobileApp.Services.QRScanner;
using Sotbify.MobileApp.Services.ServerConstConverter;
using Sotbify.MobileApp.Stab.AchievementsWindow;
using Sotbify.MobileApp.Stab.GuideContentWindow;
using Sotbify.MobileApp.Stab.SettingsWindow;
using Sotbify.MobileApp.Stab.StabNexus;
using Sotbify.MobileApp.Stab.VRGuideWindow;
using Sotbify.MobileApp.TutorialWindows.ActiveSetTutorialOverlay;
using Sotbify.MobileApp.TutorialWindows.BonusTutorialOverlay;
using Sotbify.MobileApp.TutorialWindows.BonusesMoreTutorialInfo;
using Sotbify.MobileApp.TutorialWindows.ChatNotificationTutorialOverlay;
using Sotbify.MobileApp.TutorialWindows.ChatTutorialOverlay;
using Sotbify.MobileApp.TutorialWindows.EventsTutorialOverlay;
using Sotbify.MobileApp.TutorialWindows.HomeTutorialOverlay;
using Sotbify.MobileApp.TutorialWindows.RelicsMoreTutorialInfo;
using Sotbify.MobileApp.TutorialWindows.RelicsTutorialOverlay;
using Sotbify.MobileApp.TutorialWindows.WelcomeTutorialOverlay;
using Sotbify.MobileApp.UI.Common.Providers;
using Sotbify.MobileApp.UI.Window;
using Stab.SessionsWindow;
using Stab.StabChatWindow;
using Stab.StabHomeWindow;
using Stab.StabProfileWindow;
using Stab.WarrantyWindow;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ZXing;
using ZXing.QrCode;

namespace Sotbify.MobileApp.Scopes
{
    public class MainGameLifetimeScope : LifetimeScope
    {
        [SerializeField] private TranslucentImageSource _backgroundCamera;
        [SerializeField] private BonusesInstaller _bonusesInstaller;
        [SerializeField] private BlackjackWindowInstaller _blackjackInstaller;
        [SerializeField] private MediaPlayer _videoPlayer;

        [SerializeField] private MonoRootsRectProvider _rootsRectProvider;
        
        protected override void Configure(IContainerBuilder builder)
        {
            foreach (var installer in GetInstallers())
                installer.Install(builder);
            
            builder.RegisterWindowService(resolver => 
                resolver.TryResolve<IRootsRectProvider>(out var rootsRectProvider) 
                ? rootsRectProvider.CommonRoot
                : null);

            builder.RegisterComponent(_rootsRectProvider)
                .As<IRootsRectProvider>();

            builder.RegisterInstance(_backgroundCamera);

            builder.Register<VideoPlayer.VideoPlayer>(Lifetime.Singleton)
                .WithParameter(_videoPlayer)
                .AsImplementedInterfaces();

            builder.Register<ServerConstConverter>(Lifetime.Singleton)
                .AsImplementedInterfaces();
            
            builder.RegisterEntryPoint<FirebaseCMService>()
                .As<IFirebaseCMService>();
            
            builder.Register<QRScannerService>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            builder.Register<BlurConfigSwitcher>(Lifetime.Singleton)
                .AsImplementedInterfaces();
            
            builder.Register(_ => new QRCodeReader(), Lifetime.Singleton)
                .As<Reader>();
            
            builder.Register<BackHandler.BackHandler>(Lifetime.Singleton)
                .AsImplementedInterfaces();
            
            builder.Register(resolver =>
                {
                    var reader = resolver.Resolve<Reader>();
                    return new BarcodeReaderGeneric(reader, null, null);
                }, Lifetime.Singleton)
                .AsSelf();
        }
        
        private IEnumerable<IInstaller> GetInstallers()
        {
            yield return new MainPolicyWindowInstaller();
            yield return new DailyRelicWindowInstaller();
            yield return new GreetingsWindowInstaller();
            yield return new GlassesInstaller();
            yield return new ScanWindowInstaller();
            yield return new WindowSwitcherInstaller();    
            
            yield return new LoadingScreenInstaller();
            yield return new ProfileWindowInstaller();
            yield return new HiddenModeWindowInstaller();
            yield return new PinCreationWindowInstaller();
            yield return new CheckPasswordWindowInstaller();
            yield return new TwoFactorWindowInstaller();
            yield return new ProfileCharacterChooseInstaller();
            yield return new BlurredBackgroundInstaller();
            yield return new HomeInstaller();
            yield return new FeedListInstaller();
            yield return new FeedListOverlayInstaller();
            yield return new NotificationCenterInstaller();
            yield return new GameWindowInstaller();
            yield return new AllRelicsWindowInstaller();
            yield return new NewRelicWindowInstaller();
            yield return new RelicListWindowInstaller();
            yield return new RelicDescriptionInstaller();
            yield return new RelicChooseInstaller();
            yield return new RelicActiveSetInstaller();
            yield return new ChooseCharacterInstaller();
            yield return new FAQInstaller();
            yield return new NexusInstaller();

            yield return new WelcomeTutorialOverlayInstaller();
            yield return new RelicsTutorialOverlayInstaller();
            yield return new RelicsMoreTutorialInfoInstaller();
            yield return new HomeTutorialOverlayInstaller();
            yield return new EventsTutorialOverlayInstaller();
            yield return new ChatTutorialOverlayInstaller();
            yield return new ChatNotificationTutorialOverlayInstaller();
            yield return new BonusTutorialOverlayInstaller();
            yield return new BonusesMoreTutorialInfoInstaller();
            yield return new ActiveSetTutorialOverlayInstaller();
            
            yield return new ChatInstaller();
            yield return new ChatMessagesInstaller();
            
            yield return new NewsWindowInstaller();
            yield return new ApplicationUIInstaller();

            yield return new DeletePopupInstaller();
            yield return new AchievementsWindowInstaller();
            yield return new GuideContentWindowInstaller();
            yield return new StabChatWindowInstaller();
            yield return new SessionsWindowInstaller();
            yield return new SettingsWindowInstaller();
            yield return new StabNexusInstaller();
            yield return new StabHomeWindowInstaller();
            yield return new StabProfileWindowInstaller();
            yield return new VRGuideWindowInstaller();
            yield return new WarrantyWindowInstaller();
            
            
            yield return new CloseBlackjackPopupInstaller();
            
            
            yield return _bonusesInstaller;
            yield return _blackjackInstaller;
        }
    }
}
