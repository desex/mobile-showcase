using R3;

namespace Stab.StabHomeWindow
{
    public interface IStabHome
    {
        public Observable<EHomeItem> ChangeScreen{ get; }
        public void Open(EHomeItem eItem);
    }
}