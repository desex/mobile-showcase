using System;

namespace Sotbify.MobileApp.NewsWindow.Models
{
    public interface INewsContentItemModel
    {
        int Position { get; }
        Type CachedType { get; }
    }
}