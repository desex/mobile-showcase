using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Localization.Settings;

namespace Sotbify.MobileApp.Localization
{
    public sealed class LocalizationService : ILocalizationService
    {
        public string LocaleCode
        {
            get => LocalizationSettings.SelectedLocale?.Identifier.Code ?? "en";
            set
            {
                if(value is null)
                    throw new ArgumentNullException(nameof(value));
                
                if(string.IsNullOrEmpty(value))
                    return;
                
                var locale = LocalizationSettings.AvailableLocales.GetLocale(value);
                if(locale is null)
                    throw new InvalidOperationException($"The locale '{value}' was not found");
                
                LocalizationSettings.SelectedLocale = locale;
            }
        }

        public async UniTask InitializeAsync(CancellationToken cancellationToken = default)
        {
            await LocalizationSettings.InitializationOperation
                .ToUniTask(cancellationToken: cancellationToken);
            
            await LocalizationSettings.SelectedLocaleAsync
                .ToUniTask(cancellationToken: cancellationToken);
        }
    }
}