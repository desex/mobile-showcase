using System;
using System.Collections.Generic;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace Sotbify.MobileApp.Localization
{
    public abstract class DictionaryVariable<TValue> : IVariableValueChanged, IVariableGroup
    {
        private readonly Dictionary<string, TValue> _values = new();

        public IReadOnlyDictionary<string, TValue> Values => _values;

        public event Action<IVariable> ValueChanged;

        public object GetSourceValue(ISelectorInfo selector) => this;

        public bool TryGetValue(string key, out IVariable value)
        {
            if (_values.TryGetValue(key, out var condition))
            {
                value = new ObjectValue { Value = condition };
                return true;
            }

            value = null;
            return false;
        }

        public void SetOrAdd(IEnumerable<KeyValuePair<string, TValue>> values)
        {
            if (values is null)
                throw new ArgumentNullException(nameof(values));
            
            foreach (var (key, value) in values) 
                _values[key] = value;
            
            ValueChanged?.Invoke(this);
        }

        public void SetOrAdd(string key, TValue value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException(nameof(key));

            _values[key] = value;
            ValueChanged?.Invoke(this);
        }

        private struct ObjectValue : IVariable
        {
            public object Value { get; set; }

            public object GetSourceValue(ISelectorInfo _) => Value;
        }
    }
}