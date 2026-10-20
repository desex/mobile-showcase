using System;
using Sotbify.MobileApp.Localization;
using UnityEngine;

namespace Sotbify.MobileApp.ChatMessages.Views
{
    public class DateView : MonoBehaviour
    {
        [SerializeField] private LocalizationVariableEntry<DateTimeVariable> _dateVariableEntry;

        public DateTime Date
        {
            get => _dateVariableEntry?.Variable?.Value ?? DateTime.MinValue;
            set
            {
                if(_dateVariableEntry?.Variable is null)
                    return;
                
                _dateVariableEntry.Variable.Value = value;
            }
        } 
    }
}