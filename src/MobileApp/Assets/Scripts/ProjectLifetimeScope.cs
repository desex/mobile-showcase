using System.Collections.Generic;
using System.IO;
using System.Linq;
using Services.AppTransparencyService;
using Services.PackagesService;
using Sirenix.OdinInspector;
using Sotbifay.Images;
using Sotbifay.MobileApp.Common.Sql;
using Sotbifay.MobileApp.Logger;
using Sotbifay.MobileApp.WebView;
using Sotbify.MobileApp.Addressables;
using Sotbify.MobileApp.Analytics;
using Sotbify.MobileApp.Application;
using Sotbify.MobileApp.ApplicationUIScopeFacade;
using Sotbify.MobileApp.Assets;
using Sotbify.MobileApp.Common.Preferences;
using Sotbify.MobileApp.Input;
using Sotbify.MobileApp.ItemRenderer;
using Sotbify.MobileApp.Localization;
using Sotbify.MobileApp.Logger;
using Sotbify.MobileApp.PermissionService;
using Sotbify.MobileApp.PinPassword;
using Sotbify.MobileApp.Relic;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Services.Browser;
using Sotbify.MobileApp.Services.CanvasLayoutAwaiter;
using Sotbify.MobileApp.Services.FirebaseCrashlyticsService;
using Sotbify.MobileApp.Services.PartnersService;
using Sotbify.MobileApp.Services.RemoteConfig;
using Sotbify.MobileApp.Services.SettingsOpener;
using Sotbify.MobileApp.Services.TutorialService;
using Sotbify.MobileApp.TextureProvider;
using Sotbify.MobileApp.UI.Common.Texture;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp
{
    public sealed class ProjectLifetimeScope : LifetimeScope
    {
        [Header("Parameters")]
        [ValueDropdown(nameof(GetScriptableObjectConfigs), AppendNextDrawer = true, IsUniqueList = true, NumberOfItemsBeforeEnablingSearch = 10)]
        [SerializeField] private List<ScriptableObject> _scriptableObjectConfigs = new();
        
        [SerializeField] private RemoteConfigInstaller remoteConfigInstaller;
        [SerializeField] private ItemRendererProjectInstaller _itemRendererProjectInstaller;
        
        protected override void Configure(IContainerBuilder builder)
        {
            foreach (var installer in GetInstallers())
                installer.Install(builder);
            
            builder.RegisterImages();
            
            RegisterScriptableObjectConfigs(builder);
            
            BindServices(builder);
            
            builder.Register<IntervalTextureConverter>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            builder.Register<AppTransparencyService>(Lifetime.Singleton)
                .AsImplementedInterfaces();
                
            builder.Register<PartnersService>(Lifetime.Singleton)
                .AsImplementedInterfaces();
        }
        
        private void RegisterScriptableObjectConfigs(IContainerBuilder builder)
        {
            var hasNullConfigs = false;
            foreach (var so in _scriptableObjectConfigs)
            {
                if (so == null)
                {
                    hasNullConfigs = true;
                    continue;
                }

                builder.RegisterInstance(so)
                    .AsImplementedInterfaces();
            }

            if (hasNullConfigs)
            {
                builder.RegisterBuildCallback(resolver =>
                {
                    var logger = resolver.Resolve<ILogger>();

                    if (logger.IsEnabled(LogLevel.Warning))
                        logger.Warning($"There is nullable install parameter");
                });
            }
        }
        
        private IEnumerable<IInstaller> GetInstallers()
        {
            yield return new LoggerProjectInstaller();
            yield return new LocalizationProjectInstaller();
            yield return new AnalyticsProjectInstaller();
            yield return new ApplicationInstaller();
            yield return new PreferencesInstaller();
            yield return new PinPasswordInstaller();
            yield return new AssetsProjectInstaller();
            yield return new ServerInstaller();
            yield return new AddressablesProjectInstaller();
            yield return new WebViewInstaller();
            yield return new SqlProjectInstaller();
            yield return new TextureProviderProjectInstaller();
            yield return new InputProjectInstaller();
            yield return new RelicInstaller();
            
            yield return remoteConfigInstaller;
            yield return _itemRendererProjectInstaller;
        }

        private static void BindServices(IContainerBuilder builder)
        {
            builder.Register<TutorialStepService>(Lifetime.Singleton)
                .AsImplementedInterfaces();
            
            builder.Register<PermissionService.PermissionService>(Lifetime.Singleton)
                .As<IPermissionService>();
            
            builder.Register<PackagesService>(Lifetime.Singleton)
                .As<IPackagesService>();

            builder.Register<Browser>(Lifetime.Singleton)
                .As<IBrowser>();

            builder.Register<SettingsOpener>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            builder.Register<FirebaseCrashlyticsService>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            builder.Register<CanvasLayoutAwaiterService>(Lifetime.Singleton)
                .AsImplementedInterfaces();
            
            builder.Register<ApplicationUIScopeFacade.ApplicationUIScopeFacade>(Lifetime.Singleton)
                .As<IApplicationUIScopeFacade>();
        }
        
        private static IEnumerable<ScriptableObject> GetScriptableObjectConfigs()
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets" })
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .Where(static path => string.IsNullOrEmpty(path) == false)
                .Where(static path => Path.GetFileNameWithoutExtension(path).EndsWith("_Config"))
                .Select(static path => UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(path));
#else
            return Enumerable.Empty<ScriptableObject>();
#endif
        }
    }    
}

