using System;
using UnityEngine;

namespace Sotbify.MobileApp.Common.SerializableTuple
{
    [Serializable]
    public class SerializableTuple<T1, T2>
    {
        [SerializeField] private T1 _item1;
        [SerializeField] private T2 _item2;

        public T1 Item1
        {
            get => _item1;
            set => _item1 = value;
        }

        public T2 Item2
        {
            get => _item2;
            set => _item2 = value;
        }

        public SerializableTuple() { }

        public SerializableTuple(T1 item1, T2 item2)
        {
            _item1 = item1;
            _item2 = item2;
        }

        public void Deconstruct(out T1 item1, out T2 item2)
        {
            item1 = _item1;
            item2 = _item2;
        }

        public static implicit operator ValueTuple<T1, T2>(SerializableTuple<T1, T2> tuple)
        {
            return tuple == null ? default : (item1: tuple._item1, item2: tuple._item2);
        }

        public static implicit operator SerializableTuple<T1, T2>(ValueTuple<T1, T2> tuple)
        {
            return new SerializableTuple<T1, T2>(tuple.Item1, tuple.Item2);
        }

        public override string ToString() => $"({_item1}, {_item2})";
    }

    public static class SerializableTuple
    {
        public static SerializableTuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
        {
            return new SerializableTuple<T1, T2>(item1, item2);
        }
    }
}