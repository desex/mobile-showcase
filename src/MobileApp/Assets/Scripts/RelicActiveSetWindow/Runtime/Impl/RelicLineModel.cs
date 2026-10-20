using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Sotbify.MobileApp.RelicActiveSetWindow
{
    public class RelicLineModel
    {
        public int Id;
        public string Name;
        public string Key;
        public int Rarity;
        public int Position;
        public Color RelicColor;
        [CanBeNull] public Uri ImageUrl;
    }
}