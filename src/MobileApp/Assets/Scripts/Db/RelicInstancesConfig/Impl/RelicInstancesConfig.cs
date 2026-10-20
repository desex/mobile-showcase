using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sotbify.MobileApp.Common.SerializableDictionary;
using UnityEngine;

namespace Sotbify.MobileApp.Db.RelicInstancesConfig
{
    [CreateAssetMenu(menuName = "Settings/RelicInstances_Config", fileName = "RelicInstances_Config")]
    public sealed class RelicInstancesConfig : ScriptableObject, IRelicInstancesConfig
    {
        [SerializeField] private RelicInstancesDictionary _instances;

        public IReadOnlyDictionary<string, RelicInstanceInfo> Instances => _instances;

        #region Editor
#if UNITY_EDITOR
        
        [Button]
        private void PrintAllKeys()
        {
            if(_instances is null)
                return;

            var keys = string.Join(Environment.NewLine, _instances.Keys);
            Debug.Log($"Keys:{Environment.NewLine}{keys}");
        }
        
#endif
        #endregion
    }

    [Serializable]
    public sealed class RelicInstancesDictionary : SerializableDictionary<string, RelicInstanceInfo> { }
}