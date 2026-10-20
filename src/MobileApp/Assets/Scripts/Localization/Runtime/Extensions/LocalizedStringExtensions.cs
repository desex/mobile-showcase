using System;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace Sotbify.MobileApp.Localization
{
    public static class LocalizedStringExtensions
    {
        public static TVariable AddVariable<TVariable>(this LocalizedString localizedString, string name, TVariable variable)
            where TVariable : IVariable
        {
            if (localizedString is null)
                throw new ArgumentNullException(nameof(localizedString));

            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (variable is null)
                throw new ArgumentNullException(nameof(variable));

            localizedString.Add(name, variable);
            return variable;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static bool TryGetVariable<TVariable>(this LocalizedString localizedString, string name, out TVariable result)
            where TVariable : IVariable
        {
            if (localizedString is null)
                throw new ArgumentNullException(nameof(localizedString));
            
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (localizedString.TryGetValue(name, out var value) && value is TVariable variable)
            {
                result = variable;
                return true;
            }

            result = default;
            return false;
        }

        public static TVariable GetOrAddVariable<TVariable>(this LocalizedString localizedString, string name) where TVariable : IVariable, new() => 
            localizedString.GetOrAddVariable(name, () => new TVariable());

        // ReSharper disable once MemberCanBePrivate.Global
        public static TVariable GetOrAddVariable<TVariable>(this LocalizedString localizedString, string name, Func<TVariable> variableFactory)
            where TVariable : IVariable
        {
            if (localizedString is null)
                throw new ArgumentNullException(nameof(localizedString));

            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (variableFactory is null)
                throw new ArgumentNullException(nameof(variableFactory));
            
            // ReSharper disable once InvertIf
            if (localizedString.TryGetVariable<TVariable>(name, out var variable) == false)
            {
                variable = variableFactory.Invoke();
                localizedString.Add(name, variable);
            }

            return variable;
        }
    }
}