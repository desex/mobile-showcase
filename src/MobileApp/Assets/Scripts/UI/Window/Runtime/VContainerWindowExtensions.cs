using System;
using System.Runtime.CompilerServices;
using Sotbify.MobileApp.UI.Window.Presenter;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Sotbify.MobileApp.UI.Window
{
    public static class VContainerWindowExtensions
    {
        #region Register Serivce

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RegistrationBuilder RegisterWindowService(this IContainerBuilder builder, Transform parent = null)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            return builder
                .RegisterEntryPoint<WindowService>()
                .As<IWindowService>()
                .WithParameter(parent);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RegistrationBuilder RegisterWindowService(this IContainerBuilder builder, Func<IObjectResolver, Transform> value)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            return builder
                .RegisterEntryPoint<WindowService>()
                .As<IWindowService>()
                .WithParameter(value);
        }

        #endregion

        #region Register Window

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (RegistrationBuilder WindowRegistrationBuilder, RegistrationBuilder PresenterRegistrationBuilder)
            RegisterWindow<TWindowInterface, TWindowModel, TWindowPresenter, TWindowView>
            (
                this IContainerBuilder builder,
                Func<IObjectResolver, TWindowModel> windowImplementation = null,
                Func<IObjectResolver, TWindowPresenter> presenterImplementation = null,
                Lifetime lifetime = Lifetime.Singleton
            )
            where TWindowInterface : class
            where TWindowModel : TWindowInterface
            where TWindowPresenter : IWindowPresenter<TWindowInterface, TWindowView>
            where TWindowView : IWindowView
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            RegistrationBuilder windowRegistrationBuilder;
            if (windowImplementation is not null)
            {
                windowRegistrationBuilder = builder
                    .Register(windowImplementation, lifetime)
                    .As<TWindowInterface>();
            }
            else
            {
                windowRegistrationBuilder = builder
                    .Register<TWindowModel>(lifetime)
                    .As<TWindowInterface>();
            }

            var presenterRegistrationBuilder = builder.RegisterWindowPresenter<TWindowInterface, TWindowPresenter, TWindowView>(presenterImplementation);
            return (windowRegistrationBuilder, presenterRegistrationBuilder);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (RegistrationBuilder WindowRegistrationBuilder, RegistrationBuilder PresenterRegistrationBuilder)
            RegisterEntryPointWindow<TWindowInterface, TWindowModel, TWindowPresenter, TWindowView>
            (
                this IContainerBuilder builder,
                Func<IObjectResolver, TWindowModel> windowImplementation = null,
                Func<IObjectResolver, TWindowPresenter> presenterImplementation = null,
                Lifetime lifetime = Lifetime.Singleton
            )
            where TWindowInterface : class
            where TWindowModel : TWindowInterface
            where TWindowPresenter : IWindowPresenter<TWindowInterface, TWindowView>
            where TWindowView : IWindowView
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            RegistrationBuilder windowRegistrationBuilder;
            if (windowImplementation is not null)
            {
                windowRegistrationBuilder = builder
                    .RegisterEntryPoint(windowImplementation, lifetime)
                    .As<TWindowInterface>();
            }
            else
            {
                windowRegistrationBuilder = builder
                    .RegisterEntryPoint<TWindowModel>(lifetime)
                    .As<TWindowInterface>();
            }

            var presenterRegistrationBuilder = builder.RegisterWindowPresenter<TWindowInterface, TWindowPresenter, TWindowView>(presenterImplementation);
            return (windowRegistrationBuilder, presenterRegistrationBuilder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (RegistrationBuilder WindowRegistrationBuilder, RegistrationBuilder PresenterRegistrationBuilder)
            RegisterWindow<TWindowModel, TWindowPresenter, TWindowView>
            (
                this IContainerBuilder builder,
                Func<IObjectResolver, TWindowModel> windowImplementation = null,
                Func<IObjectResolver, TWindowPresenter> presenterImplementation = null,
                Lifetime lifetime = Lifetime.Singleton
            )
            where TWindowModel : class
            where TWindowPresenter : IWindowPresenter<TWindowModel, TWindowView>
            where TWindowView : IWindowView
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            RegistrationBuilder windowRegistrationBuilder;
            if (windowImplementation is not null)
            {
                windowRegistrationBuilder = builder
                    .Register(windowImplementation, lifetime)
                    .AsSelf();
            }
            else
            {
                windowRegistrationBuilder = builder
                    .Register<TWindowModel>(lifetime)
                    .AsSelf();
            }

            var presenterRegistrationBuilder = builder.RegisterWindowPresenter<TWindowModel, TWindowPresenter, TWindowView>(presenterImplementation);
            return (windowRegistrationBuilder, presenterRegistrationBuilder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (RegistrationBuilder WindowRegistrationBuilder, RegistrationBuilder PresenterRegistrationBuilder)
            RegisterEntryPointWindow<TWindowModel, TWindowPresenter, TWindowView>
            (
                this IContainerBuilder builder,
                Func<IObjectResolver, TWindowModel> windowImplementation = null,
                Func<IObjectResolver, TWindowPresenter> presenterImplementation = null,
                Lifetime lifetime = Lifetime.Singleton
            )
            where TWindowModel : class
            where TWindowPresenter : IWindowPresenter<TWindowModel, TWindowView>
            where TWindowView : IWindowView
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            RegistrationBuilder windowRegistrationBuilder;
            if (windowImplementation is not null)
            {
                windowRegistrationBuilder = builder
                    .RegisterEntryPoint(windowImplementation, lifetime)
                    .AsSelf();
            }
            else
            {
                windowRegistrationBuilder = builder
                    .RegisterEntryPoint<TWindowModel>(lifetime)
                    .AsSelf();
            }

            var presenterRegistrationBuilder = builder.RegisterWindowPresenter<TWindowModel, TWindowPresenter, TWindowView>(presenterImplementation);
            return (windowRegistrationBuilder, presenterRegistrationBuilder);
        }

        #endregion

        #region Register Presenter

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RegistrationBuilder RegisterWindowPresenter<TWindowModel, TWindowPresenter, TWindowView>
        (
            this IContainerBuilder builder,
            Func<IObjectResolver, TWindowPresenter> implementation = null
        )
            where TWindowModel : class
            where TWindowPresenter : IWindowPresenter<TWindowModel, TWindowView>
            where TWindowView : IWindowView
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            RegistrationBuilder registration;
            if (implementation is not null)
            {
                registration = builder
                    .Register(implementation, Lifetime.Transient)
                    .AsSelf();
            }
            else
            {
                registration = builder
                    .Register<TWindowPresenter>(Lifetime.Transient)
                    .AsSelf();
            }

            builder.RegisterBuildCallback(resolver =>
            {
                if (resolver.TryResolve<IWindowService>(out var windowService) == false)
                    return;

                windowService.Bind<TWindowModel, TWindowPresenter, TWindowView>();
            });

            builder.RegisterDisposeCallback(resolver =>
            {
                if (resolver.TryResolve<IWindowService>(out var windowService) == false)
                    return;

                windowService.Unbind<TWindowModel>();
            });

            return registration;
        }

        #endregion
    }
}