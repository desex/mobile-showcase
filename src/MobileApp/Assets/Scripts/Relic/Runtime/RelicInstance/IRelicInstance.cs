using System;
using UnityEngine;

namespace Sotbify.MobileApp.Relic
{
    public interface IRelicInstance : IDisposable
    {
        Texture Texture { get; }
        Color BackgroundColor { get; }

        event Action Loaded;
    }
}