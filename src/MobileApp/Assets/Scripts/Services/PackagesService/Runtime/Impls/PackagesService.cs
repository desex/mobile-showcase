namespace Services.PackagesService
{
    public class PackagesService : IPackagesService
    {
        public long GetPackageVersionCode(string packageName)
        {
            using var unityPlayer = new UnityEngine.AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var currentActivity = unityPlayer.GetStatic<UnityEngine.AndroidJavaObject>("currentActivity");
            using var packageManager = currentActivity.Call<UnityEngine.AndroidJavaObject>("getPackageManager");
            // var packageName = currentActivity.Call<string>("getPackageName");
            using var packageInfo = packageManager.Call<UnityEngine.AndroidJavaObject>("getPackageInfo", packageName, 0);

            return packageInfo.Call<long>("getLongVersionCode");
        }
    }
}