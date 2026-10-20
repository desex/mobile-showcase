using System;
using R3;

namespace Sotbify.MobileApp.UI.Common.Components.StateTracker
{
    public static class StateTrackerExtensions
    {
        public static Observable<StateTracker.State> AsObservable(this StateTracker tracker)
        {
            if (tracker == null)
                throw new ArgumentNullException(nameof(tracker));
            
            return Observable.FromEvent<StateTracker.State>
            (
                h => tracker.StateChanged += h,
                h => tracker.StateChanged -= h
            );
        }
        
        public static ReadOnlyReactiveProperty<StateTracker.State> AsReactiveProperty(this StateTracker tracker)
        {
            if (tracker == null)
                throw new ArgumentNullException(nameof(tracker));

            return tracker.AsObservable()
                .ToReadOnlyReactiveProperty(tracker.CurrentState);
        }
    }
}