using System;
using Sotbify.MobileApp.Common.SerializableDictionary;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Sotbify.MobileApp.Services.RemoteConfig
{
    [Serializable]
    public class RemoteConfigInstaller : IInstaller
    {
        [SerializeField] private RemoteConfigDictionary _defaultConfigs;
        
        public void Install(IContainerBuilder builder)
        {
            builder.Register<FirebaseRemoteConfigService>(Lifetime.Singleton)
                .WithParameter("defaults", _defaultConfigs)
                .AsImplementedInterfaces()
                .As<IRemoteConfigService>();
        }
    }

    [Serializable]
    public sealed class RemoteConfigDictionary : SerializableDictionary<string, string>
    {
    }
}