using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.ChatMessages;
using Sotbify.MobileApp.ChatMessages.Units;
using Sotbify.MobileApp.Db.ChatParameters;
using Sotbify.MobileApp.UI.Window.View;
using Sotbify.MobileApp.Utils.Extensions;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Stab.StabChatWindow
{
    public class StabChatView : FadeWindowView
    {
        [SerializeField] private VerticalLayoutGroup _chatLayoutGroup;
        [SerializeField] private GameObject _quickMessagesBar;
        
        [SerializeField] private List<NavbarButton> _fastMessageButtons = new();
        [SerializeField] private int _initialBottomPadding;
        
        private IChatParameters _chatParameters;
        private float _canvasHeight;

        [field: SerializeField] public MessagesListView MessagesListView { get; private set; }
        public IReadOnlyList<NavbarButton> FastMessageButtons => _fastMessageButtons;

        [Inject]
        public void Construct(IChatParameters chatParameters)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        protected override UniTask ShowInternalAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ChangeQuickMessagePanelVisibility(bool isVisible)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void MoveInput(int keyboardHeight)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}