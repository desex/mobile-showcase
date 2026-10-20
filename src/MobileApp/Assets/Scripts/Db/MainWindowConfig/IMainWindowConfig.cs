using LitMotion;
using Sotbify.MobileApp.Utils;
using UnityEngine;

namespace Sotbify.MobileApp.Db.MainWindowConfig
{
    public interface IMainWindowConfig
    {
        public float FeedPreloadIndent { get; }
        public Vector2 MoveRange { get; }
        public Vector2 RotationRange { get; }
        public Vector2 MotionDurationRange { get; }
        public float DelayBetweenMotions { get; }
        public Ease MoveEase { get; }
        public MotionParameters ScrollMotionParameters { get; }
    }
}