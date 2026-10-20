using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace Sotbify.MobileApp.Localization
{
    public interface ILocalizationVariableEntry<out TVariable> where TVariable : IVariable
    {
        TVariable Variable { get; }
    }
}