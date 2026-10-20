using System;
using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;
using Sotbify.MobileApp.Common.SerializableDictionary;
using UnityEngine;

namespace Sotbify.MobileApp.Db.MediaPathConfig.Impl
{
    [CreateAssetMenu(menuName = "Settings/MediaPath_Config", fileName = "MediaPath_Config")]
    public class MediaPathConfig : ScriptableObject, IMediaPathConfig
    {
        [SerializeField] private WindowToPath _mediaPaths;

        public IReadOnlyDictionary<string, MediaPath> MediaPaths => _mediaPaths;
    }

    [Serializable]
    public sealed class WindowToPath : SerializableDictionary<string, MediaPath>
    {
    }
}