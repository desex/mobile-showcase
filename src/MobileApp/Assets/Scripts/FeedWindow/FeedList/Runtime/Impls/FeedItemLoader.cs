using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.FeedWindow.FeedList.Models;
using Sotbify.MobileApp.Localization;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Services.PartnersService;
using Sotbify.MobileApp.Services.ServerConstConverter;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Schemas.Feed;
using UnityEngine.Pool;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.FeedWindow.FeedList
{
    public class FeedItemLoader : IFeedItemLoader
    {
        private const int Limit = 10;

        private readonly Client _client;
        private readonly IPartnersService _partnersService;
        private readonly IServerConstConverter _serverConstConverter;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;

        private int? _currentOffset = 0;
        private int _generation;

        public FeedItemLoader
        (
            Client client,
            IPartnersService partnersService,
            IServerConstConverter serverConstConverter,
            ILocalizationService localizationService,
            ILogger logger
        )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _partnersService = partnersService ?? throw new ArgumentNullException(nameof(partnersService));
            _serverConstConverter = serverConstConverter ?? throw new ArgumentNullException(nameof(serverConstConverter));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsAllLoaded => _currentOffset == null;

        public void Reset()
        {
            _generation++;
            _currentOffset = 0;
        }

        public UniTask<IReadOnlyList<IFeedModel>> LoadMoreAsync(CancellationToken cancellationToken = default)
        {
            if (_currentOffset == null)
                return UniTask.FromResult<IReadOnlyList<IFeedModel>>(Array.Empty<IFeedModel>());

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug("Loading more news");

            return LoadMoreAsyncInternal(cancellationToken);
        }

        private async UniTask<IReadOnlyList<IFeedModel>> LoadMoreAsyncInternal(CancellationToken cancellationToken)
        {
            var generation = _generation;
            var offset = _currentOffset!.Value;
            using (ListPool<int>.Get(out var partnerIds))
            {
                foreach (var (_, partnerData) in _partnersService.Partners)
                    partnerIds.Add(partnerData.Id);

                var postSchemas = await _client.GetHomeItemsAsync(partnerIds, Limit, offset, cancellationToken);

                if (generation != _generation)
                    return Array.Empty<IFeedModel>();

                if (postSchemas.Count == 0)
                {
                    _currentOffset = null;
                    return Array.Empty<IFeedModel>();
                }

                var result = new List<IFeedModel>(postSchemas.Count);

                var localeCode = _localizationService.LocaleCode;

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var postSchema in postSchemas)
                {
                    var model = ConvertToData(postSchema, localeCode);
                    if (model is not null)
                    {
                        result.Add(model);
                    }
                    else if (_logger.IsEnabled(LogLevel.Warning))
                    {
                        _logger.Warning($"Unknown home item type: {postSchema.Type}", context: this);
                    }
                }

                _currentOffset = offset + postSchemas.Count;
                return result;
            }
        }

        private IFeedModel ConvertToData(FeedItemSchema schema, string localeCode)
        {
            var homeItemType = _serverConstConverter.ToEHomeItemType(schema.Type);
            return homeItemType switch
            {
                EHomeItemType.Bonus => ConvertAsBonus(schema, localeCode),
                EHomeItemType.AppNews or EHomeItemType.ProjectNews => ConvertAsPost(schema, homeItemType, localeCode),
                _ => null
            };
        }

        private FeedBonusModel ConvertAsBonus(FeedItemSchema schema, string localeCode)
        {
            var bonusSchema = schema.PromoCode;
            if (bonusSchema is null)
            {
                var title = schema.TitleTranslations is not null && schema.TitleTranslations.TryGetValue(localeCode, out var localizedTitle)
                    ? localizedTitle
                    : schema.Title;
                
                var description = schema.DescriptionTranslations is not null 
                                  && schema.DescriptionTranslations.TryGetValue(localeCode, out var localizedDescription)
                    ? localizedDescription
                    : schema.Description;
                
                return new FeedBonusModel
                {
                    Id = schema.Id,
                    Title = title,
                    Description = description,
                    BonusType = EBonusType.NoDeposit,
                    BonusImageUri = schema.ImageUrl,
                    Promocode = string.Empty,
                    ItemType = EHomeItemType.Bonus,
                    ActivationUrl = string.Empty
                };
            }
            
            var bonusTitle = bonusSchema.TitleTranslations is not null 
                             && bonusSchema.TitleTranslations.TryGetValue(localeCode, out var localizedBonusTitle)
                ? localizedBonusTitle
                : bonusSchema.Title;
            
            var bonusDescription = bonusSchema.DescriptionTranslations is not null 
                             && bonusSchema.DescriptionTranslations.TryGetValue(localeCode, out var localizedBonusDescription)
                ? localizedBonusDescription
                : bonusSchema.Description;
            
            var bonusType = _serverConstConverter.ToEBonusType(bonusSchema?.Type ?? string.Empty);

            return new FeedBonusModel
            {
                Id = schema.Id,
                Title = bonusTitle,
                Description = bonusDescription,
                BonusType = bonusType,
                BonusImageUri = schema.ImageUrl,
                Promocode = bonusSchema.Code ?? string.Empty,
                ItemType = EHomeItemType.Bonus,
                ActivationUrl = bonusSchema.ActivationUrlString ?? string.Empty
            };
        }

        private static PostModel ConvertAsPost(FeedItemSchema schema, EHomeItemType itemType, string localeCode)
        {
            var title = schema.TitleTranslations is not null && schema.TitleTranslations.TryGetValue(localeCode, out var localizedTitle)
                ? localizedTitle
                : schema.Title;
            
            var description = schema.DescriptionTranslations is not null 
                              && schema.DescriptionTranslations.TryGetValue(localeCode, out var localizedDescription)
                ? localizedDescription
                : schema.Description;
            
            var feedButton = schema.FeedButton;
            if (feedButton is null)
            {
                return new PostModel
                {
                    Id = schema.Id,
                    Title = title,
                    Description = description,
                    ImageUri = schema.ImageUrl,
                    IsRead = schema.IsRead,
                    PublishedAt = schema.CreatedAt,
                    Color = schema.Color,
                    ItemType = itemType,
                    ShowHrefButtons = false
                };
            }
            
            var feedButtonTitle = feedButton.TitleTranslations is not null 
                                  && feedButton.TitleTranslations.TryGetValue(localeCode, out var localizedFeedButtonTitle)
                ? localizedFeedButtonTitle
                : feedButton.Title;

            return new PostModel
            {
                Id = schema.Id,
                Title = title,
                Description = description,
                ImageUri = schema.ImageUrl,
                PublishedAt = schema.CreatedAt,
                ShowHrefButtons = true,
                IsRead = schema.IsRead,
                ItemType = itemType,
                Color = schema.Color,
                ButtonText = feedButtonTitle,
                ButtonHref = feedButton.Url
            };
        }
    }
}