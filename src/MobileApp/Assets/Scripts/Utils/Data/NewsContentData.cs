using System;
using Sotbify.MobileApp.Utils.Enums;

namespace Sotbify.MobileApp.Utils.Data
{
    public struct NewsContentData
    {
        public int Id;
        public string Title;
        public string ShortDescription;
        public string SpriteUrl;
        public long CreatedAt;
        public string Text;
        public EHomeItemType Type;

        public DateTime CreatedAtDate => new(CreatedAt);
    }
}