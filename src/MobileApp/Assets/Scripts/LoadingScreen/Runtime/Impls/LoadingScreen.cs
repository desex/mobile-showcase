using R3;
using RenderHeads.Media.AVProVideo;

namespace Sotbify.MobileApp.LoadingScreen
{
    public class LoadingScreen : ILoadingScreen
    {
        private readonly ReactiveProperty<MediaPlayer> _mediaPlayer = new ();

        public ReadOnlyReactiveProperty<MediaPlayer> MediaPlayer => _mediaPlayer;
        
        public void AssignPlayer(MediaPlayer player)
        {
            _mediaPlayer.Value = player;
        }
    }
}