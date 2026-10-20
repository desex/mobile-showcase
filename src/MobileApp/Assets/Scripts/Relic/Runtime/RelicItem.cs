using System;
using System.Text;
using UnityEngine;

namespace Sotbify.MobileApp.Relic
{
    public sealed class RelicItem : IRelicItem
    {
        public string Key { get; set; }
        public Uri ImageUri { get; set; }
        public Color BackgroundColor { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append('(');
            
            if (string.IsNullOrEmpty(Key) == false) 
                stringBuilder.Append(nameof(Key)).Append(": ").Append(Key).Append(", ");
            
            if (ImageUri is not null) 
                stringBuilder.Append(nameof(ImageUri)).Append(": ").Append(ImageUri).Append(", ");
            
            stringBuilder.Append(nameof(BackgroundColor)).Append(": ").Append(BackgroundColor);
            
            stringBuilder.Append(')');
            
            return stringBuilder.ToString();
        }
    }
}