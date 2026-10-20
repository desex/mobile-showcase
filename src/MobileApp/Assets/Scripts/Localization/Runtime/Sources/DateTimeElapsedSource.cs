using System;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Scripting;

namespace Sotbify.MobileApp.Localization
{
    [Preserve]
    [Serializable]
    public sealed class DateTimeElapsedSource : ISource
    {
        private const string Selector = "Elapsed";
        
        public bool TryEvaluateSelector(ISelectorInfo selectorInfo)
        {
            if (selectorInfo.CurrentValue is not DateTime dateTime || selectorInfo.SelectorText.Equals(Selector) == false)
                return false;

            selectorInfo.Result = DateTime.Now - dateTime;
            return true;
        }
    }
}