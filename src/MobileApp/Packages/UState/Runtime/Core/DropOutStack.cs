using System;

namespace UState
{
    public class DropOutStack<T>
    {
        private readonly T[] _items;

        private int _topIndex;
        private int _count;

        public DropOutStack(int capacity)
        {
            if(capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero");
                
            _items = new T[capacity];
        }

        public T Peek()
        {
            if(_count <= 0)
                throw new InvalidOperationException("Stack is empty");
            
            var topIndex = (_items.Length + _topIndex - 1) % _items.Length;
            return _items[topIndex];
        }
        
        public bool TryPeek(out T item)
        {
            if (_count <= 0)
            {
                item = default;
                return false;
            }
            
            var topIndex = (_items.Length + _topIndex - 1) % _items.Length;
            item = _items[topIndex];
            return true;
        }

        public T Pop()
        {
            if(_count <= 0)
                throw new InvalidOperationException("Stack is empty");
            
            _topIndex = (_items.Length + _topIndex - 1) % _items.Length;
            var item = _items[_topIndex];
            _count--;

            return item;
        }

        public bool TryPop(out T item)
        {
            if (_count <= 0)
            {
                item = default;
                return false;
            }
            
            _topIndex = (_items.Length + _topIndex - 1) % _items.Length;
            item = _items[_topIndex];
            _count--;
            
            return true;
        }

        public void Push(T item)
        {
            _items[_topIndex] = item;
            _topIndex = (_topIndex + 1) % _items.Length;

            if (_count < _items.Length)
                _count++;
        }

        public void Clear()
        {
            Array.Clear(_items, 0, _items.Length);
            _topIndex = 0;
            _count = 0;
        }
    }
}