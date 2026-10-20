using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using Gyroscope = UnityEngine.InputSystem.Gyroscope;

namespace Sotbify.MobileApp.UI.Common
{
    public class PanoramaGyroCommon : MonoBehaviour
    {
        [SerializeField] private Transform _gyroObject;
        [SerializeField] private float _maxAngle = 10f;
        [SerializeField] private float _inputAngleRange = 30f;
        [SerializeField] private float _smoothSpeed = 5f;
        [SerializeField] private bool _invertX;
        [SerializeField] private bool _invertY;

        private Quaternion _initialRotation;

        private void Awake()
        {
            if (_gyroObject == null)
                _gyroObject = transform;

            _initialRotation = _gyroObject.localRotation;
        }
        
        private void Update()
        {
            if (_gyroObject == null)
                return;

            var targetRotation = GetTargetRotation();
            var lerpFactor = 1f - Mathf.Exp(-_smoothSpeed * Time.deltaTime);
            _gyroObject.localRotation = Quaternion.Slerp(_gyroObject.localRotation, targetRotation, lerpFactor);
        }

        private Quaternion GetTargetRotation()
        {
            var gyroRotation = GetRotation();
            var gyroEuler = gyroRotation.eulerAngles;
            var inputAngleRange = Mathf.Max(0.001f, _inputAngleRange);

            var x = Mathf.Clamp(Mathf.DeltaAngle(0f, gyroEuler.x) / inputAngleRange, -1f, 1f) * _maxAngle;
            var y = Mathf.Clamp(Mathf.DeltaAngle(0f, gyroEuler.y) / inputAngleRange, -1f, 1f) * _maxAngle;

            if (_invertX)
                x = -x;

            if (_invertY)
                y = -y;

            return _initialRotation * Quaternion.Euler(x, y, 0f);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Quaternion GetRotation()
        {
            var attitudeRotation = GetAttitudeRotation();
            var screenRotationCorrection = GetScreenRotationCorrection();
            return screenRotationCorrection * attitudeRotation;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Quaternion GetAttitudeRotation()
        {
#if UNITY_EDITOR
            return Quaternion.identity;
#endif
            
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (Gyroscope.current != null && Gyroscope.current.enabled == false) 
                InputSystem.EnableDevice(Gyroscope.current); 

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (AttitudeSensor.current != null && AttitudeSensor.current.enabled == false) 
                InputSystem.EnableDevice(AttitudeSensor.current);

            if (AttitudeSensor.current == null)
                return Quaternion.identity;
            
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