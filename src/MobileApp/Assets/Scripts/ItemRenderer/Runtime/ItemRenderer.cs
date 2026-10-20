using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Sotbify.MobileApp.ItemRenderer
{
    public sealed class ItemRenderer : MonoBehaviour, IItemRenderer
    {
        private const string TextureGroup = "Texture";

        [Required] [SerializeField] private Camera _camera;
        [Required] [SerializeField] private Transform _container;
        [SerializeField] private LayerMask _layerMask;

        [BoxGroup(TextureGroup)] 
        [SerializeField] private Vector2Int _resolution = new(512, 512);
        [BoxGroup(TextureGroup), ValueDropdown(nameof(AntialiasingDropdown))]
        [SerializeField] private int _antiAliasing = 1;
        [BoxGroup(TextureGroup)]
        [SerializeField] private GraphicsFormat _colorFormat = GraphicsFormat.R16G16B16A16_SFloat;
        [BoxGroup(TextureGroup)]
        [SerializeField] private GraphicsFormat _depthStencilFormat = GraphicsFormat.D16_UNorm;

        private RenderTexture _renderTexture;
        
        public Vector2Int Resolution
        {
            get => _resolution;
            set
            {
                var width = Mathf.Max(32, value.x);
                var height = Mathf.Max(32, value.y);
                _resolution = new Vector2Int(width, height);

                if (_renderTexture != null)
                {
                    var tempCameraState = _camera != null && _camera.enabled;
                    
                    _renderTexture = ResizeRenderTexture(_renderTexture, _resolution);
                    
                    if(_camera != null)
                        _camera.enabled = tempCameraState;
                }
            }
        }

        // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
        public RenderTexture Texture => _renderTexture;
        public Transform Container => _container != null ? _container : transform;

        public bool RenderingIsActive
        {
            get => _camera != null && _camera.enabled;
            set
            {
                if (_camera != null)
                    _camera.enabled = value;
            }
        }

        private void Awake()
        {
            if (_camera == null)
                return;

            _renderTexture = CreateRenderTexture(_resolution, _antiAliasing, _colorFormat, _depthStencilFormat);
            _camera.targetTexture = _renderTexture;
        }

        private void Start()
        {
            if(_renderTexture != null)
                _renderTexture.name = $"{name}_RenderTexture";
        }

        private void OnDestroy()
        {
            if (_renderTexture != null)
            {
                Destroy(_renderTexture);
                _renderTexture = null;
            }

            if (_camera != null)
                _camera.targetTexture = null;
        }

        private static RenderTexture ResizeRenderTexture(RenderTexture renderTexture, Vector2Int resolution)
        {
            renderTexture.Release();
            renderTexture.width = resolution.x;
            renderTexture.height = resolution.y;
            renderTexture.Create();
            
            return renderTexture;
        }

        private static RenderTexture CreateRenderTexture
        (
            Vector2Int resolution,
            int antiAliasing,
            GraphicsFormat colorFormat,
            GraphicsFormat depthStencilFormat
        )
        {
            return new RenderTexture(resolution.x, resolution.y, colorFormat, depthStencilFormat)
            {
                antiAliasing = antiAliasing,
            };
        }

        #region Editor

        private IEnumerable<ValueDropdownItem<int>> AntialiasingDropdown()
        {
            yield return new ValueDropdownItem<int>("None", 1);
            yield return new ValueDropdownItem<int>("2 sample", 2);
            yield return new ValueDropdownItem<int>("4 sample", 4);
            yield return new ValueDropdownItem<int>("8 sample", 8);
        }

        #endregion
    }
}