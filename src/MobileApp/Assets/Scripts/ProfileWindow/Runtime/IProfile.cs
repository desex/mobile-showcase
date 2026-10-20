using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Sotbify.MobileApp.ProfileWindow
{
    public interface IProfile
    {
        public ReadOnlyReactiveProperty<string> PlayerName { get; }
        public ReadOnlyReactiveProperty<string> CharacterName { get; }
        public ReadOnlyReactiveProperty<Color> BackgroundColor { get; }
        public ReadOnlyReactiveProperty<Sprite> CharacterSprite { get; }
        public ReadOnlyReactiveProperty<bool> IsImageLoading { get; }
        public bool IsPasswordEnabled { get; }
        public Observable<Unit> CloseProfile { get; }
        public Observable<Unit> EditCharacterObservable { get; }
        public Observable<Unit> PasswordLoginObservable { get; }

        public UniTask LoadProfileData(CancellationToken cancellationToken = default);

        public void SetPasswordEnabled(bool isEnabled);
        public void EditCharacter();
        public void OpenPasswordLogin();
        public void Close();
    }
}
