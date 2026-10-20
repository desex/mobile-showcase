using System;

namespace Sotbify.MobileApp.NewsWindow.Models
{
    public sealed class NewsContentImageModel : INewsContentItemModel
    {
        public NewsContentImageModel(int position, Uri imageUri)
        {
            Position = position;
            ImageUri = imageUri;
        }

        public int Position { get; }
        public Uri ImageUri { get; }
        public Type CachedType => typeof(NewsContentImageModel);
    }
}