using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbify.MobileApp.FeedWindow.FeedList.Models;
using Sotbify.MobileApp.Utils.Enums;

namespace Sotbify.MobileApp.FeedWindow.FeedList
{
    public interface IFeedList
    {
        public IReadOnlyObservableList<IFeedModel> HomeModels { get; }
        public bool IsAllHomeItemsLoaded { get; }
        public Observable<int> NewsReadMarked { get; }
        public Observable<int> OpenContentObservable { get; }


        public void OpenFeedContent(int id);
        public UniTask LoadMoreHomeItems(CancellationToken cancellationToken = default);
        public void FilterItems(EHomeItemType chosenType);
        public bool IsNewsRead(int id);
        public void MarkNewsRead(int id);
        
        
        public void Clear();
        public void OpenNotificationCenter();
        
        public UniTask OpenBrowser(string url, CancellationToken cancellationToken = default);
        public void CopyPromocode(string promocode);
    }
}