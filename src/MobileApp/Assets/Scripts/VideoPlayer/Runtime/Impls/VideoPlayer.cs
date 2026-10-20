using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using RenderHeads.Media.AVProVideo;
using UnityEngine;

namespace Sotbify.MobileApp.VideoPlayer
{
    public class VideoPlayer : IVideoPlayer, IDisposable
    {
        private const float PreparationTimeoutSeconds = 15f;
        private readonly MediaPlayer _videoPlayer;
        private readonly ReactiveProperty<bool> _isPrepared = new(false);

        public ReadOnlyReactiveProperty<bool> IsPrepared => _isPrepared;
        public MediaPlayer MediaPlayer { get; private set; }

        public VideoPlayer(MediaPlayer videoPlayer)
        {
            _videoPlayer = videoPlayer;
            MediaPlayer = videoPlayer;
        }

        public async UniTask PrepareVideo(MediaPath videoPath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_videoPlayer.MediaOpened)
                _videoPlayer.CloseMedia();
            
            _isPrepared.Value = false;
            try
            {
                if (!_videoPlayer.OpenMedia(videoPath.PathType, videoPath.Path, false))
                    throw new InvalidOperationException($"Failed to open video: {videoPath.Path}");

                await WaitForVideo(() => _videoPlayer.Control != null && _videoPlayer.Control.CanPlay(), cancellationToken);
                _isPrepared.Value = true;
            }
            catch
            {
                _videoPlayer.CloseMedia();
                throw;
            }
        }

        public async UniTask WaitForFirstFrame(CancellationToken cancellationToken = default)
        {
            try
            {
                await WaitForVideo(() => _videoPlayer.Control != null && _videoPlayer.Control.CanPlay()
                    && _videoPlayer.TextureProducer != null
                    && _videoPlayer.TextureProducer.GetTextureFrameCount() > 0
                    && _videoPlayer.TextureProducer.GetTexture() != null, cancellationToken);
            }
            catch
            {
                _isPrepared.Value = false;
                _videoPlayer.CloseMedia();
                throw;
            }
        }

        private async UniTask WaitForVideo(Func<bool> isReady, CancellationToken cancellationToken)
        {
            var error = ErrorCode.None;
            var startTime = Time.realtimeSinceStartup;

            void OnMediaPlayerEvent(MediaPlayer player, MediaPlayerEvent.EventType eventType, ErrorCode errorCode)
            {
                if (eventType == MediaPlayerEvent.EventType.Error)
                    error = errorCode;
            }

            _videoPlayer.Events.AddListener(OnMediaPlayerEvent);
            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var currentError = _videoPlayer.Control?.GetLastError() ?? ErrorCode.None;
                    if (error != ErrorCode.None || currentError != ErrorCode.None)
                        throw new InvalidOperationException($"Video preparation failed: {(error != ErrorCode.None ? error : currentError)}");

                    if (!_videoPlayer.MediaOpened)
                        throw new InvalidOperationException("Video was closed during preparation.");

                    if (isReady())
                        return;

                    if (Time.realtimeSinceStartup - startTime >= PreparationTimeoutSeconds)
                        throw new TimeoutException("Timed out waiting for video preparation.");

                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }
            }
            finally
            {
                _videoPlayer.Events.RemoveListener(OnMediaPlayerEvent);
            }
        }

        public void PlayCurrentVideo()
        {
            _videoPlayer.Play();
        }

        public void Stop()
        {
            _videoPlayer.Stop();
        }

        public void Dispose()
        {
            _isPrepared.Dispose();
        }
    }
}