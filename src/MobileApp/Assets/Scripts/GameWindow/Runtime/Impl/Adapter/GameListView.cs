using System;
using Com.ForbiddenByte.OSA.CustomAdapters.GridView;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Common.OSA;
using Sotbify.MobileApp.Services.Browser;
using Sotbify.MobileApp.Utils.Extensions;
using VContainer;

namespace Sotbify.MobileApp.PlayWindow.Adapter
{
    public class GameListView : GridAdapter<GridParams, GameViewsHolder>
    {
        private ListDataHelper<IPartnerModel> _data;
        private ILogger _logger;
        private IBrowser _browser;

        [Inject]
        public void Construct(
            ILogger logger,
            IBrowser browser
        )
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public ListDataHelper<IPartnerModel> Data => _data ??= new ListDataHelper<IPartnerModel>(this, false, false);

        protected override CellGroupViewsHolder<GameViewsHolder> CreateViewsHolder(int itemIndex)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void UpdateCellViewsHolder(GameViewsHolder viewsHolder)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}