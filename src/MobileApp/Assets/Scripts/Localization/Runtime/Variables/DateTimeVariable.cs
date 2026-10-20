using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.Scripting;

namespace Sotbify.MobileApp.Localization
{
    [Preserve]
    [DisplayName("Date time")]
    public sealed class DateTimeVariable : IVariableValueChanged
    {
        [SerializeField] private long _value;

        public DateTimeVariable() : this(0)
        {
        }
        
        public DateTimeVariable(DateTime value) : this(value.Ticks)
        {
        }
        
        public DateTimeVariable(long value)
        {
            _value = value;
        }

        public DateTime Value
        {
            get => new(_value);
            set
            {
                var valueTicks = value.Ticks;
                if (_value == valueTicks)
                    return;
                
                _value = valueTicks;
                ValueChanged?.Invoke(this);
            }
        }

        public event Action<IVariable> ValueChanged;

        public object GetSourceValue(ISelectorInfo _) => new DateTime(_value);
    }
}