using System.Collections.Generic;
using NUnit.Framework;
using Sotbifay.MobileApp.Common.Tools.Editor.Utilities;
using UnityEditor;
using UnityEngine;

namespace Sotbifay.Launcher.Common.Tests.EditMode
{
    public sealed class AssetsValidationTests
    {
        [TestCaseSource(nameof(GetAllAssetsPaths))]
        public void All_Assets_Should_Not_Have_Missing_Script(string assetPath)
        {
            var asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
            Assert.IsNotNull(asset);


            if (asset is not ScriptableObject scriptableObject) 
                return;
            
            var monoScript = MonoScript.FromScriptableObject(scriptableObject);
            Assert.IsNotNull(monoScript);
            Assert.IsNotNull(monoScript.GetClass());
        }
        
        private static IEnumerable<string> GetAllAssetsPaths() => AssetsUtils.FindAllByExtensions(".asset");
    }
}