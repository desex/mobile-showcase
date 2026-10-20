using System;
using Sotbify.MobileApp.Common.SerializableDictionary;
using UnityEngine.UI;

namespace Stab.StabHomeWindow
{
    [Serializable]
    public sealed class ItemToButton : SerializableDictionary<EHomeItem, Button>
    {
    }
}