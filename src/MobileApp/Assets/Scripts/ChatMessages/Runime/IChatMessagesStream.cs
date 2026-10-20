using R3;

namespace Sotbify.MobileApp.ChatMessages
{
    public interface IChatMessagesStream
    {
        public Observable<Unit> ReconnectObservable { get; }
        public void Start();
    }
}