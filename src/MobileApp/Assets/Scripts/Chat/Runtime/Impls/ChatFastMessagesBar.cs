using System.Collections.Generic;
using System.Linq;
using R3;
using Sotbify.MobileApp.ChatMessages.Units;
using UnityEngine;

namespace Sotbify.MobileApp.Chat
{
    public class ChatFastMessagesBar : MonoBehaviour
    {
        [SerializeField] private List<NavbarButton> _navbarButtons = new ();

        public Observable<string> FastButtonPressedObservable => _navbarButtons
            .Select(button => button.OnButtonClicked)
            .Merge();
    }
} 