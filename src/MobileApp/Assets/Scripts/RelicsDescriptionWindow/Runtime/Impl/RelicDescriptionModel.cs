using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Sotbify.MobileApp.RelicsDescriptionWindow
{
    public class RelicDescriptionModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int Rarity { get; set; } = 0;
        public Color MainColor { get; set; } = Color.white;
        public Color ColorFrom { get; set; } = Color.white;
        public Color ColorTo { get; set; } = Color.white;
        [CanBeNull] public Uri ImageUrl { get; set; } = null;
    }
}