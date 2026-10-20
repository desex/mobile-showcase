using R3;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Common.Components.DropDown
{
    public sealed class DropDownsGroup : MonoBehaviour
    {
        [SerializeField] private bool _onlyOneActive = true;
        [SerializeField] private bool _hideAllOnDisable = true;
        
        private DropDown[] _dropDowns;

        private void Awake()
        {
            _dropDowns = GetComponentsInChildren<DropDown>();
        }

        private void Start()
        {
            foreach (var dropDown in _dropDowns)
            {
                dropDown.IsEnabled
                    .Where(isEnabled => isEnabled)
                    .Subscribe(this, (isEnabled, group) => group.OnDropDownEnabled(dropDown))
                    .AddTo(this);
            }
        }

        private void OnDisable()
        {
            if (_hideAllOnDisable == false)
            {
                return;
            }

            HideAll();
        }

        private void OnDropDownEnabled(DropDown activeDropDown)
        {
            if (_onlyOneActive == false)
            {
                return;
            }

            foreach (var dropDown in _dropDowns)
            {
                dropDown.IsEnabled.Value = dropDown == activeDropDown;
            }
        }
        
        private void HideAll()
        {
            foreach (var dropDown in _dropDowns)
            {
                dropDown.IsEnabled.Value = false;
            }
        }
    }
}