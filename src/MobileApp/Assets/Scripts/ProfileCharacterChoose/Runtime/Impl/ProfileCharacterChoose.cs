using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Utils.Schemas.Profile;
using UnityEngine.Pool;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Localization;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ProfileCharacterChoose
{
    public class ProfileCharacterChoose : IProfileCharacterChoose, IDisposable
    {
        private readonly List<ProfileCharacterData> _characters = new();
        private readonly Subject<Unit> _close = new();
        private readonly ReactiveProperty<ProfileCharacterData> _currentCharacter = new();
        
        private readonly Client _client;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        
        private int _index = -1;
        private int _loadVersion;

        public ProfileCharacterChoose(Client client, ILocalizationService localizationService, ILogger logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ReadOnlyReactiveProperty<ProfileCharacterData> CurrentCharacter => _currentCharacter;
        public Observable<Unit> CloseObservable => _close;
        public bool HasPrevious => _index > 0;
        public bool HasNext => _index >= 0 && _index < _characters.Count - 1;
        public int CurrentCharacterIndex => _index;
        public int CharacterCount => _characters.Count;

        public void Previous()
        {
            if (HasPrevious)
                _currentCharacter.Value = _characters[--_index];
        }

        public void Next()
        {
            if (HasNext)
                _currentCharacter.Value = _characters[++_index];
        }

        public void Close()
        {
            _close.OnNext(Unit.Default);
        }

        public async UniTask LoadCharacters(CancellationToken cancellationToken = default)
        {
            var version = ++_loadVersion;
            var profileCharacterSchemas = await _client.LoadCharacters(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (version != _loadVersion)
                return;

            using (ListPool<ProfileCharacterData>.Get(out var data))
            {
                var localeCode = _localizationService.LocaleCode;

                foreach (var profileCharacterSchema in profileCharacterSchemas ?? Array.Empty<ProfileCharacterSchema>())
                    data.Add(ConvertToData(profileCharacterSchema, localeCode));
                
                _characters.Clear();
                _characters.AddRange(data);

                var activeIndex = _characters.FindIndex(character => character.IsActive);
                if (activeIndex < 0)
                    activeIndex = _characters.FindIndex(character => character.Id == _currentCharacter.CurrentValue?.Id);
                _index = _characters.Count == 0 ? -1 : activeIndex >= 0 ? activeIndex : 0;
                _currentCharacter.Value = _index < 0 ? null : _characters[_index];
            }
        }

        public async UniTask ChooseAsync(CancellationToken cancellationToken = default)
        {
            var character = _currentCharacter.CurrentValue;
            if (character == null || character.IsActive)
                return;

            try
            {
                if (!await _client.ActivateCharacter(character.Id, cancellationToken))
                {
                    _logger.Error($"Failed to activate character {character.Id}", context: this);
                    return;
                }

                foreach (var data in _characters)
                    data.IsActive = data.Id == character.Id;
                
                _currentCharacter.Value = _characters[_index];
                _currentCharacter.ForceNotify();
                Close();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                _logger.Error($"Failed to activate character {character.Id}", exception, this);
            }
        }

        private static ProfileCharacterData ConvertToData(ProfileCharacterSchema schema, string localeCode)
        {
            var name = schema.NameTranslations is not null && schema.NameTranslations.TryGetValue(localeCode, out var localizedName)
                ? localizedName
                : schema.Name;
            
            var description = schema.DescriptionTranslations is not null 
                              && schema.DescriptionTranslations.TryGetValue(localeCode, out var localizedDescription)
                ? localizedDescription
                : schema.Description;
            
            return new ProfileCharacterData
            {
                Id = schema.Id,
                Name = name,
                Description = description,
                Color = schema.Color,
                ImageUri = schema.ImageUrl,
                IsActive = schema.IsActive
            };
        }

        public void Dispose()
        {
            _loadVersion++;
            _currentCharacter.Dispose();
        }
    }
}
