using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace Sotbify.MobileApp.Localization
{
    [Serializable]
    public sealed class LocalizationVariableEntry : ILocalizationVariableEntry<IVariable>
    {
        [Required] 
        [SerializeField] private LocalizeStringEvent _localizeStringEvent;
        [ValueDropdown(nameof(GetVariables))] 
        [SerializeField] private string _variable;

        public IVariable Variable
        {
            get
            {
                if (_localizeStringEvent == null || _localizeStringEvent.StringReference is null)
                    return null;
                
                var stringReference = _localizeStringEvent.StringReference;
                return stringReference.TryGetValue(_variable, out var value) ? value : null;
            }
        }
        
        private IEnumerable<string> GetVariables()
        {
#if UNITY_EDITOR
            if (_localizeStringEvent == null || _localizeStringEvent.StringReference is null)
                return Enumerable.Empty<string>();

            var stringReference = _localizeStringEvent.StringReference;
            return stringReference.Keys;
#endif
            return Enumerable.Empty<string>();
        }
    }

    [Serializable]
    public sealed class LocalizationVariableEntry<TVariable> : ILocalizationVariableEntry<TVariable>
        where TVariable : IVariable
    {
        [Required] 
        [SerializeField] private LocalizeStringEvent _localizeStringEvent;
        [ValueDropdown(nameof(GetVariables))] 
        [SerializeField] private string _variable;
        
        public TVariable Variable
        {
            get
            {
                if (_localizeStringEvent == null || _localizeStringEvent.StringReference is null)
                    return default;
                
                var stringReference = _localizeStringEvent.StringReference;
                return stringReference.TryGetVariable<TVariable>(_variable, out var value) ? value : default;
            }
        }
        
        public static implicit operator TVariable(LocalizationVariableEntry<TVariable> entry) => entry.Variable;

        private IEnumerable<string> GetVariables()
        {
#if UNITY_EDITOR
            if (_localizeStringEvent == null || _localizeStringEvent.StringReference is null)
                return Enumerable.Empty<string>();

            var stringReference = _localizeStringEvent.StringReference;
            return stringReference.Keys
                .Where(key => stringReference.TryGetValue(key, out var variable) && variable is TVariable);
#endif
            return Enumerable.Empty<string>();
        }
    }
}