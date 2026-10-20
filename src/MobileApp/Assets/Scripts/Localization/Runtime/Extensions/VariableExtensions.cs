using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace Sotbify.MobileApp.Localization
{
    public static class VariableExtensions
    {
        public static T GetValueOrDefault<T>(this Variable<T> variable) => 
            variable.GetValueOrDefault(default);

        public static T GetValueOrDefault<T>(this Variable<T> variable, T defaultValue)
        {
            return variable is not null 
                ? variable.Value
                : defaultValue;
        }

        public static bool TrySetValue<T>(this Variable<T> variable, T value)
        {
            if (variable is null)
                return false;

            try
            {
                variable.Value = value;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}