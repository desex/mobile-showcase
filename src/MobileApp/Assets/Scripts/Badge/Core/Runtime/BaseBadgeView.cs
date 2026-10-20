using TMPro;
using UnityEngine;

namespace Sotbify.MobileApp.Badge
{
    public class BaseBadgeView : MonoBehaviour, IBadgeView
    {
        [SerializeField] private int _count;
        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_Text _countText;

        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                UpdateCount(value);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateCount(_count);
        }
#endif

        protected virtual void OnUpdateCount(int count)
        {
        }

        private void Awake()
        {
            UpdateCount(_count);
        }

        private void UpdateCount(int count)
        {
            if(_root != null)
                _root.SetActive(count > 0);
            
            if (_countText != null) 
                _countText.text = count.ToString();
            
            OnUpdateCount(count);
        }
    }
}