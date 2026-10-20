using UnityEngine;

namespace Sotbify.MobileApp.Db.ChatParameters.Impl
{
    [CreateAssetMenu(menuName = "Settings/" + nameof(ChatParameters), fileName = nameof(ChatParameters))]
    public class ChatParameters : ScriptableParameters, IChatParameters
    {
        [field: SerializeField] public int PreLoadDifference { get; private set; }
        [field: SerializeField] public int DateShowTimeSpan { get; private set; } = 15;
        [field: SerializeField] public float KeyboardTopPadding { get; private set; } = 12.5f;
        [field: SerializeField] public float ManagerStatusRequestDelayMinutes { get; private set; } = 5f;
    }
}