using System;
using UnityEngine;

namespace Sotbifay.Spotlight
{
    public interface ISpotlightCutout
    {
        event Action Changed;
        
        Rect GetWorldSpaceRect();
    }
}