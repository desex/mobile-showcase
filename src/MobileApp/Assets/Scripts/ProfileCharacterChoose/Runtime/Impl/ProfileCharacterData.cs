using System;
using UnityEngine;

namespace Sotbify.MobileApp.ProfileCharacterChoose
{
    public class ProfileCharacterData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Uri ImageUri { get; set; }
        public Color Color { get; set; }
        public bool IsActive { get; set; }
    }
}