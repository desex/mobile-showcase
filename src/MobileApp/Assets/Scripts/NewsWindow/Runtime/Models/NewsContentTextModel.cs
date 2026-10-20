using System;

namespace Sotbify.MobileApp.NewsWindow.Models
{
    public sealed class NewsContentTextModel : INewsContentItemModel
    {
        public NewsContentTextModel(int position, string text)
        {
            Position = position;
            Text = text;
        }

        public int Position { get; }
        public string Text { get; }
        public Type CachedType => typeof(NewsContentTextModel);
    }
}