using System;
using Sotbify.MobileApp.Server.HttpClient.Core;
using Sotbify.MobileApp.Server.HttpClient.Implementations;
using Sotbify.MobileApp.Server.Serialization;
using UnityEngine.Pool;
using VContainer;
using VContainer.Unity;

namespace Sotbify.MobileApp.Server
{
    public sealed class ServerInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterFactory(() => new HttpRequestBuilder());

            builder.RegisterEntryPoint(resolver =>
                {
                    var httpRequestBuilderFactory = resolver.Resolve<Func<HttpRequestBuilder>>();
                    return new ObjectPool<HttpRequestBuilder>(httpRequestBuilderFactory, actionOnRelease: b => b.Clear());
                }, Lifetime.Singleton)
                .As<IObjectPool<HttpRequestBuilder>>();

            builder.Register<JsonSerializer>(Lifetime.Singleton)
                .As<ISerializer>();

            builder.Register<BestHttpClient>(Lifetime.Singleton)
                .AsImplementedInterfaces();
            
            builder.Register<Server>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            builder.Register<Client>(Lifetime.Singleton)
                .AsSelf();
        }
    }
}