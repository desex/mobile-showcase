using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using RenderHeads.Media.AVProVideo;

namespace Sotbify.MobileApp.VideoPlayer
{
    public interface IVideoPlayer
    {
        public ReadOnlyReactiveProperty<bool> IsPrepared { get; }
        public MediaPlayer MediaPlayer { get; }
        public UniTask PrepareVideo(MediaPath videoPath, CancellationToken cancellationToken = default);
        public UniTask WaitForFirstFrame(CancellationToken cancellationToken = default);
        public void PlayCurrentVideo();
        public void Stop();
    }
}