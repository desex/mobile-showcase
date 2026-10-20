using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using Sotbify.MobileApp.Common.SerializableDictionary;
using Sotbify.MobileApp.Services.CanvasLayoutAwaiter;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Enums.Nexus;
using Sotbify.MobileApp.Utils.Extensions;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using VContainer;

namespace Sotbify.MobileApp.NexusWindow.Dock
{
    public class DockView : MonoBehaviour
    {
        [SerializeField] private DockItems _dockItems;
        [SerializeField] private HighlighterView _highlighter;
        [SerializeField] private DockCategoryToTutorialHighlighter _dockCategoryToTutorialHighlighter;
        [SerializeField] private RectTransform _content;

        private ICanvasLayoutAwaiterService _canvasLayoutAwaiterService;

        public IReadOnlyDictionary<ENexusCategory, DockItem> DockItems => _dockItems;

        [Inject]
        public void Construct(ICanvasLayoutAwaiterService canvasLayoutAwaiterService)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ForceRebuild()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask ActivateItemAsync(ENexusCategory nexusCategory, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public async UniTask DeactivateItemAsync(ENexusCategory nexusCategory, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ChangeHighlight(ENexusCategory category)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }

    [Serializable]
    public sealed class DockItems : SerializableDictionary<ENexusCategory, DockItem>
    {
    }
    
    [Serializable]
    public sealed class DockCategoryToTutorialHighlighter : SerializableDictionary<ENexusCategory, GameObject>
    {
    }
}