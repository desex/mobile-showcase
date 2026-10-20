using Sotbifay.Builder.Editor;

namespace Sotbify.MobileApp.Builder.Editor.Options
{
    public sealed class IOSOptions
    {
        [Option("iosTeamID")] public string IOSTeamID { get; set; } = string.Empty;
        [Option("iosPackageName")] public string IOSPackageName { get; set; } = string.Empty;
        [Option("partnerName")] public string PartnerName { get; set; } = string.Empty;
    }
}