using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.UI.Common.Components
{
    public sealed class SpriteWithLoader : MonoBehaviour
    {
        [SerializeField] private bool _allowNullTexture;
        [SerializeField] private Image _image;
        [SerializeField] private List<GameObject> _gameObjectsActiveIfLoaded;
        [SerializeField] private List<GameObject> _gameObjectsActiveIfNotLoaded;
        [SerializeField] private bool _useAspectRatio;
        [SerializeField, ShowIf(nameof(_useAspectRatio))]
        private AspectRatioFitter _aspectRatioFitter;

        public Sprite Sprite
        {
            get => _image.sprite;
            set => SetSprite(value);
        }

        #region Editor

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (_image == null)
                return;

            SetSprite(_image.sprite);
        }

#endif

        #endregion

        private void Awake()
        {
            if (_image == null)
                return;

            SetSprite(_image.sprite);
        }

        private void SetSprite(Sprite value)
        {
            var hasTexture = value != null;

            if (hasTexture || _allowNullTexture)
                _image.sprite = value;

            SetActiveGameObjects(_gameObjectsActiveIfLoaded, hasTexture);
            SetActiveGameObjects(_gameObjectsActiveIfNotLoaded, hasTexture == false);

            if (hasTexture && _useAspectRatio)
                _aspectRatioFitter.aspectRatio = (float)value.texture.width / value.texture.height;
        }

        private static void SetActiveGameObjects(IEnumerable<GameObject> gameObjects, bool isActive)
        {
            foreach (var gameObject in gameObjects)
            {
                if (gameObject == null)
                    continue;

                gameObject.SetActive(isActive);
            }
        }
    }
}