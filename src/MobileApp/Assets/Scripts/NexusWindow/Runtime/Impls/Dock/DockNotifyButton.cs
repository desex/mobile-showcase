using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.NexusWindow.Dock
{
    public class DockNotifyButton : MonoBehaviour
    {
        [SerializeField] private GameObject _notifyBoxRoot;
        [SerializeField] private TMP_Text _notifyText;
        [field: SerializeField] public Button Button { get; private set; }
        
        private int _notifyCount;

        public int NotifyCount
        {
            get => _notifyCount;
            set
            {
                _notifyBoxRoot.SetActive(value > 0);
                _notifyCount = value;
                _notifyText.text = $"+{value}";
            }
        }
    }
}