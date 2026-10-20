using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace Sotbify.MobileApp.Localization
{
    public static class LocalizationVariableEntryExtensions
    {
        public static T GetValueOrDefault<T>(this ILocalizationVariableEntry<Variable<T>> entry) => 
            entry.GetValueOrDefault(default);

        public static T GetValueOrDefault<T>(this ILocalizationVariableEntry<Variable<T>> entry, T defaultValue)
        {
            return entry is not null 
                ? entry.Variable.GetValueOrDefault(defaultValue) 
                : defaultValue;
        }

        public static bool TrySetValue<T>(this ILocalizationVariableEntry<Variable<T>> entry, T value)
        {
            if (entry?.Variable is null)
                return false;

            try
            {
                entry.Variable.Value = value;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}