using LitMotion.Animation;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Sotbify.MobileApp.Chat
{
    public class ManagerInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text _managerName;
        [SerializeField] private TMP_Text _managerStatus;
        [SerializeField] private Image _managerStatusImage;
        [SerializeField] private Image _managerImage;
        
        [SerializeField] private LitMotionAnimation _managerLoadingAnimation;
        [SerializeField] private Color _loadingColor;
        [SerializeField] private Color _onlineColor;
        [SerializeField] private Color _offlineColor;
        
        [SerializeField] private LocalizedString _onlineLocalizedString;
        [SerializeField] private LocalizedString _offlineLocalizedString;
        
        public string ManagerName
        {
            set => _managerName.SetText(value);
        }

        public ENetworkStatus ManagerStatus
        {
            set
            {
                switch (value)
                {
                    case ENetworkStatus.Online:
                        _managerStatus.color = _onlineColor;
                        _managerStatusImage.color = _onlineColor;

                        _managerStatus.SetText(_onlineLocalizedString.GetLocalizedString());
                        break;
                    
                    case ENetworkStatus.None:
                    case ENetworkStatus.Offline:
                        _managerStatus.color = _offlineColor;
                        _managerStatusImage.color = _offlineColor;
                        
                        _managerStatus.SetText(_offlineLocalizedString.GetLocalizedString());
                        break;
                }
            }
        }

        public Sprite ManagerImage
        {
            set => _managerImage.sprite = value;
        }

        public bool IsThumbnailLoading
        {
            set
            {
                _managerLoadingAnimation.gameObject.SetActive(value);
                _managerImage.color = value ? _loadingColor : Color.white;
                _managerImage.sprite = value ? null : _managerImage.sprite;
            }
        }

        public void PlayLoadingAnimation()
        {
            _managerLoadingAnimation.Play();
        }
        
    }
}