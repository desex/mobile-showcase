using System;
using Com.ForbiddenByte.OSA.Core;
using Sotbify.MobileApp.Localization;
using Sotbify.MobileApp.UI.Common.Components.Filter;

namespace Sotbify.MobileApp.HomeWindow.Filter
{
    public class FilterViewsHolder<T> : BaseItemViewsHolder
        where T : Enum
    {
        private readonly IStringLocalizer _stringLocalizer;
        
        public FilterItem<T> View { get; private set; }

        public FilterViewsHolder(IStringLocalizer stringLocalizer)
        {
            _stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
        }

        public override void CollectViews()
        {
            View = root.GetComponent<FilterItem<T>>();
        }

        public void Bind(FilterData<T> filterData)
        {
            var type = typeof(T);
            var value = filterData.Value;
            
            var typeName = type.Name;
            var valueName = Enum.GetName(type, value);

            var localizedValue = _stringLocalizer.GetLocalizedString(typeName, valueName);

            View.Name = string.IsNullOrEmpty(localizedValue) == false ? localizedValue : valueName;
            View.Count = filterData.Count;
            View.Value = value;
        }
    }
}