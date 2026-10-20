using System;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Common.Texture
{
    public static class ConvertTextureExtensions
    {
        public static Texture2D CreateTextureFromBytes(this byte[] bytes, bool markNonReadable = true) => 
            CreateTextureFromBytes(bytes.AsSpan(), markNonReadable);
        
        public static Texture2D CreateTextureFromBytes(this ReadOnlyMemory<byte> bytes, bool markNonReadable = true) => 
            CreateTextureFromBytes(bytes.Span, markNonReadable);

        public static Texture2D CreateTextureFromBytes(this ReadOnlySpan<byte> bytes, bool markNonReadable = true)
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.LoadImage(bytes, markNonReadable);
            return texture;
        }
    }
}