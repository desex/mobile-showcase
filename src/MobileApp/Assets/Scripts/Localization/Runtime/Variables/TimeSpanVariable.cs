using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.Scripting;

namespace Sotbify.MobileApp.Localization
{
    [Preserve]
    [DisplayName("Time span")]
    public sealed class TimeSpanVariable : IVariableValueChanged
    {
        [SerializeField] private long _value;

        public TimeSpanVariable() : this(0)
        {
        }
        
        public TimeSpanVariable(TimeSpan value) : this(value.Ticks)
        {
        }
        
        public TimeSpanVariable(long value)
        {
            _value = value;
        }

        public TimeSpan Value
        {
            get => new(_value);
            set
            {
                if (_value == value.Ticks)
                    return;
                
                _value = value.Ticks;
                ValueChanged?.Invoke(this);
            }
        }

        public event Action<IVariable> ValueChanged;

        public object GetSourceValue(ISelectorInfo _) => new TimeSpan(_value);
    }
}