using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Gyroscope = UnityEngine.InputSystem.Gyroscope;

#nullable disable
namespace Sotbify.MobileApp.UI.Common
{
    public sealed class PanoramaGyroImage : MonoBehaviour
    {
        private const string FovGroup = "FOV";
        private const string AspectGroup = "Aspect";
        private const string ViewRotationGroup = "View Rotation";

        [SerializeField] private Graphic _graphic;

        [BoxGroup(FovGroup)] 
        [SerializeField] private float _fov = 60f;
        [BoxGroup(FovGroup)]
        [SerializeField] private string _fovParameterName = "_FOV";

        [BoxGroup(AspectGroup)] 
        [SerializeField] private float _aspect = 1f;
        [BoxGroup(AspectGroup)]
        [SerializeField] private string _aspectParameterName = "_Aspect";

        [BoxGroup(ViewRotationGroup)]
        [SerializeField] private Vector3 _initialRotation = Vector3.zero;
        [BoxGroup(ViewRotationGroup)]
        [SerializeField] private string _viewRotationParameterName = "_ViewRotation";

        public float Fov
        {
            get => _fov;
            set
            {
                _fov = value;

                if (TryGetMaterial(out var material))
                    material.SetFloat(_fovParameterName, value);
            }
        }

        public float Aspect
        {
            get => _aspect;
            set
            {
                _aspect = value;

                if (TryGetMaterial(out var material))
                    material.SetFloat(_aspectParameterName, value);
            }
        }

        public Vector3 InitialRotation
        {
            get => _initialRotation;
            set => _initialRotation = value;
        }

        #region Editor

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (TryGetMaterial(out var material) == false)
                return;

            material.SetFloat(_fovParameterName, _fov);
            material.SetFloat(_aspectParameterName, _aspect);

            var rotation = GetRotation(Quaternion.Euler(_initialRotation));
            material.SetVector(_viewRotationParameterName, rotation);
        }

#endif

        #endregion

        private void Start()
        {
            if (TryGetMaterial(out var material) == false)
                return;

            material.SetFloat(_fovParameterName, _fov);
            material.SetFloat(_aspectParameterName, _aspect);
        }

        private void Update()
        {
            if (TryGetMaterial(out var material) == false)
                return;
            
            var rotation = GetRotation(Quaternion.Euler(_initialRotation));
            material.SetVector(_viewRotationParameterName, rotation);
        }

        private bool TryGetMaterial(out Material material)
        {
            material = null;
            if (_graphic == null)
                return false;

            material = _graphic.materialForRendering;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector4 GetRotation(Quaternion initialRotation)
        {
            var attitudeRotation = GetAttitudeRotation();
            var screenRotationCorrection = GetScreenRotationCorrection();
            var rotation = initialRotation * screenRotationCorrection * attitudeRotation;
            return new Vector4(rotation.x, rotation.y, rotation.z, rotation.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Quaternion GetAttitudeRotation()
        {
#if UNITY_EDITOR
            return Quaternion.identity;
#endif
            
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (Gyroscope.current == null || Gyroscope.current.enabled == false) 
                InputSystem.EnableDevice(Gyroscope.current); 

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (AttitudeSensor.current == null || AttitudeSensor.current.enabled == false) 
                InputSystem.EnableDevice(AttitudeSensor.current);
            
            var attitudeRotation = AttitudeSensor.current.attitude.ReadValue();
            return new Quaternion(attitudeRotation.x, attitudeRotation.y, -attitudeRotation.z, -attitudeRotation.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Quaternion GetScreenRotationCorrection()
        {
#if UNITY_EDITOR
            return Quaternion.identity;
#endif
            
            return Screen.orientation switch
            {
                ScreenOrientation.LandscapeLeft => Quaternion.Euler(90f, 0f, -90f),
                ScreenOrientation.LandscapeRight => Quaternion.Euler(90f, 0f, 90f),
                ScreenOrientation.PortraitUpsideDown => Quaternion.Euler(90f, 0f, 180f),
                _ => Quaternion.Euler(90f, 0f, 0f)
            };
        }
    }
}