using R3;
using System;
using Sotbify.MobileApp.Relic;
using Com.ForbiddenByte.OSA.CustomAdapters.GridView;
using UnityEngine;

namespace Sotbify.MobileApp.RelicListWindow.Adapter
{
    public class RelicsViewsHolder : CellViewsHolder
    {
        private RelicView _view;
        private Func<IRelicItem, IRelicInstance> _relicInstanceFactory;

        private IDisposable _bindings;
        private IRelicInstance _relicInstance;

        public Observable<int> OpenDescriptionObservable => _view.OpenDescription;

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

        private static GradientColorKey[] UpdateGradientColors
        (
            GradientColorKey[] keys,
            Color gradientFromColor,
            Color mainColor,
            Color gradientToColor
        )
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}