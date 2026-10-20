using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Common.Components.StateTracker
{
    [Serializable]
    public class StatePropertyBlock<TValue>
    {
        [SerializeField] private TValue _normal;

        [SerializeField] private bool _useHover = true;
        [ShowIf(nameof(_useHover))]
        [SerializeField] private TValue _hover;
        
        [SerializeField] private bool _usePressed = true;
        [ShowIf(nameof(_usePressed))]
        [SerializeField] private TValue _pressed;
        
        [SerializeField] private TValue _disabled;
        
        public TValue Normal
        {
            get => _normal;
            set => _normal = value;
        }

        public bool UseHover
        {
            get => _useHover;
            set => _useHover = value;
        }

        public TValue Hover
        {
            get => _hover;
            set => _hover = value;
        }

        public bool UsePressed
        {
            get => _usePressed;
            set => _usePressed = value;
        }

        public TValue Pressed
        {
            get => _pressed;
            set => _pressed = value;
        }

        public TValue Disabled
        {
            get => _disabled;
            set => _disabled = value;
        }
    }
}