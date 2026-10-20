using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TierStudios.UIParticleSystem.Core
{
    [AddComponentMenu("UI/UI Particle Trigger")]
    [DisallowMultipleComponent]
    public class UIParticleTrigger : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private UIParticleSystem target;
        [Tooltip("Play the particle system when this component is enabled.")]
        [SerializeField] private bool playOnEnable = false;
        [Tooltip("Stop emitting when this component is disabled.")]
        [SerializeField] private bool stopOnDisable = false;
        [Tooltip("Play / restart on click. Works with Button.onClick or a Graphic with Raycast Target.")]
        [SerializeField] private bool playOnClick = true;
        [Tooltip("Emit a burst of particles on click.")]
        [SerializeField] private bool burstOnClick = false;
        [SerializeField] private int burstCount = 10;
        [Tooltip("Play when the pointer enters. Requires Raycast Target.")]
        [SerializeField] private bool playOnPointerEnter = false;
        [Tooltip("Stop emitting when the pointer exits.")]
        [SerializeField] private bool stopOnPointerExit = false;

        private Button _button;
        private bool _handlingClick;

        private UIParticleSystem ResolvedTarget
        {
            get
            {
                if (target == null)
                    target = FindTarget();
                return target;
            }
        }

        private void Awake()
        {
            if (target == null)
                target = FindTarget();
        }

        private void OnEnable()
        {
            _button = GetComponent<Button>();
            if (_button != null)
                _button.onClick.AddListener(HandleClick);

            if (playOnEnable)
                Play();
        }

        private void OnDisable()
        {
            if (_button != null)
                _button.onClick.RemoveListener(HandleClick);

            if (stopOnDisable)
                Stop();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_button != null)
                return;

            HandleClick();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (playOnPointerEnter)
                Play();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (stopOnPointerExit)
                Stop();
        }

        public void Play()
        {
            ResolvedTarget?.StartEmitting();
        }

        public void Restart()
        {
            ResolvedTarget?.RestartSystem();
        }

        public void Stop()
        {
            ResolvedTarget?.StopEmitting();
        }

        public void Burst()
        {
            var system = ResolvedTarget;
            if (system == null)
                return;

            system.EmitBurst(Mathf.Max(0, burstCount));
        }

        private void HandleClick()
        {
            if (_handlingClick)
                return;

            _handlingClick = true;
            try
            {
                if (playOnClick)
                    Restart();
                if (burstOnClick)
                    Burst();
            }
            finally
            {
                _handlingClick = false;
            }
        }

        private UIParticleSystem FindTarget()
        {
            var self = GetComponent<UIParticleSystem>();
            if (self != null)
                return self;

            var child = GetComponentInChildren<UIParticleSystem>(true);
            if (child != null)
                return child;

            return GetComponentInParent<UIParticleSystem>();
        }
    }
}
