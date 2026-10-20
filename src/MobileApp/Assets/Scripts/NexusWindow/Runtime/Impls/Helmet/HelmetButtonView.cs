using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.NexusWindow.Helmet
{
    public class HelmetButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        
        [field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }
        public Observable<Unit> ButtonClick => _button.OnClickAsObservable();
    }
}