using System;
using System.Collections.Generic;
using LeTai.Asset.TranslucentImage;
using Sotbify.MobileApp.Common.SerializableDictionary;
using Sotbify.MobileApp.Utils.Enums.Nexus;
using UnityEngine;

namespace Sotbify.MobileApp.Db.BlurConfig
{
    [CreateAssetMenu(menuName = "Settings/Blur_Config", fileName = "Blur_Config")]
    public class BlurConfig : ScriptableObject, IBlurConfiig 
    {
        [SerializeField] private BlurConfigs _blurConfigs;

        public IReadOnlyDictionary<ENexusCategory, ScalableBlurConfig> BlurConfigs => _blurConfigs;
    }

    [Serializable]
    public sealed class BlurConfigs : SerializableDictionary<ENexusCategory, ScalableBlurConfig>
    {
    }
}