using System;
using UnityEngine;

namespace Sotbify.MobileApp.HomeWindow
{
    public readonly struct Item : IDisposable
    {
        private readonly IDisposable _disposable;

        public Item(Texture texture, Color? backgroundColor = null, IDisposable disposable = null)
        {
            _disposable = disposable;
            Texture = texture != null ? texture : Texture2D.blackTexture;
            BackgroundColor = backgroundColor ?? Color.clear;
        }

        public Texture Texture { get; }
        public Color BackgroundColor { get; }

        public void Dispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}