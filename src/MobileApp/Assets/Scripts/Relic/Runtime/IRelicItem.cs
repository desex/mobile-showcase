using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Sotbify.MobileApp.Relic
{
    public interface IRelicItem
    {
        [CanBeNull] string Key { get; }
        [CanBeNull] Uri ImageUri { get; }
        Color BackgroundColor { get; }
    }
}