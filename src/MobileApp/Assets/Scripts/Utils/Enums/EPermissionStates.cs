namespace Sotbify.MobileApp.Utils.Enums
{
    public enum EPermissionStates
    {
        None = 0,
        DontAskAgain = 1,
        Denied = 2,
        Granted = 3,
    }

    public enum ENotificationPermissionState
    {
        NotDetermined = 0,
        Denied = 1,
        Authorized = 2,
        Provisional = 3,
        Ephemeral = 4,
        Unknown = 5,
    }
}