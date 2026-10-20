using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Sotbify.MobileApp.UI.Layout
{
    public static class LayoutElementExtensions
    {
        #region MinWidth

        public static float GetMinWidth<TState>
        (
            this RectTransform rect,
            TState state,
            [CanBeNull] Func<TState, ILayoutElement, bool> condition,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty(state, condition, static (_, element) => element.minWidth, defaultValue, out source);

        public static float GetMinWidth<TState>
        (
            this RectTransform rect,
            TState state,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty(state, null, static (_, element) => element.minWidth, defaultValue, out source);

        public static float GetMinWidth
        (
            this RectTransform rect,
            [CanBeNull] Func<ILayoutElement, bool> condition,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty
        (
            condition,
            static (condition, layoutElement) => condition.Invoke(layoutElement),
            static (_, element) => element.minWidth,
            defaultValue,
            out source
        );

        public static float GetMinWidth
        (
            this RectTransform rect,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty<object>
        (
            null,
            null,
            static (_, element) => element.minWidth,
            defaultValue,
            out source
        );

        #endregion

        #region PreferredWidth

        public static float GetPreferredWidth<TState>
        (
            this RectTransform rect,
            TState state,
            [CanBeNull] Func<TState, ILayoutElement, bool> condition,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty(state, condition, static (_, element) => element.preferredWidth, defaultValue, out source);

        public static float GetPreferredWidth<TState>
        (
            this RectTransform rect,
            TState state,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty(state, null, static (_, element) => element.preferredWidth, defaultValue, out source);

        public static float GetPreferredWidth
        (
            this RectTransform rect,
            [CanBeNull] Func<ILayoutElement, bool> condition,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty
        (
            condition,
            static (condition, layoutElement) => condition.Invoke(layoutElement),
            static (_, element) => element.preferredWidth,
            defaultValue,
            out source
        );

        public static float GetPreferredWidth
        (
            this RectTransform rect,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty<object>
        (
            null,
            null,
            static (_, element) => element.preferredWidth,
            defaultValue,
            out source
        );

        #endregion

        #region FlexibleWidth

        public static float GetFlexibleWidth<TState>
        (
            this RectTransform rect,
            TState state,
            [CanBeNull] Func<TState, ILayoutElement, bool> condition,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty(state, condition, static (_, element) => element.flexibleWidth, defaultValue, out source);

        public static float GetFlexibleWidth<TState>
        (
            this RectTransform rect,
            TState state,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty(state, null, static (_, element) => element.flexibleWidth, defaultValue, out source);

        public static float GetFlexibleWidth
        (
            this RectTransform rect,
            [CanBeNull] Func<ILayoutElement, bool> condition,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty
        (
            condition,
            static (condition, layoutElement) => condition.Invoke(layoutElement),
            static (_, element) => element.flexibleWidth,
            defaultValue,
            out source
        );

        public static float GetFlexibleWidth
        (
            this RectTransform rect,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty<object>
        (null,
            null,
            static (_, element) => element.flexibleWidth,
            defaultValue,
            out source
        );

        #endregion

        #region MinHeight

        public static float GetMinHeight<TState>
        (
            this RectTransform rect,
            TState state,
            [CanBeNull] Func<TState, ILayoutElement, bool> condition,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty(state, condition, static (_, element) => element.minHeight, defaultValue, out source);

        public static float GetMinHeight<TState>
        (
            this RectTransform rect,
            TState state,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty(state, null, static (_, element) => element.minHeight, defaultValue, out source);

        public static float GetMinHeight
        (
            this RectTransform rect,
            [CanBeNull] Func<ILayoutElement, bool> condition,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty
        (
            condition,
            static (condition, layoutElement) => condition.Invoke(layoutElement),
            static (_, element) => element.minHeight,
            defaultValue,
            out source
        );

        public static float GetMinHeight
        (
            this RectTransform rect,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty<object>
        (null,
            null,
            static (_, element) => element.minHeight,
            defaultValue,
            out source
        );

        #endregion
        
        #region PreferredHeight

        public static float GetPreferredHeight<TState>
        (
            this RectTransform rect,
            TState state,
            [CanBeNull] Func<TState, ILayoutElement, bool> condition,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty(state, condition, static (_, element) => element.preferredHeight, defaultValue, out source);

        public static float GetPreferredHeight<TState>
        (
            this RectTransform rect,
            TState state,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty(state, null, static (_, element) => element.preferredHeight, defaultValue, out source);

        public static float GetPreferredHeight
        (
            this RectTransform rect,
            [CanBeNull] Func<ILayoutElement, bool> condition,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty
        (
            condition,
            static (condition, layoutElement) => condition.Invoke(layoutElement),
            static (_, element) => element.preferredHeight,
            defaultValue,
            out source
        );

        public static float GetPreferredHeight
        (
            this RectTransform rect,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty<object>
        (null,
            null,
            static (_, element) => element.preferredHeight,
            defaultValue,
            out source
        );

        #endregion
        
        #region FlexibleHeight

        public static float GetFlexibleHeight<TState>
        (
            this RectTransform rect,
            TState state,
            [CanBeNull] Func<TState, ILayoutElement, bool> condition,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty(state, condition, static (_, element) => element.flexibleHeight, defaultValue, out source);

        public static float GetFlexibleHeight<TState>
        (
            this RectTransform rect,
            TState state,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty(state, null, static (_, element) => element.flexibleHeight, defaultValue, out source);

        public static float GetFlexibleHeight
        (
            this RectTransform rect,
            [CanBeNull] Func<ILayoutElement, bool> condition,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty
        (
            condition,
            static (condition, layoutElement) => condition.Invoke(layoutElement),
            static (_, element) => element.flexibleHeight,
            defaultValue,
            out source
        );

        public static float GetFlexibleHeight
        (
            this RectTransform rect,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty<object>
        (null,
            null,
            static (_, element) => element.flexibleHeight,
            defaultValue,
            out source
        );

        #endregion
        
        #region GetLayoutProperty

        public static float GetLayoutProperty<TState>
        (
            this RectTransform rect,
            TState state,
            [CanBeNull] Func<TState, ILayoutElement, bool> condition,
            Func<TState, ILayoutElement, float> property,
            float defaultValue,
            out ILayoutElement source
        )
        {
            source = null;
            if (rect == null)
                return 0;

            var min = defaultValue;
            var maxPriority = int.MinValue;

            using (ListPool<Component>.Get(out var components))
            {
                rect.GetComponents(typeof(ILayoutElement), components);

                var componentsCount = components.Count;
                for (var i = 0; i < componentsCount; i++)
                {
                    var layoutComp = components[i] as ILayoutElement;

                    // ReSharper disable once MergeIntoLogicalPattern
                    if (layoutComp == null || layoutComp is Behaviour { isActiveAndEnabled: false })
                        continue;

                    // ReSharper disable once RedundantBoolCompare
                    if (condition != null && condition.Invoke(state, layoutComp) == false)
                        continue;

                    // If this layout components has lower priority than a previously used, ignore it.
                    var priority = layoutComp.layoutPriority;
                    if (priority < maxPriority)
                        continue;

                    // If this layout property is set to a negative value, it means it should be ignored.
                    var prop = property.Invoke(state, layoutComp);
                    if (prop < 0)
                        continue;

                    // If this layout component has higher priority than all previous ones,
                    // overwrite with this one's value.
                    if (priority > maxPriority)
                    {
                        min = prop;
                        maxPriority = priority;
                        source = layoutComp;
                    }

                    // If the layout component has the same priority as a previously used,
                    // use the largest of the values with the same priority.
                    else if (prop > min)
                    {
                        min = prop;
                        source = layoutComp;
                    }
                }
            }

            return min;
        }

        public static float GetLayoutProperty<TState>
        (
            this RectTransform rect,
            TState state,
            Func<TState, ILayoutElement, float> property,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty(state, null, property, defaultValue, out source);

        public static float GetLayoutProperty
        (
            this RectTransform rect,
            [CanBeNull] Func<ILayoutElement, bool> condition,
            Func<ILayoutElement, float> property,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty<(Func<ILayoutElement, bool>, Func<ILayoutElement, float>)>
        (
            (condition, property),
            static (state, layoutElement) => state.Item1.Invoke(layoutElement),
            static (state, layoutElement) => state.Item2.Invoke(layoutElement),
            defaultValue,
            out source
        );

        public static float GetLayoutProperty
        (
            this RectTransform rect,
            Func<ILayoutElement, float> property,
            float defaultValue,
            out ILayoutElement source
        ) => rect.GetLayoutProperty
        (
            property,
            null,
            static (state, layoutElement) => state.Invoke(layoutElement),
            defaultValue,
            out source
        );

        #endregion
    }
}