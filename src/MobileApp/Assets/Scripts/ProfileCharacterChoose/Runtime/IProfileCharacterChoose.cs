using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

namespace Sotbify.MobileApp.ProfileCharacterChoose
{
    public interface IProfileCharacterChoose
    {
        public ReadOnlyReactiveProperty<ProfileCharacterData> CurrentCharacter { get; }
        public Observable<Unit> CloseObservable { get; }
        public bool HasPrevious { get; }
        public bool HasNext { get; }
        public int CurrentCharacterIndex { get; }
        public int CharacterCount { get; }
        public UniTask LoadCharacters(CancellationToken cancellationToken = default);

        public UniTask ChooseAsync(CancellationToken cancellationToken = default);
        public void Previous();
        public void Next();
        public void Close();
    }
}
