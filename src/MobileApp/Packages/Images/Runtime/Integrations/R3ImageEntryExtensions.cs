#if R3_SUPPORT

using System;
using R3;
using UnityEngine;

namespace Sotbifay.Images
{
    public static class R3ImageEntryExtensions
    {
        public static Observable<Unit> LoadedObservable(this IImageEntry entry)
        {
            return entry is not null
                ? Observable.FromEvent(h => entry.Loaded += h, h => entry.Loaded -= h) 
                : throw new ArgumentNullException(nameof(entry));
        }
        
        public static ReadOnlyReactiveProperty<bool> IsLoadedReactiveProperty(this IImageEntry entry)
        {
            if(entry is null)
                throw new ArgumentNullException(nameof(entry));

            return entry.LoadedObservable()
                .Select(entry, static (_, entry) => entry.IsLoaded)
                .ToReadOnlyReactiveProperty(entry.IsLoaded);
        }

        public static Observable<Texture2D> WhenLoadedObservable(this IImageEntry entry)
        {
            if(entry is null)
                throw new ArgumentNullException(nameof(entry));

            return entry.LoadedObservable()
                .Prepend(Unit.Default)
                .Select(entry, static (_, entry) => entry.IsLoaded)
                .Where(isLoaded => isLoaded)
                .Select(entry, static (_, entry) => entry.Texture);
        }
    }
}

#endif
