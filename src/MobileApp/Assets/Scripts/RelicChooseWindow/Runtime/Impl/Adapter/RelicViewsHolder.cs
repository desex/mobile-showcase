using System;
using Com.ForbiddenByte.OSA.CustomAdapters.GridView;
using R3;
using Sotbify.MobileApp.Relic;

namespace Sotbify.MobileApp.RelicChooseWindow.Adapter
{
    public class RelicViewsHolder : CellViewsHolder
    {
        private Func<IRelicItem, IRelicInstance> _relicInstanceFactory;

        private IDisposable _bindings;
        private IRelicInstance _relicInstance;

        public RelicChooseView RelicChooseView { get; private set; }

        public Observable<(int Id, IRelicInstance RelicInstance)> OpenDescriptionObservable => RelicChooseView
            .OpenSmallDescriptionObservable
            .Select(this, static (id, @this) => (id, @this._relicInstance));

        public void Constructor(Func<IRelicItem, IRelicInstance> relicInstanceFactory)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public override void CollectViews()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Bind(RelicModel model)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public override void OnBeforeRecycleOrDisable(int newItemIndex)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}