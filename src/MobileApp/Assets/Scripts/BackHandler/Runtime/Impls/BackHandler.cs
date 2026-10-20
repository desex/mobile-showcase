using System;
using R3;
using Sotbify.MobileApp.ApplicationUI;
using Sotbify.MobileApp.Input;
using Sotbify.MobileApp.NexusWindow;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Sotbify.MobileApp.BackHandler
{
    public class BackHandler : IBackHandler, IInitializable, IDisposable
    {
        private readonly InputSystemActions _inputSystemActions;
        private readonly INexus _nexus;
        private readonly IApplicationUI _applicationUI;
        private IDisposable _disposable;

        public BackHandler(
            InputSystemActions inputSystemActions,
            INexus nexus,
            IApplicationUI applicationUI
        )
        {
            _inputSystemActions = inputSystemActions;
            _nexus = nexus;
            _applicationUI = applicationUI;
        }
        
        public void Initialize()
        {
            var backInputAction = _inputSystemActions.UI.GoBack;
            _disposable = Observable.FromEvent<InputAction.CallbackContext>(
                    h => backInputAction.started += h,
                    h => backInputAction.started -= h
                )
                .Subscribe(GoBack);
        }
        
        public void Dispose()
        {
            _disposable?.Dispose();
        }
        
        private void GoBack(InputAction.CallbackContext callbackContext)
        {
            _applicationUI.TryUndo(out _, true);
            _nexus.Undo();
        }
    }
}