using System.Linq;
using UnityEditor;

namespace Sotbifay.Builder.Editor
{
    public static class SplashScreenUtils
    {
        public static void RemoveUnityLogoFromSplashScreen()
        {
            PlayerSettings.SplashScreen.showUnityLogo = false;
        
            // showUnityLogo = false doesn't remove Unity logo from sequence list, so we do it manually here.
            var unityLogo = PlayerSettings.SplashScreenLogo.CreateWithUnityLogo();

            PlayerSettings.SplashScreen.logos = PlayerSettings.SplashScreen.logos
                .Where(splashScreenLogo => splashScreenLogo.logo.name != unityLogo.logo.name)
                .ToArray();
        }
    }
}