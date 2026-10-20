using System;
using System.Threading;
using Com.ForbiddenByte.OSA.CustomAdapters.GridView;
using Cysharp.Threading.Tasks;

namespace Sotbify.MobileApp.PlayWindow.Adapter
{
    public class GameViewsHolder : CellViewsHolder
    {
        public PartnerView PartnerView { get; private set; }
        
        private IPartnerModel _model;

        public string Link => _model.Link;

        public override void CollectViews()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public async UniTask UpdateViews(IPartnerModel model, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}