using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.Scripting;

namespace Sotbify.MobileApp.Localization
{
    [Preserve]
    [DisplayName("Date time offset")]
    public sealed class DateTimeOffsetVariable : IVariableValueChanged
    {
        [SerializeField] private long _value;

        public DateTimeOffsetVariable() : this(0)
        {
        }
        
        public DateTimeOffsetVariable(DateTimeOffset value) : this(value.ToUnixTimeMilliseconds())
        {
            }
        
        public DateTimeOffsetVariable(long value)
        {
            _value = value;
        }

        public DateTimeOffset Value
        {
            get => DateTimeOffset.FromUnixTimeMilliseconds(_value);
            set
            {
                var valueTimeMilliseconds = value.ToUnixTimeMilliseconds();
                if (_value == valueTimeMilliseconds)
                    return;
                
                _value = valueTimeMilliseconds;
                ValueChanged?.Invoke(this);
            }
        }

        public event Action<IVariable> ValueChanged;

        public object GetSourceValue(ISelectorInfo _) => DateTimeOffset.FromUnixTimeMilliseconds(_value);
    }
}