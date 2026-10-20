using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.FAQWindow
{
    public class FAQItem : MonoBehaviour
    {
        [SerializeField] private Button _readButton;
        
        [SerializeField] 
        private FAQInfo _faqContent;

        public Observable<FAQInfo> ReadButtonPressed => _readButton.OnClickAsObservable()
            .Select(_faqContent, static(_, faqContent) => faqContent);
    }
}