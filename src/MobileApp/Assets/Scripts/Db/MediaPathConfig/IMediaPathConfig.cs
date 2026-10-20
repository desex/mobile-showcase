using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;

namespace Sotbify.MobileApp.Db.MediaPathConfig
{
    public interface IMediaPathConfig
    {
        public IReadOnlyDictionary<string,MediaPath> MediaPaths { get; }
    }
}