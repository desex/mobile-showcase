using System.Threading;
using Cysharp.Threading.Tasks;
using LeTai.Asset.TranslucentImage;
using R3;
using Sotbify.MobileApp.Utils.Enums.Nexus;

namespace Sotbify.MobileApp.NexusWindow
{
    public interface INexus
    {
        public ReadOnlyReactiveProperty<ENexusCategory> ActiveCategory { get; }
        public ReadOnlyReactiveProperty<ENexusCategory> ActiveHelmetHighlight { get; }
        public ReadOnlyReactiveProperty<ENexusCategory> ActiveDockHighlight { get; }
        
        public void ChangeDockScreen(ENexusCategory key);
        public void ChangeHelmetScreen(ENexusCategory key);
        public void ChangeActiveCategory(ENexusCategory key, bool addToHistory = true);
        public void Undo();
        public void HighlightHeader(ENexusCategory activeSet);
        public void HighlightDock(ENexusCategory category);
    }
}