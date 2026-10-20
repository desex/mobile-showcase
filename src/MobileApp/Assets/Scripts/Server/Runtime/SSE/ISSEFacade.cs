using R3;

namespace Sotbify.MobileApp.Server.SSE
{
    public interface ISSEFacade
    {
        public Observable<SSEFacadeWrapper> OpenObservable { get; }
        public Observable<SSEFacadeWrapper> ReconnectObservable { get; }
        public Observable<SSEFacadeWrapper> CloseObservable { get; }
        public Observable<SSEFacadeWrapper> ErrorObservable { get; }
        public Observable<SSEFacadeWrapper> RetryObservable { get; }
        public Observable<SSEFacadeWrapper> MessageObservable { get; }
        public Observable<SSEFacadeWrapper> CommentObservable { get; }
        public Observable<SSEFacadeWrapper> StateChangeObservable { get; }

        public void Start();
        public void Stop();
    }
}