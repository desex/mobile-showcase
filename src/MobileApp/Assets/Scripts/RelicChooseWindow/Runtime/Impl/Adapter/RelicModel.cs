using System;
using JetBrains.Annotations;

namespace Sotbify.MobileApp.RelicChooseWindow.Adapter
{
    public class RelicModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int Rarity { get; set; }
        [CanBeNull] public Uri ImageUrl { get; set; } = null;
    }
}