using System;
using System.Threading.Tasks;
using Sotbifay.Builder.Editor;
using UnityEditor;
using UnityEngine;

namespace Sotbify.MobileApp.Builder.Editor
{
    public static class BuilderMenuItems
    {
        [MenuItem(Consts.MenuItemRoot + "Build")]
        public static async void Build()
        {
            try
            {
                var commonOptions = new CommonOptions
                {
                    BuildType = BuildType.Release
                };
            
                var buildReport = await Build(commonOptions);
                RevealInFinder(buildReport);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        [MenuItem(Consts.MenuItemRoot + "Debug Build")]
        public static async void DebugBuild()
        {
            try
            {
                var commonOptions = new CommonOptions
                {
                    BuildType = BuildType.Debug,
                };
            
                var buildReport = await Build(commonOptions);
                RevealInFinder(buildReport);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        [MenuItem(Consts.MenuItemRoot + "Debug Build(Profiler)")]
        public static async void DebugBuildWithProfiler()
        {
            try
            {
                var commonOptions = new CommonOptions
                {
                    BuildType = BuildType.Debug,
                    UseProfiler = true
                };
            
                var buildReport = await Build(commonOptions);
                RevealInFinder(buildReport);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        [MenuItem(Consts.MenuItemRoot + "Debug Build(Deep profiler)")]
        public static async void DebugBuildWithDeepProfiler()
        {
            try
            {
                var commonOptions = new CommonOptions
                {
                    BuildType = BuildType.Debug,
                    UseProfiler = true,
                    DeepProfiler = true
                };
            
                var buildReport = await Build(commonOptions);
                RevealInFinder(buildReport);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static Task<BuildReport> Build(CommonOptions commonOptions)
        {
            var androidOptions = new AndroidOptions();
            var androidKeystoreOptions = new AndroidKeystoreOptions();
            
            var buildPipeline = BuildPipeline<BuildReport>
                .Create(new BuildReport())
                .Append(new SetStartBuildTimestampStep())
                .Append(new CreateBuildPlayerOptionsStep())
                .Append(new ApplyCommonOptionsStep(commonOptions))
                .Append(new ApplyAndroidOptionsStep(androidOptions))
                .Append(new ApplyAndroidKeystoreOptionsStep(androidKeystoreOptions))
                .Append(new ClearCacheAddressablesStep())
                .Append(new BuildAddressablesStep())
                .Append(new BuildStep())
                .Append(new SetFinishBuildTimestampStep())
                .Append(new CreateBuildReportFileStep(commonOptions));

            return buildPipeline.ExecuteAsync();
        }

        private static void RevealInFinder(BuildReport buildReport)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if(buildReport is null)
                return;
            
            var locationPathName = buildReport.BuildOptions.locationPathName;
            EditorUtility.RevealInFinder(locationPathName);
        }
    }
}