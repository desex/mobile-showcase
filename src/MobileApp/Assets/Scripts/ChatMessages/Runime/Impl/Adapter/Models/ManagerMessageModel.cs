using System;
using Sotbify.MobileApp.Utils.Enums;
using UnityEngine;

namespace Sotbify.MobileApp.ChatMessages.Models
{
    public class ManagerMessageModel : IChatItemModel, IPendingSizeChange
    {
        private string _message;
        public int ID { get; set; }
        public bool IsPendingSizeChange { get; private set; }
        public int ManagerID;
        public EMessageContentType ContentType;
        public Sprite Image;
        public DateTime CreatedAt { get; set; }
        public bool FreezeEndEdge { get; set; }
        public bool KeepVelocityOnCountChange { get; set; }
        public Vector2 CachedSize { get; set; }
        public Type CachedType { get; }

        public string Message
        {
            get => _message;
            set
            {
                if (_message == value)
                    return;
                
                _message = value;
                IsPendingSizeChange = true;
            }
        }
        

        public ManagerMessageModel()
        {
            CachedType = GetType();
        }

    }
}