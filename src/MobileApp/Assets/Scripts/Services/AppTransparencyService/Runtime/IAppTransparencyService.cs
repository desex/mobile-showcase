using AppTrackingTransparency.Common;

namespace Services.AppTransparencyService
{
    public interface IAppTransparencyService
    {
        public AppTrackingTransparencyAuthorizationStatus Status { get; }
        public void RequestPermission();
        public void Track();
    }
}