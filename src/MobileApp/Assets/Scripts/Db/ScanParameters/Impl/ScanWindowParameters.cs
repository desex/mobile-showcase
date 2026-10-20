using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sotbify.MobileApp.Utils.Enums;
using UnityEngine;

namespace Sotbify.MobileApp.Db.ScanParameters
{
    [CreateAssetMenu(menuName = "Settings/" + nameof(ScanWindowParameters), fileName = nameof(ScanWindowParameters))]
    public class ScanWindowParameters : ScriptableParameters, IScanWindowParameters
    {
        [SerializeField] private EScanStateToColor _scanWindowStateToFrameCornersColor;
        [SerializeField] private EScanStateToColor _scanWindowStateToFrameColor;
        [SerializeField] private EScanStateToColor _scanWindowStateToBackgroundColor;
        [field: SerializeField] public Color InitialScanZoneBackgroundColor { get; private set; }
        [field: SerializeField] public Color DeniedCameraColor { get; private set; }
        [field: SerializeField] public Color DeniedCameraIconColor { get; private set; }
        [field: SerializeField] public Color InitialCameraIconColor { get; private set; }
        [field: SerializeField] public string EditorQRCode { get; private set; }
        [field: SerializeField] public float AfterScanAnimationDelay { get; private set; } = 999999;

        [field: SerializeField]
        [field: MinMaxSlider(0, 2048, true)]
        public Vector2Int MaxCameraResolution { get; private set; }

        [field: SerializeField] public float ScanDelay { get; private set; } = 1f;
        
        public IReadOnlyDictionary<EScanWindowState, Color> ScanWindowStateToFrameCornersColor => _scanWindowStateToFrameCornersColor;
        public IReadOnlyDictionary<EScanWindowState, Color> ScanWindowStateToFrameColor => _scanWindowStateToFrameColor;

        public IReadOnlyDictionary<EScanWindowState, Color> ScanWindowStateToBackgroundColor =>
            _scanWindowStateToBackgroundColor;
    }
}