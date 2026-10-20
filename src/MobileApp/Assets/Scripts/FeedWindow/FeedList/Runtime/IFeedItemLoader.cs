using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.FeedWindow.FeedList.Models;

namespace Sotbify.MobileApp.FeedWindow.FeedList
{
    public interface IFeedItemLoader
    {
        public bool IsAllLoaded { get; }
        void Reset();
        public UniTask<IReadOnlyList<IFeedModel>> LoadMoreAsync(CancellationToken cancellationToken = default);
    }
}
