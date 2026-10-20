using System;
using UnityEngine;

namespace Sotbify.MobileApp.FAQWindow
{
    [Serializable]
    public struct FAQInfo
    {
        public string Header;
        
        [TextArea]
        public string Content;
    }
}