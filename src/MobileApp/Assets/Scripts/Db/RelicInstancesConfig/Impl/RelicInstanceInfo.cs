using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Sotbify.MobileApp.Db.RelicInstancesConfig
{
    [Serializable]
    public struct RelicInstanceInfo
    {
        [field: SerializeField]
        public AssetReferenceGameObject AssetReference { get; private set; }
    }
}