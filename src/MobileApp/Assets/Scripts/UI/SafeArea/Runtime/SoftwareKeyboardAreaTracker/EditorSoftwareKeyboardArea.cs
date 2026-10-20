using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Sotbify.MobileApp.UI.SafeArea
{
    public class EditorSoftwareKeyboardArea : ISoftwareKeyboardArea
    {
        private const float HeightRatio = 0.35f;
        private const int StubSortingOrder = short.MaxValue;
        
        private static InputAction _f2Action;
        private static GameObject _keyboardStub;

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
        private static void Reset()
        {
            HideStubKeyboard();
            
            _f2Action = new InputAction("F2", InputActionType.Button, "<Keyboard>/f2");
            _f2Action.performed += OnActionPerformed;
            _f2Action.Enable();
            
            Application.quitting -= OnQuitting;
            Application.quitting += OnQuitting;
        }

        private static void OnQuitting()
        {
            Application.quitting -= OnQuitting;
            
            _f2Action.performed -= OnActionPerformed;
            _f2Action.Disable();

            HideStubKeyboard();
        }

        public int GetHeight(bool includeInput = false) =>
            _keyboardStub != null ? Mathf.RoundToInt(Screen.height * HeightRatio) : 0;

        public void Dispose() { }

        private static void OnActionPerformed(InputAction.CallbackContext context)
        {
            if (_keyboardStub != null)
            {
                HideStubKeyboard();
            }
            else
            {
                ShowStubKeyboard();
            }
        }

        private static void ShowStubKeyboard()
        {
            if (_keyboardStub != null)
                return;

            _keyboardStub = CreateStubKeyboardGameObject();
            Object.DontDestroyOnLoad(_keyboardStub);
        }

        private static void HideStubKeyboard()
        {
            if (_keyboardStub != null)
                Object.Destroy(_keyboardStub);

            _keyboardStub = null;
        }

        private static GameObject CreateStubKeyboardGameObject()
        {
            var canvasGameObject = new GameObject("StubSoftwareKeyboard_Canvas");

            var canvas = canvasGameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = StubSortingOrder;

            var imageGameObject = new GameObject("StubSoftwareKeyboard_Image");
            var imageTransform = imageGameObject.AddComponent<RectTransform>();
            imageTransform.SetParent(canvasGameObject.transform, worldPositionStays: false);
            imageTransform.anchorMin = Vector2.zero;
            imageTransform.anchorMax = new Vector2(1f, HeightRatio);
            imageTransform.offsetMin = Vector2.zero;
            imageTransform.offsetMax = Vector2.zero;

            var image = imageGameObject.AddComponent<Image>();
            image.color = new Color(0f, 0f, 1f, 0.5f);
            image.raycastTarget = false;

            return canvasGameObject;
        }
    }
}