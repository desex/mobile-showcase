using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.TextureProvider;
using Sotbify.MobileApp.Utils.Schemas.Profile;
using UnityEngine;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Localization;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ProfileWindow.Core
{
    public class Profile : IProfile
    {
        private readonly ReactiveProperty<string> _playerName = new();
        private readonly ReactiveProperty<string> _characterName = new();
        private readonly ReactiveProperty<Color> _backgroundColor = new();
        private readonly ReactiveProperty<Sprite> _characterSprite = new();
        private readonly ReactiveProperty<bool> _isImageLoading = new();
        private readonly Subject<Unit> _closeProfile = new();
        private readonly Subject<Unit> _editCharacter = new();
        private readonly Subject<Unit> _passwordLogin = new();
        
        private readonly Client _client;
        private readonly ITextureProvider _textureProvider;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;

        public Profile
        (
            Client client,
            ITextureProvider textureProvider,
            ILocalizationService localizationService,
            ILogger logger
        )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _textureProvider = textureProvider ?? throw new ArgumentNullException(nameof(textureProvider));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public ReadOnlyReactiveProperty<string> PlayerName => _playerName;
        public ReadOnlyReactiveProperty<string> CharacterName => _characterName;
        public ReadOnlyReactiveProperty<Color> BackgroundColor => _backgroundColor;
        public ReadOnlyReactiveProperty<Sprite> CharacterSprite => _characterSprite;
        public ReadOnlyReactiveProperty<bool> IsImageLoading => _isImageLoading;
        public bool IsPasswordEnabled { get; private set; }
        public Observable<Unit> CloseProfile => _closeProfile;
        public Observable<Unit> EditCharacterObservable => _editCharacter;
        public Observable<Unit> PasswordLoginObservable => _passwordLogin;

        public void OpenPasswordLogin() {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void SetPasswordEnabled(bool isEnabled) {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask LoadProfileData(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void EditCharacter()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Close()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask<ProfileCharacterSchema?> ResolveActiveCharacterAsync(ProfileSchema schema, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private static ProfileData ConvertToData(ProfileSchema schema, ProfileCharacterSchema? activeCharacter, string localeCode)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
