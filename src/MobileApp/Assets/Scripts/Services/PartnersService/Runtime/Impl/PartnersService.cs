using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.TextureProvider;
using Sotbify.MobileApp.Utils.Data;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Schemas;
using UnityEngine;
using UnityEngine.Pool;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.Services.PartnersService
{
    public class PartnersService : IPartnersService
    {
        private readonly Dictionary<string, PartnerData> _partners = new();
        
        private readonly Client _client;
        private readonly ITextureProvider _textureProvider;
        private readonly ILogger _logger;

        public PartnersService
        (
            Client client,
            ITextureProvider textureProvider,
            ILogger logger
        )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _textureProvider = textureProvider ?? throw new ArgumentNullException(nameof(textureProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyDictionary<string, PartnerData> Partners => _partners;
        public EInitializingState InitializingState { get; private set; } = EInitializingState.None;

        public async UniTask InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (_partners.Count > 0)
                return;

            InitializingState = EInitializingState.Pending;
            
            var partnersSchemas = await _client.GetPartners(cancellationToken);
            using (ListPool<UniTask>.Get(out var taskList))
            {
                foreach (var schema in partnersSchemas)
                    taskList.Add(CachePartnerAsync(schema));

                await UniTask.WhenAll(taskList);
                InitializingState = EInitializingState.Done;
            }
        }

        private async UniTask CachePartnerAsync(PartnersSchema schema)
        {
            Sprite logo = null;
            try
            {
                if(schema.LogoUrl is not null)
                    logo = await _textureProvider.LoadSpriteAsync(schema.LogoUrl);
            }
            catch (Exception e)
            {
                _logger.Error("Error loading logo", e, this);
            }

            var partner = new PartnerData
            {
                Id = schema.Id,
                Name = schema.Name,
                Url = schema.UrlString,
                Logo = logo,
                AccentColor = schema.Color
            };
            
            _partners.Add(partner.Name, partner);
        }
    }
}