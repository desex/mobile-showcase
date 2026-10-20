using System;
using System.Collections.Concurrent;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Assets;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.UI.Window.Presenter;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using UnityEngine.Pool;
using VContainer.Unity;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.UI.Window
{
    public sealed class WindowService : IWindowService, IDisposable
    {
        #region Internal Types

        private readonly struct BindInfo
        {
            public BindInfo(Type windowType, Type windowPresenterType, Type windowViewType)
            {
                WindowType = windowType ?? throw new ArgumentNullException(nameof(windowType));
                WindowPresenterType = windowPresenterType ?? throw new ArgumentNullException(nameof(windowPresenterType));
                WindowViewType = windowViewType ?? throw new ArgumentNullException(nameof(windowViewType));
            }

            public Type WindowType { get; }
            public Type WindowPresenterType { get; }
            public Type WindowViewType { get; }
        }

        private readonly struct WindowActivation
        {
            public WindowActivation(object window, IWindowPresenter presenter, GameObject view) : this(window, presenter, view, Guid.NewGuid())
            {
            }

            public WindowActivation(object window, IWindowPresenter presenter, GameObject view, Guid entryId)
            {
                Window = window ?? throw new ArgumentNullException(nameof(window));
                Presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
                View = view ?? throw new ArgumentNullException(nameof(view));
                EntryId = entryId;
            }

            public object Window { get; }
            public IWindowPresenter Presenter { get; }
            public GameObject View { get; }
            public Guid EntryId { get; }
        }

        #endregion

        private readonly ConcurrentDictionary<Type, BindInfo> _bindings = new();
        private readonly ConcurrentDictionary<Type, WindowActivation> _activeWindows = new();

        private readonly LifetimeScope _parentScope;
        private readonly IAssetsProvider _assetsProvider;
        private readonly ILogger _logger;
        private readonly Transform _parent;

        public WindowService
        (
            LifetimeScope parentScope,
            IAssetsProvider assetsProvider,
            ILogger logger,
            Transform parent
        )
        {
            _parentScope = parentScope ?? throw new ArgumentNullException(nameof(parentScope));
            _assetsProvider = assetsProvider ?? throw new ArgumentNullException(nameof(assetsProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _parent = parent;
        }

        public event Action<Type> Opened;
        public event Action<Type> Closed;

        public void Bind<TWindowModel, TWindowPresenter, TWindowView>()
            where TWindowModel : class
            where TWindowPresenter : IWindowPresenter<TWindowModel, TWindowView>
            where TWindowView : IWindowView =>
            BindInternal(typeof(TWindowModel), typeof(TWindowPresenter), typeof(TWindowView));

        public void Unbind<TWindowModel>() where TWindowModel : class =>
            UnbindInternal(typeof(TWindowModel));

        public bool IsOpen<TWindowModel>() where TWindowModel : class =>
            _activeWindows.ContainsKey(typeof(TWindowModel));

        public UniTask<TWindowModel> OpenAsync<TWindowModel>(CancellationToken cancellationToken = default) where TWindowModel : class =>
            OpenAsync<TWindowModel>(_parent, false, cancellationToken);

        public UniTask<TWindowModel> OpenAsync<TWindowModel>
        (
            Transform parent,
            bool instantiateInWorldSpace = false,
            CancellationToken cancellationToken = default
        ) where TWindowModel : class =>
            OpenInternalAsync(typeof(TWindowModel), parent, instantiateInWorldSpace, cancellationToken)
                .ContinueWith(windowModel => UniTask.FromResult((TWindowModel)windowModel));

        public GameObject GetView<TWindowModel>() where TWindowModel : class
        {
            var windowModelType = typeof(TWindowModel);

            if (_activeWindows.TryGetValue(windowModelType, out var result) == false)
                throw new InvalidOperationException($"Not found in active windows '{windowModelType.Name}'");

            return result.View != null
                ? result.View
                : throw new NullReferenceException($"View for window '{windowModelType.Name}' is null");
        }

        public UniTask CloseAsync<TWindowModel>(CancellationToken cancellationToken = default) where TWindowModel : class =>
            CloseInternalAsync(typeof(TWindowModel), cancellationToken);

        public void Dispose()
        {
            using (ListPool<Type>.Get(out var windowModelTypes))
            {
                windowModelTypes.AddRange(_bindings.Keys);

                foreach (var windowModelType in windowModelTypes)
                    UnbindInternal(windowModelType);
            }
        }

        private void BindInternal(Type windowModelType, Type windowPresenterType, Type windowViewType)
        {
            if (_bindings.ContainsKey(windowModelType))
                throw new InvalidOperationException($"Window '{windowModelType.Name}' already bound");

            var bindInfo = new BindInfo(windowModelType, windowPresenterType, windowViewType);
            if (_bindings.TryAdd(windowModelType, bindInfo) == false)
                throw new InvalidOperationException($"Failed to bind window '{windowModelType.Name}'. Could not add binding");

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Successfully bound window '{windowModelType.Name}'", context: this);
        }

        private void UnbindInternal(Type windowModelType)
        {
            if (_bindings.ContainsKey(windowModelType) == false)
                throw new InvalidOperationException($"Window '{windowModelType.Name}' is not bound");

            if (_activeWindows.TryRemove(windowModelType, out var windowActivation) == false)
                return;

            windowActivation.Presenter.Dispose();

            if (windowActivation.View != null)
                _assetsProvider.ReleaseInstance(windowActivation.View);

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Successfully unbound window '{windowModelType.Name}'", context: this);
        }

        private async UniTask<object> OpenInternalAsync
        (
            Type windowModelType,
            Transform parent,
            bool instantiateInWorldSpace,
            CancellationToken cancellationToken
        )
        {
            if (_activeWindows.TryGetValue(windowModelType, out var activeWindow))
                return activeWindow.Window;

            if (_bindings.TryGetValue(windowModelType, out var bindInfo) == false)
                throw new InvalidOperationException($"Binding for {windowModelType.Name} not found");

            using (LifetimeScope.EnqueueParent(_parentScope))
            {
                var objectResolver = _parentScope.Container;
                var windowModel = objectResolver.Resolve(bindInfo.WindowType);
                var windowPresenter = (IWindowPresenter)objectResolver.Resolve(bindInfo.WindowPresenterType);

                GameObject windowViewGameObject = null;
                var entryId = Guid.Empty;

                try
                {
                    var windowViewKey = bindInfo.WindowViewType.Name;
                    windowViewGameObject = await _assetsProvider.InstantiateAsync(windowViewKey, parent, instantiateInWorldSpace, cancellationToken: cancellationToken);
                    objectResolver.InjectGameObject(windowViewGameObject);

                    var windowActivation = new WindowActivation(windowModel, windowPresenter, windowViewGameObject);
                    entryId = windowActivation.EntryId;

                    if (_activeWindows.TryAdd(windowModelType, windowActivation) == false)
                        throw new InvalidOperationException($"Unable to open the window '{windowModelType.Name}'. Failed to add to active windows");

                    if (windowViewGameObject.TryGetComponent(bindInfo.WindowViewType, out var view) == false)
                        throw new InvalidOperationException(
                            $"Cannot find component of type '{bindInfo.WindowViewType.Name}' for key '{windowViewKey}'");

                    windowPresenter.BindModel(windowModel);
                    windowPresenter.BindView((IWindowView)view);
                    windowPresenter.Initialize();

                    await windowPresenter.ShowAsync(cancellationToken);

                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.Debug($"Successfully opened window '{windowModelType.Name}'", context: this);

                    Opened?.Invoke(windowModelType);
                    return windowModel;
                }
                catch
                {
                    if (entryId != Guid.Empty && _activeWindows.TryGetValue(windowModelType, out var current) && current.EntryId == entryId)
                        _activeWindows.TryRemove(windowModelType, out _);

                    windowPresenter.Dispose();

                    if (windowViewGameObject != null)
                        _assetsProvider.ReleaseInstance(windowViewGameObject);

                    throw;
                }
            }
        }

        private async UniTask CloseInternalAsync(Type windowModelType, CancellationToken cancellationToken = default)
        {
            if (_activeWindows.ContainsKey(windowModelType) == false)
                throw new InvalidOperationException($"Failed to close window '{windowModelType.Name}'. Not found in active windows");

            if (_activeWindows.TryRemove(windowModelType, out var windowActivation) == false)
                throw new InvalidOperationException($"Failed to close the window '{windowModelType.Name}'. Removal from active windows failed");

            var windowPresenter = windowActivation.Presenter;
            try
            {
                await windowPresenter.HideAsync(cancellationToken);
            }
            finally
            {
                windowPresenter.Dispose();
                _assetsProvider.ReleaseInstance(windowActivation.View);
            }

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Successfully closed window '{windowModelType.Name}'", context: this);

            Closed?.Invoke(windowModelType);
        }
    }
}