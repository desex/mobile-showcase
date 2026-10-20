using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sotbify.MobileApp.UI.Common.ParticleSystem
{
    [ExecuteAlways]
    [RequireComponent(typeof(UnityEngine.ParticleSystem))]
    public sealed class RadialParticleRotation : MonoBehaviour
    {
        [SerializeField]
        private bool updateInEditMode = true;

        [Tooltip("Включи, если штрихи должны быть направлены к центру круга.")]
        [SerializeField]
        private bool pointInside;

        [Tooltip("Инвертирует экранную Y-ось при расчёте roll. Полезно, если верх/низ направлены противоположно боковым сторонам.")]
        [SerializeField]
        private bool invertScreenY = true;

        [Tooltip("Камера, относительно которой billboard остаётся лицом к экрану. Если не задана, используется Camera.main, а в Edit Mode — SceneView camera.")]
        [SerializeField]
        private Camera cameraOverride;

        [Tooltip("Дополнительный поворот, если исходная текстура/mesh полоски требует ручной подстройки.")]
        [SerializeField]
        private float rotationOffsetDegrees;

        [Tooltip("Автоматически выставляет ParticleSystem/Renderer в режим, где billboard roll должен учитываться.")]
        [SerializeField]
        private bool forceBillboardRollSettings = true;

        [Tooltip("Диагностика: принудительно задаёт всем частицам один угол, чтобы проверить, влияет ли rotation на Renderer.")]
        [SerializeField]
        private bool debugForceRotation;

        [SerializeField]
        private float debugRotationDegrees;

        [Header("Debug State")]
        [SerializeField]
        private int lastParticlesCount;

        [SerializeField]
        private int lastAppliedParticlesCount;

        [SerializeField]
        private bool lastSetParticlesCalled;

        [SerializeField]
        private string lastCameraName;

        private const float BillboardUpAxisToRadialOffsetDegrees = -90f;
        private const float MinRadialLengthSqr = 0.000001f;

        private UnityEngine.ParticleSystem particleSystemComponent;
        private ParticleSystemRenderer particleSystemRendererComponent;
        private UnityEngine.ParticleSystem.Particle[] particles;
        private Camera cachedMainCamera;

        private void OnEnable()
        {
            Initialize();

    #if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
            EditorApplication.update += EditorUpdate;
    #endif
        }

        private void OnDisable()
        {
    #if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
    #endif
        }

        private void OnValidate()
        {
            Initialize();
            RotateParticles();
        }

        private void LateUpdate()
        {
            if (Application.isPlaying)
                RotateParticles();
        }

    #if UNITY_EDITOR
        private void EditorUpdate()
        {
            if (Application.isPlaying || !updateInEditMode)
                return;

            RotateParticles();
            SceneView.RepaintAll();
        }
    #endif

        private void Initialize()
        {
            if (particleSystemComponent == null)
                particleSystemComponent = GetComponent<UnityEngine.ParticleSystem>();

            if (particleSystemRendererComponent == null)
                particleSystemRendererComponent = GetComponent<ParticleSystemRenderer>();

            if (particleSystemComponent == null)
                return;

            ApplyBillboardRollSettings();

            int maxParticles = particleSystemComponent.main.maxParticles;

            if (particles == null || particles.Length < maxParticles)
            {
                particles =
                    new UnityEngine.ParticleSystem.Particle[maxParticles];
            }
        }

        private void RotateParticles()
        {
            lastParticlesCount = 0;
            lastAppliedParticlesCount = 0;
            lastSetParticlesCalled = false;
            lastCameraName = string.Empty;

            if (particleSystemComponent == null)
                Initialize();

            if (particleSystemComponent == null)
                return;

            ApplyBillboardRollSettings();

            UnityEngine.ParticleSystem.MainModule main = particleSystemComponent.main;

            UnityEngine.ParticleSystem.ShapeModule shape =
                particleSystemComponent.shape;

            Camera referenceCamera = GetReferenceCamera();

            if (referenceCamera == null)
                return;

            lastCameraName = referenceCamera.name;

            Transform particleTransform = transform;

            // Центр окружности в world space: billboard roll должен считаться
            // относительно того, как круг видит камера, а не в локальной плоскости Shape.
            Vector3 shapeCenter = particleTransform.TransformPoint(shape.position);
            Transform cameraTransform = referenceCamera.transform;
            Vector3 cameraRight = cameraTransform.right;
            Vector3 cameraUp = cameraTransform.up;

            float rotationOffset =
                BillboardUpAxisToRadialOffsetDegrees + rotationOffsetDegrees;

            int particlesCount = particleSystemComponent.GetParticles(particles);
            lastParticlesCount = particlesCount;

            if (particlesCount == 0)
                return;

            for (int i = 0; i < particlesCount; i++)
            {
                float rotationDegrees;

                if (debugForceRotation)
                {
                    rotationDegrees = debugRotationDegrees;
                }
                else
                {
                    Vector3 particlePosition = GetParticleWorldPosition(
                        particles[i].position,
                        main.simulationSpace,
                        main.customSimulationSpace
                    );
                    Vector3 radial = particlePosition - shapeCenter;
                    float radialX = Vector3.Dot(radial, cameraRight);
                    float radialY = Vector3.Dot(radial, cameraUp);

                    if (invertScreenY)
                        radialY = -radialY;

                    if (radialX * radialX + radialY * radialY < MinRadialLengthSqr)
                        continue;

                    float radialAngle = Mathf.Atan2(radialY, radialX) * Mathf.Rad2Deg;

                    if (pointInside)
                        radialAngle += 180f;

                    rotationDegrees = radialAngle + rotationOffset;
                }

                particles[i].rotation = rotationDegrees;
                particles[i].rotation3D = new Vector3(0f, 0f, rotationDegrees);
                lastAppliedParticlesCount++;
            }

            particleSystemComponent.SetParticles(
                particles,
                particlesCount
            );

            lastSetParticlesCalled = true;
        }

        private void ApplyBillboardRollSettings()
        {
            if (!forceBillboardRollSettings || particleSystemComponent == null)
                return;

            UnityEngine.ParticleSystem.MainModule main = particleSystemComponent.main;
            main.startRotation3D = true;

            UnityEngine.ParticleSystem.ShapeModule shape = particleSystemComponent.shape;
            shape.alignToDirection = false;
            shape.sphericalDirectionAmount = 0f;

            if (particleSystemRendererComponent == null)
                particleSystemRendererComponent = GetComponent<ParticleSystemRenderer>();

            if (particleSystemRendererComponent == null)
                return;

            particleSystemRendererComponent.renderMode = ParticleSystemRenderMode.Billboard;
            particleSystemRendererComponent.alignment = ParticleSystemRenderSpace.View;
            particleSystemRendererComponent.allowRoll = true;
        }

        private Vector3 GetParticleWorldPosition(
            Vector3 particlePosition,
            ParticleSystemSimulationSpace simulationSpace,
            Transform customSimulationSpace
        )
        {
            return simulationSpace switch
            {
                ParticleSystemSimulationSpace.Local => transform.TransformPoint(particlePosition),
                ParticleSystemSimulationSpace.World => particlePosition,
                ParticleSystemSimulationSpace.Custom when customSimulationSpace != null =>
                    customSimulationSpace.TransformPoint(particlePosition),
                _ => transform.TransformPoint(particlePosition)
            };
        }

        private Camera GetReferenceCamera()
        {
            if (cameraOverride != null)
                return cameraOverride;

            if (Application.isPlaying)
            {
                if (cachedMainCamera == null || !cachedMainCamera.isActiveAndEnabled)
                    cachedMainCamera = Camera.main;

                return cachedMainCamera;
            }

    #if UNITY_EDITOR
            return SceneView.lastActiveSceneView != null
                ? SceneView.lastActiveSceneView.camera
                : Camera.main;
    #else
            return Camera.main;
    #endif
        }
    }
}




