using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using Sotbify.MobileApp.Utils.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.Utils.Extensions
{
    public static class LMotionExtension
    {
        public static UniTask CreateOpacityTask
        (
            this CanvasGroup canvasGroup,
            float from,
            float to,
            MotionParameters motionParameters,
            CancelBehavior cancelBehavior = CancelBehavior.Cancel,
            CancellationToken token = default
        )
        {
            return LMotion
                .Create(from, to, motionParameters.Time)
                .WithEase(motionParameters.Ease)
                .Bind(value => canvasGroup.alpha = value)
                .AddTo(canvasGroup)
                .ToUniTask(cancelBehavior, token);
        }
        
        public static UniTask CreateOpacityTask(
            this Image image,
            float from,
            float to,
            MotionParameters motionParameters,
            CancelBehavior cancelBehavior = CancelBehavior.Cancel,
            CancellationToken token = default
        )
        {
            return image.CreateGraphicColorAnimation(
                image.color.ChangeAlpha(from),
                image.color.ChangeAlpha(to),
                motionParameters,
                cancelBehavior,
                token);
        }
        
        public static IEnumerable<UniTask> CreateOpacityTask(
            this IEnumerable<CanvasGroup> canvasGroupObjects,
            float from,
            float to,
            MotionParameters motionParameters,
            CancelBehavior cancelBehavior = CancelBehavior.Cancel,
            CancellationToken token = default
        )
        {
            var opacityTasks = new List<UniTask>();
            
            foreach (var canvasGroup in canvasGroupObjects)
                opacityTasks.Add(canvasGroup.CreateOpacityTask(from, to, motionParameters, cancelBehavior, token));

            return opacityTasks;
        }
        
        public static UniTask CreateGraphicColorAnimation(
            this Graphic graphic,
            Color from,
            Color to,
            MotionParameters motionParameters,
            CancelBehavior cancelBehavior = CancelBehavior.Cancel,
            CancellationToken token = default
        )
        {
            return LMotion
                .Create(from, to, motionParameters.Time)
                .WithEase(motionParameters.Ease)
                .Bind(value => graphic.color = value)
                .AddTo(graphic)
                .ToUniTask(cancelBehavior, token);
        }
        
        public static UniTask CreateGraphicColorAnimation(
            this Graphic graphic,
            Color from,
            Color to,
            Ease ease,
            float duration,
            CancelBehavior cancelBehavior = CancelBehavior.Cancel,
            CancellationToken token = default
        )
        {
            return LMotion
                .Create(from, to, duration)
                .WithEase(ease)
                .Bind(value => graphic.color = value)
                .AddTo(graphic)
                .ToUniTask(cancelBehavior, token);
        }

        public static UniTask CreateAnchoredPositionAnimation(
            this RectTransform transform,
            Vector2 from,
            Vector2 to,
            MotionParameters motionParameters,
            CancelBehavior cancelBehavior = CancelBehavior.Cancel,
            CancellationToken token = default
        )
        {
            return LMotion
                .Create(from, to, motionParameters.Time)
                .WithEase(motionParameters.Ease)
                .BindToAnchoredPosition(transform)
                .AddTo(transform)
                .ToUniTask(cancelBehavior, token);
        }
        
        public static UniTask CreateLocalPositionAnimation(
            this RectTransform transform,
            Vector3 from,
            Vector3 to,
            MotionParameters motionParameters,
            CancelBehavior cancelBehavior = CancelBehavior.Cancel,
            CancellationToken token = default
        )
        {
            return LMotion
                .Create(from, to, motionParameters.Time)
                .WithEase(motionParameters.Ease)
                .BindToLocalPosition(transform)
                .AddTo(transform)
                .ToUniTask(cancelBehavior, token);
        }
        
        public static UniTask CreateLocalPositionAnimation(
            this Transform transform,
            Vector3 from,
            Vector3 to,
            MotionParameters motionParameters,
            CancelBehavior cancelBehavior = CancelBehavior.Cancel,
            CancellationToken token = default
        )
        {
            return LMotion
                .Create(from, to, motionParameters.Time)
                .WithEase(motionParameters.Ease)
                .BindToLocalPosition(transform)
                .AddTo(transform)
                .ToUniTask(cancelBehavior, token);
        }
        
        public static UniTask CreateLocalPositionAnimation(
            this Transform transform,
            Vector3 from,
            Vector3 to,
            float time,
            Ease ease,
            CancelBehavior cancelBehavior = CancelBehavior.Cancel,
            CancellationToken token = default
        )
        {
            return LMotion
                .Create(from, to, time)
                .WithEase(ease)
                .BindToLocalPosition(transform)
                .AddTo(transform)
                .ToUniTask(cancelBehavior, token);
        }
        
        public static UniTask CreateSizeDeltaAnimation(
            this RectTransform transform,
            Vector2 from,
            Vector2 to,
            MotionParameters motionParameters,
            CancelBehavior cancelBehavior = CancelBehavior.Cancel,
            CancellationToken token = default
        )
        {
            return LMotion
                .Create(from, to, motionParameters.Time)
                .WithEase(motionParameters.Ease)
                .BindToSizeDelta(transform)
                .AddTo(transform)
                .ToUniTask(cancelBehavior, token);
        }
        
        public static UniTask CreateRotationAnimation(
            this RectTransform transform,
            Vector3 from,
            Vector3 to,
            MotionParameters motionParameters,
            CancelBehavior cancelBehavior = CancelBehavior.Cancel,
            CancellationToken token = default
        )
        {
            return LMotion
                .Create(from, to, motionParameters.Time)
                .WithEase(motionParameters.Ease)
                .BindToLocalEulerAngles(transform)
                .AddTo(transform)
                .ToUniTask(cancelBehavior,token);
        }
        
        public static UniTask CreateRotationAnimation(
            this RectTransform transform,
            Vector3 from,
            Vector3 to,
            float time,
            Ease ease,
            CancelBehavior cancelBehavior = CancelBehavior.Cancel,
            CancellationToken token = default
        )
        {
            return LMotion
                .Create(from, to, time)
                .WithEase(ease)
                .BindToLocalEulerAngles(transform)
                .AddTo(transform)
                .ToUniTask(cancelBehavior,token);
        }

        public static UniTask CreatePaddingAnimation(
            this LayoutGroup layoutGroup,
            int from,
            int to,
            EDirection direction,
            MotionParameters motionParameters,
            CancelBehavior cancelBehavior = CancelBehavior.Cancel,
            CancellationToken cancellationToken = default
        )
        {
            var builder = LMotion
                .Create(from, to, motionParameters.Time)
                .WithEase(motionParameters.Ease);

            MotionHandle? motionHandle = direction switch
            {
                EDirection.Top => builder.Bind(value => layoutGroup.padding.top = value).AddTo(layoutGroup),
                EDirection.Right => builder.Bind(value => layoutGroup.padding.right = value).AddTo(layoutGroup),
                EDirection.Bottom => builder.Bind(value => layoutGroup.padding.bottom = value).AddTo(layoutGroup),
                EDirection.Left => builder.Bind(value => layoutGroup.padding.left = value).AddTo(layoutGroup),
                _ => null
            };
            
            return motionHandle!.Value.ToUniTask(cancelBehavior,cancellationToken);
        }
    }
}