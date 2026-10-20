using System;

namespace Sotbify.MobileApp.PlayWindow
{
    public interface IPartnerModel
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public Uri ImageUri { get; set; }
    }
}