using System;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace Sotbify.MobileApp.Localization
{
    public static class LocalizeStringEventExtensions
    {
        public static TVariable AddVariable<TVariable>(this LocalizeStringEvent localizeStringEvent, string name, TVariable variable)
            where TVariable : IVariable
        {
            if (localizeStringEvent == null)
                throw new ArgumentNullException(nameof(localizeStringEvent));

            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (variable is null)
                throw new ArgumentNullException(nameof(variable));

            var localizedString = localizeStringEvent.StringReference;
            return localizedString is not null 
                ? localizedString.AddVariable(name, variable) 
                : throw new InvalidOperationException("Localized string is null");
        }
        
        // ReSharper disable once MemberCanBePrivate.Global
        public static bool TryGetVariable<TVariable>(this LocalizeStringEvent localizeStringEvent, string name, out TVariable result)
            where TVariable : IVariable
        {
            if (localizeStringEvent is null)
                throw new ArgumentNullException(nameof(localizeStringEvent));
            
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var localizedString = localizeStringEvent.StringReference;
            return localizedString is not null 
                ? localizedString.TryGetVariable(name, out result) 
                : throw new InvalidOperationException("Localized string is null");
        }
        
        public static TVariable GetOrAddVariable<TVariable>(this LocalizeStringEvent localizeStringEvent, string name) where TVariable : IVariable, new() => 
            localizeStringEvent.GetOrAddVariable(name, () => new TVariable());

        // ReSharper disable once MemberCanBePrivate.Global
        public static TVariable GetOrAddVariable<TVariable>(this LocalizeStringEvent localizeStringEvent, string name, Func<TVariable> variableFactory)
            where TVariable : IVariable
        {
            if (localizeStringEvent is null)
                throw new ArgumentNullException(nameof(localizeStringEvent));

            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (variableFactory is null)
                throw new ArgumentNullException(nameof(variableFactory));
            
            var localizedString = localizeStringEvent.StringReference;
            return localizedString is not null 
                ? localizedString.GetOrAddVariable(name, variableFactory) 
                : throw new InvalidOperationException("Localized string is null");
        }
    }
}