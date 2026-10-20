using System;
using UnityEngine;

namespace Sotbify.MobileApp.ChatMessages.Models
{
    public class UserMessageModel : IChatItemModel, IPendingSizeChange
    {
        private string _message;
        public int ID { get; set; }
        public bool IsPendingSizeChange { get; private set; }
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
                if (_message == value || value == string.Empty)
                    return;
                
                _message = value;
                IsPendingSizeChange = true;
            }
        }

        public UserMessageModel()
        {
            CachedType = GetType();
        }

    }
}