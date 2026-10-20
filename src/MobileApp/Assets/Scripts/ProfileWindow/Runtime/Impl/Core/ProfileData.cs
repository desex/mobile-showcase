using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Sotbify.MobileApp.ProfileWindow.Core
{
    public class ProfileData
    {
        public string PlayerName;
        public string CharacterName;
        public Color CharacterColor;
        [CanBeNull] public Uri ImageUri;
    }
}