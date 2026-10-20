using System;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.ChatMessages.Models;
using Sotbify.MobileApp.Plugins.KeyboardHeight;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;
#if UNITY_ANDROID || UNITY_IOS
using UnityEngine;
#endif

namespace Stab.StabChatWindow
{
    public class StabChat : IStabChat, IDisposable
    {
        private readonly IStabMessagesRepository _messagesRepository;
        private readonly ILogger _logger;
        private readonly Subject<int> _moveChatCommand = new();
        private readonly Subject<bool> _changeQuickMessagePanelVisibilityCommand = new();

        private int? _keyboardSize = null;
        
        public StabChat(
            IStabMessagesRepository messagesRepository,
            ILogger logger
        )
        {
            _messagesRepository = messagesRepository ?? throw new ArgumentNullException(nameof(messagesRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
            KeyboardHeight.Register();
            KeyboardHeight.HeightChanged += OnKeyboardHeightChanged;
#endif
        }

        public IReadOnlyObservableList<UserMessageModel> Messages => _messagesRepository.Messages;
        public Observable<int> MoveChatObservable => _moveChatCommand;

        public Observable<bool> ChangeQuickMessagePanelVisibilityObservable =>
            _changeQuickMessagePanelVisibilityCommand;

        public void SendMessage(string viewMessage)
        {
            if(string.IsNullOrEmpty(viewMessage))
                throw new ArgumentException("Can not be null or empty", nameof(viewMessage));
            
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Pending message: {viewMessage}", context: this);
            
            var messageData = new UserMessageModel
            {
                ID = int.MaxValue,
                Message = viewMessage,
                CreatedAt = DateTime.Now,
                FreezeEndEdge = true,
                KeepVelocityOnCountChange = true
            };
            
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Message sent: {viewMessage}", context: this);
            
            _messagesRepository.AddToEnd(messageData);
        }
        
        public async UniTask MoveChat(bool isMoveUpwards)
        {
            await UniTask.WaitForEndOfFrame();
            
            _changeQuickMessagePanelVisibilityCommand?.OnNext(!isMoveUpwards);
            if (!isMoveUpwards)
            {
                _moveChatCommand?.OnNext(0);
                return;
            }
            
            var keyboardSize = CalculateKeyboardSize();
            if (keyboardSize > 0)
            {
                _keyboardSize = keyboardSize;
            }

            _moveChatCommand?.OnNext(_keyboardSize ?? 0);
        }

        public void Clear()
        {
            _messagesRepository.Clear();
        }

        public void Dispose()
        {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
            KeyboardHeight.HeightChanged -= OnKeyboardHeightChanged;
            KeyboardHeight.Unregister();
#endif
            _moveChatCommand?.Dispose();
            _changeQuickMessagePanelVisibilityCommand?.Dispose();
        }
        
        private int CalculateKeyboardSize()
        {
#if UNITY_ANDROID || UNITY_IOS
            return KeyboardHeight.GetHeight();
#else
            return 0;
#endif
        }

#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        private void OnKeyboardHeightChanged(int height)
        {
            if (height > 0)
            {
                _keyboardSize = height;
            }
            else
            {
                _keyboardSize = null;
            }

            _changeQuickMessagePanelVisibilityCommand?.OnNext(height <= 0);
            _moveChatCommand?.OnNext(height);
        }
#endif
    }
}