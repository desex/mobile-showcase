using System;
using UnityEngine;

namespace Sotbify.MobileApp.Utils.UDateTime
{
    [Serializable]
    public class UDateTime : ISerializationCallbackReceiver {
        
        protected bool Equals(UDateTime other)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public override int GetHashCode()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        [HideInInspector] public DateTime DateTime;

        [HideInInspector] [SerializeField] private string _dateTime;

        public static implicit operator DateTime(UDateTime udt) 
        {
            return udt.DateTime;
        }

        public static implicit operator UDateTime(DateTime dt) 
        {
            return new UDateTime {DateTime = dt};
        }

        public static bool operator >(UDateTime udt1, UDateTime udt2)
        {
            return udt1.DateTime > udt2.DateTime;
        }

        public static bool operator <(UDateTime udt1, UDateTime udt2)
        {
            return udt1.DateTime < udt2.DateTime;
        }

        public static bool operator ==(UDateTime udt1, UDateTime udt2)
        {
            return udt1.DateTime == udt2.DateTime;
        }

        public static bool operator !=(UDateTime udt1, UDateTime udt2)
        {
            return !(udt1 == udt2);
        }


        public void OnAfterDeserialize() {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void OnBeforeSerialize() {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}