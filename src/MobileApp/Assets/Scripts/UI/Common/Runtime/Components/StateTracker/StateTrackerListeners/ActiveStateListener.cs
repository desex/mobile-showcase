using Sirenix.OdinInspector;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Common.Components.StateTracker.StateTrackerListeners
{
    public sealed class ActiveStateListener : StateTrackerListener<bool>
    {
        [Required]
        [SerializeField] private GameObject _gameObject;
        
        protected override void StateChangedInternal(StateTracker.State state, bool value)
        {
            if (_gameObject == null)
                return;
            
            _gameObject.SetActive(value);
        }
    }
}