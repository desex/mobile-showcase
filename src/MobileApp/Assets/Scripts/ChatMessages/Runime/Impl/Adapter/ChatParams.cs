using System;
using Com.ForbiddenByte.OSA.Core;
using UnityEngine;

namespace Sotbify.MobileApp.ChatMessages
{
    [Serializable]
    public class ChatParams : BaseParams
    {
        public RectTransform UserMessagePrefab;
        public RectTransform ManagerMessagePrefab;
        public RectTransform BlackjackPrefab;
        public RectTransform DateElementPrefab;
            
        public override void InitIfNeeded(IOSA iAdapter)
        {
            base.InitIfNeeded(iAdapter);

            AssertValidWidthHeight(UserMessagePrefab);
            AssertValidWidthHeight(ManagerMessagePrefab);
            AssertValidWidthHeight(BlackjackPrefab);
            AssertValidWidthHeight(DateElementPrefab);
            
            DefaultItemSize = Mathf.Min(UserMessagePrefab.rect.height, ManagerMessagePrefab.rect.height);
        }
    }
}