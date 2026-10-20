using System.Collections.Generic;
using LeTai.Asset.TranslucentImage;
using Sotbify.MobileApp.Utils.Enums.Nexus;

namespace Sotbify.MobileApp.Db.BlurConfig
{
    public interface IBlurConfiig
    {
        public IReadOnlyDictionary<ENexusCategory, ScalableBlurConfig> BlurConfigs { get; }
    }
}