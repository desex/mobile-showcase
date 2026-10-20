using System;

namespace Sotbify.MobileApp.PlayWindow.Adapter
{
    public class PartnerModel : IPartnerModel
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public Uri ImageUri { get; set; }
    }
}