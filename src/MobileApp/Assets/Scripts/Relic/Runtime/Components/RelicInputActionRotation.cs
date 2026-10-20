using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sotbify.MobileApp.Relic
{
    public sealed class RelicInputActionRotation : MonoBehaviour
    {
        private const string InputGroup = "Input";
        private const string SpringGroup = "Spring";

        [SerializeField] private Transform _target;

        [BoxGroup(InputGroup)] 
        [SerializeField] private InputActionProperty _action;
        [BoxGroup(InputGroup)]
        [SerializeField, Min(0f)] private float _deadZone = 0.05f;
        [BoxGroup(InputGroup)] 
        [SerializeField] private Vector3 _anglePerG = new(15f, 15f, 0f);
        [BoxGroup(InputGroup)]
        [SerializeField, Min(0f)] private float _maxAngle = 25f;
        [BoxGroup(InputGroup)]
        [SerializeField, Min(0f)] private float _strength = 3f;
        
        [BoxGroup(SpringGroup)]
        [SerializeField, Range(0.5f, 10f)] private float _frequency = 4f;
        [BoxGroup(SpringGroup)] 
        [SerializeField, Range(0f, 1f)] private float _damping = 0.3f;

        private Quaternion _restRotation;
        private Vector3 _angles;
        private Vector3 _angularVelocity;

        private void OnEnable()
        {
            _restRotation = _target.localRotation;
        }

        private void OnDisable()
        {
            _target.localRotation = _restRotation;
        }

        private void Update()
        {
            var deltaTime = Time.unscaledDeltaTime;
            var targetAngles = GetTargetAngles();

            var omega = 2f * Mathf.PI * _frequency;
            var k = omega * omega;
            var c = 2f * _damping * omega;

            _angularVelocity =
                (_angularVelocity + k * deltaTime * (targetAngles - _angles))
                / (1f + c * deltaTime + k * deltaTime * deltaTime);

            _angles += _angularVelocity * deltaTime;

            _target.localRotation = _restRotation * Quaternion.Euler(_angles);
        }

        private Vector3 GetTargetAngles()
        {
            var acceleration = GetAcceleration();
            var angles = new Vector3
            (
                acceleration.y * _anglePerG.x,
                -acceleration.x * _anglePerG.y,
                acceleration.x * _anglePerG.z
            );

            return Vector3.ClampMagnitude(angles, _maxAngle);
        }

        private Vector3 GetAcceleration()
        {
            var action = GetInputActionOrNull();
            if (action is null)
                return Vector3.zero;

            var acceleration = action.ReadValue<Vector3>();
            var magnitude = acceleration.magnitude;

            if (magnitude <= _deadZone)
                return Vector3.zero;

            return acceleration / magnitude * (magnitude - _deadZone) * _strength;
        }

        private InputAction GetInputActionOrNull()
        {
            InputAction action;

            if (_action.action is not null)
            {
                action = _action.action;
            }
            else
            {
                var actionReference = _action.reference;
                action = actionReference?.action;
            }
            
            if(action is not null && action.enabled == false)
                action.Enable();

            return action;
        }
    }
}