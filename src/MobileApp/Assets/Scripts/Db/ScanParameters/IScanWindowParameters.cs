using System.Collections.Generic;
using Sotbify.MobileApp.Utils.Enums;
using UnityEngine;

namespace Sotbify.MobileApp.Db.ScanParameters
{
    public interface IScanWindowParameters
    {
        public IReadOnlyDictionary<EScanWindowState, Color> ScanWindowStateToFrameCornersColor { get; }
        public IReadOnlyDictionary<EScanWindowState, Color> ScanWindowStateToFrameColor { get; }
        public IReadOnlyDictionary<EScanWindowState, Color> ScanWindowStateToBackgroundColor { get; }

        public Color InitialScanZoneBackgroundColor { get; }
        public Color DeniedCameraColor { get; }
        public Color DeniedCameraIconColor { get; }
        public Color InitialCameraIconColor { get; }
        public string EditorQRCode { get; }
        public float AfterScanAnimationDelay { get; }
        public Vector2Int MaxCameraResolution { get; }
        public float ScanDelay { get; }
    }
}