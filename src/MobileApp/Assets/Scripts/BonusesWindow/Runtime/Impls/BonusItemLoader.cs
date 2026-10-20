using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.BonusesWindow.Core.Models;
using Sotbify.MobileApp.Localization;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Services.ServerConstConverter;
using Sotbify.MobileApp.Utils.Schemas;
using Sotbify.MobileApp.Utils.Schemas.Bonus;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.BonusesWindow
{
    public class BonusItemLoader : IBonusItemLoader
    {
        private readonly Client _client;
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IServerConstConverter _serverConstConverter;
        
        public BonusItemLoader
        (
            Client client,
            ILogger logger,
            ILocalizationService localizationService,
            IServerConstConverter serverConstConverter
        )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _serverConstConverter = serverConstConverter ?? throw new ArgumentNullException(nameof(serverConstConverter));
        }

        public UniTask<IReadOnlyList<IBonusModel>> LoadBonusesAsync(CancellationToken cancellationToken = default)
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask<IReadOnlyList<IBonusModel>> LoadBonusesAsyncInternal(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private IBonusModel ConvertToData(BonusSchema schema, string localeCode)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private static IReadOnlyList<BonusLine> ConvertLines(IReadOnlyList<BonusLineSchema> lines, string localeCode)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}