using UnityEngine;

namespace Sotbify.MobileApp.Db.TutorialConfig
{
    [CreateAssetMenu(menuName = "Settings/Tutorial_Config", fileName = "Tutorial_Config")]
    public class TutorialConfig : ScriptableObject, ITutorialConfig
    {
        [field: SerializeField] 
        public float RelicsSecondsDelay { get; private set; } = 0.3f;
    }
}