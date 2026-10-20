using R3;
using RenderHeads.Media.AVProVideo;

namespace Sotbify.MobileApp.LoadingScreen
{
    public interface ILoadingScreen
    {
        public ReadOnlyReactiveProperty<MediaPlayer> MediaPlayer { get; }
        
        public void AssignPlayer(MediaPlayer player);
    }
}