using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sotbify.MobileApp.Common.Collections.SerializedMap
{
    [Serializable]
    public abstract class SerializedMap<TKey1, TKey2> : IMap<TKey1, TKey2>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey1> _keys1 = new();
        [SerializeField] private List<TKey2> _keys2 = new();
        
        private readonly Dictionary<TKey1, TKey2> _forward = new();
        private readonly Dictionary<TKey2, TKey1> _reverse = new();

        public TKey2 this[TKey1 key] => _forward[key];
        public TKey1 this[TKey2 key] => _reverse[key];

        public IReadOnlyDictionary<TKey1, TKey2> Forward => _forward;
        public IReadOnlyDictionary<TKey2, TKey1> Reverse => _reverse;

        public void Add(TKey1 key1, TKey2 key2)
        {
            _forward.Add(key1, key2);
            _reverse.Add(key2, key1);
        }
        
        public bool TryGet(TKey1 key1, out TKey2 key2) => 
            _forward.TryGetValue(key1, out key2);

        public bool TryGet(TKey2 key2, out TKey1 key1) => 
            _reverse.TryGetValue(key2, out key1);

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _forward.Clear();
            for (var i = 0; i < _keys1.Count && i < _keys2.Count; i++) 
                _forward[_keys1[i]] = _keys2[i];
            
            _reverse.Clear();
            for (var i = 0; i < _keys1.Count && i < _keys2.Count; i++) 
                _reverse[_keys2[i]] = _keys1[i];

            OnAfterDeserialize();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // _keys1.Clear();
            // _keys2.Clear();
            // foreach (var (key1, key2) in _forward)
            // {
            //     _keys1.Add(key1);
            //     _keys2.Add(key2);
            // }

            OnBeforeSerialize();
        }

        protected virtual void OnAfterDeserialize() { }
        protected virtual void OnBeforeSerialize() { }
    }
}