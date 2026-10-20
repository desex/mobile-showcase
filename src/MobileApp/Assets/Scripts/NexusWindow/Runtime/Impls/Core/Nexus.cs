using System;
using System.Collections.Generic;
using System.Threading;
using LeTai.Asset.TranslucentImage;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.DockWindowSwitcher;
using Sotbify.MobileApp.Utils.Enums.Nexus;

namespace Sotbify.MobileApp.NexusWindow.Core
{
    public class Nexus : INexus, IDisposable
    {
        private readonly IWindowSwitcher _windowSwitcher;
        private readonly TranslucentImageSource _translucentImageSource;
        private readonly ILogger _logger;

        private readonly ReactiveProperty<ENexusCategory> _activeCategory = new();
        private readonly ReactiveProperty<ENexusCategory> _activeHelmetHighlight = new();
        private readonly ReactiveProperty<ENexusCategory> _activeDockHighlight = new();
        private readonly Stack<ENexusCategory> _categoriesHistory = new();

        private IDisposable _disposable;

        public Nexus
        (
            IWindowSwitcher windowSwitcher,
            TranslucentImageSource translucentImageSource,
            ILogger logger
        )
        {
            _windowSwitcher = windowSwitcher ?? throw new ArgumentNullException(nameof(windowSwitcher));
            _translucentImageSource = translucentImageSource ?? throw new ArgumentNullException(nameof(translucentImageSource));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ReadOnlyReactiveProperty<ENexusCategory> ActiveCategory => _activeCategory;
        public ReadOnlyReactiveProperty<ENexusCategory> ActiveHelmetHighlight => _activeHelmetHighlight;
        public ReadOnlyReactiveProperty<ENexusCategory> ActiveDockHighlight => _activeDockHighlight;

        public void ChangeDockScreen(ENexusCategory key)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ChangeHelmetScreen(ENexusCategory key)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ChangeActiveCategory(ENexusCategory key, bool addToHistory = true)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Undo()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void HighlightDock(ENexusCategory category) 
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void HighlightHeader(ENexusCategory category) 
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Dispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}