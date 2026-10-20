using RenderHeads.Media.AVProVideo;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;

namespace Sotbify.MobileApp.LoadingScreen
{
    public class LoadingScreenView : FadeWindowView
    {
        [SerializeField] private DisplayUGUI _mediaPlayer;
        
        public MediaPlayer MediaPlayer { set => _mediaPlayer.Player = value; }
    }
}