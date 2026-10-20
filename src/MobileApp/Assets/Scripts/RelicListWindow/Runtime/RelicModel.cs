using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Sotbify.MobileApp.RelicListWindow
{
    public class RelicModel
    {
        public int Id { get; set; }
        public int Rarity { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public Color GradientFrom { get; set; } = Color.clear;
        public Color MainColor { get; set; } = Color.clear;
        public Color GradientTo { get; set; } = Color.clear;
        [CanBeNull] public Uri ImageUrl { get; set; } = null;
    }
}