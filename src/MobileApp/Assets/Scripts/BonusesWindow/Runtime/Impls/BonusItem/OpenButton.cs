using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.BonusesWindow.BonusItem
{
    public class OpenButton : MonoBehaviour
    {
        [SerializeField] private Image _arrowIcon;
        [SerializeField] private Image _closeIcon;
        [SerializeField] private Button _button;
        
        public Observable<Unit> OnButtonClick => _button.OnClickAsObservable();

        public void ChangeState(bool isOpened)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}