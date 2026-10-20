using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.HomeWindow
{
    public sealed class ItemView : MonoBehaviour
    {
        [SerializeField] private RawImage _itemImage;
        [SerializeField] private Graphic _backgroundGraphic;
        [SerializeField] private bool _isActive = false;
        [SerializeField] private List<GameObject> _activeGameObjectsIfActive;
        [SerializeField] private List<GameObject> _inactiveGameObjectsIfActive;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                SetGameObjectsActive(_activeGameObjectsIfActive, value);
                SetGameObjectsActive(_inactiveGameObjectsIfActive, value == false);
            }
        }

        public Texture ItemTexture
        {
            get => _itemImage != null ? _itemImage.texture : null;
            set
            {
                if(_itemImage != null)
                    _itemImage.texture = value;
                
                SetGameObjectsActive(_activeGameObjectsIfActive, value != null);
                SetGameObjectsActive(_inactiveGameObjectsIfActive, value == null);
            }
        }

        public Color ItemBackgroundColor
        {
            get => _backgroundGraphic != null ? _backgroundGraphic.color : Color.clear;
            set
            {
                if (_backgroundGraphic != null)
                    _backgroundGraphic.color = value;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetGameObjectsActive(_activeGameObjectsIfActive, _isActive);
            SetGameObjectsActive(_inactiveGameObjectsIfActive, _isActive == false);
        }
#endif

        private void Awake()
        {
            SetGameObjectsActive(_activeGameObjectsIfActive, _isActive);
            SetGameObjectsActive(_inactiveGameObjectsIfActive, _isActive == false);
        }

        private static void SetGameObjectsActive(IEnumerable<GameObject> gameObjects, bool isActive)
        {
            foreach (var gObject in gameObjects)
            {
                if(gObject != null)
                    gObject.SetActive(isActive);
            }
        }
    }
}