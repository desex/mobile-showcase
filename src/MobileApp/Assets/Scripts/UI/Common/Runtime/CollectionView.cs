using System.Collections.Generic;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Common
{
    public abstract class CollectionView<T>: MonoBehaviour 
        where T : Component
    {
        [SerializeField] private Transform _container;
        [SerializeField] private T _prefab;

        private readonly List<T> _views = new();
        
        public IReadOnlyList<T> Views => _views;
        
        public T CreateView()
        {      
            var view = Instantiate(_prefab, _container);
            _views.Add(view);

            OnViewCreated(view);
            return view;
        }

        public void AddView(T view)
        {
            _views.Add(view);
        }

        public void Clear()
        {
            foreach (var view in _views)
            {
                Destroy(view.gameObject);
            }
            
            _views.Clear();
        }
        
        protected virtual void OnViewCreated(T view)
        {
        }
    }
}