using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.BonusesWindow.Core.Models;

namespace Sotbify.MobileApp.BonusesWindow
{
    public interface IBonusItemLoader
    {
        public UniTask<IReadOnlyList<IBonusModel>> LoadBonusesAsync(CancellationToken cancellationToken = default);
    }
}