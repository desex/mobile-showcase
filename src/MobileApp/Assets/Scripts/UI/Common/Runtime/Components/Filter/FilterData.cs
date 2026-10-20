using System;

namespace Sotbify.MobileApp.UI.Common.Components.Filter
{
    public class FilterData<T>
        where T : Enum
    {
        public T Value;
        public int Count;
    }
}