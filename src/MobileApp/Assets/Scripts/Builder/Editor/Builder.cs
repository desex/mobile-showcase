using System;
using Sotbifay.Builder.Editor;
using Sotbify.MobileApp.Builder.Editor.Options;
using Sotbify.MobileApp.Builder.Editor.Steps;
using UnityEngine;

namespace Sotbify.MobileApp.Builder.Editor
{
    public static class Builder
    {
        public static async void BuildAndroid()
        {
            try
            {
                var argumentsReader = ArgumentsReader.Create();
                var commonOptions = argumentsReader.To<CommonOptions>();
                var androidOptions = argumentsReader.To<AndroidOptions>();
                var androidKeystoreOptions = argumentsReader.To<AndroidKeystoreOptions>();

                var buildReport = new BuildReport();
                var buildPipeline = BuildPipeline<BuildReport>
                    .Create(buildReport)
                    .Append(new SetStartBuildTimestampStep())
                    .Append(new CreateBuildPlayerOptionsStep())
                    .Append(new ApplyCommonOptionsStep(commonOptions))
                    .Append(new ApplyAndroidOptionsStep(androidOptions))
                    .Append(new ApplyAndroidKeystoreOptionsStep(androidKeystoreOptions))
                    .Append(new ClearCacheAddressablesStep())
                    // .Append(new BuildAddressablesStep())
                    .Append(new BuildStep())
                    .Append(new SetFinishBuildTimestampStep())
                    .Append(new CreateBuildReportFileStep(commonOptions));

                await buildPipeline.ExecuteAsync();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        public static async void BuildIOS()
        {
            try
            {
                var argumentsReader = ArgumentsReader.Create();
                var commonOptions = argumentsReader.To<CommonOptions>();
                var iOSOptions = argumentsReader.To<IOSOptions>();

                var buildReport = new BuildReport();
                var buildPipeline = BuildPipeline<BuildReport>
                    .Create(buildReport)
                    .Append(new SetStartBuildTimestampStep())
                    .Append(new CreateBuildPlayerOptionsStep())
                    .Append(new ApplyIOSOptionsStep(iOSOptions))
                    .Append(new ApplyCommonIOSOptionsStep(commonOptions))
                    .Append(new ClearCacheAddressablesStep())
                    // .Append(new BuildAddressablesStep())
                    .Append(new BuildStep())
                    .Append(new SetFinishBuildTimestampStep())
                    .Append(new CreateBuildReportFileStep(commonOptions));

                await buildPipeline.ExecuteAsync();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}