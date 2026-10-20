using System;

namespace Sotbify.MobileApp.UI.Common.Components.StateTracker
{
    public static class StatePropertyBlockExtensions
    {
        public static TProperty GetValueByState<TProperty>(this StatePropertyBlock<TProperty> propertyBlock, StateTracker.State state)
        {
            if (propertyBlock == null)
                throw new ArgumentNullException(nameof(propertyBlock));

            return state switch
            {
                StateTracker.State.Normal => propertyBlock.Normal,
                StateTracker.State.Hover => propertyBlock.UseHover ? propertyBlock.Hover : propertyBlock.GetValueByState(StateTracker.State.Normal),
                StateTracker.State.Pressed => propertyBlock.UsePressed ? propertyBlock.Pressed : propertyBlock.GetValueByState(StateTracker.State.Hover),
                StateTracker.State.Disabled => propertyBlock.Disabled,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
        }
    }
}