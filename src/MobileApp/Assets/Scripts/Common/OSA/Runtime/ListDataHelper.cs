using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Com.ForbiddenByte.OSA.Core;

namespace Sotbify.MobileApp.Common.OSA
{
    public class ListDataHelper<TItem> : IList<TItem>, IReadOnlyList<TItem>
    {
        private readonly List<TItem> _list = new();
        private readonly IOSA _adapter;
        private readonly bool _keepVelocityOnCountChange;
        private readonly bool _freezeEndEdge;

        public ListDataHelper(IOSA adapter, bool freezeEndEdge = false, bool keepVelocityOnCountChange = true)
        {
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            _freezeEndEdge = freezeEndEdge;
            _keepVelocityOnCountChange = keepVelocityOnCountChange;
        }

        public int Count => _list.Count;
        public bool IsReadOnly => false;

        public TItem this[int index]
        {
            get => _list[index];
            set
            {
                _list[index] = value;
                _adapter.ResetItems(_list.Count, false, _keepVelocityOnCountChange);
            }
        }

        public void Add(TItem item)
            => InsertOneAtEndInternal(item, _freezeEndEdge);

        public void AddStart(TItem item)
            => InsertOneAtStartInternal(item, _freezeEndEdge);

        public void AddRange(IReadOnlyCollection<TItem> items) =>
            InsertItemsAtEndInternal(items, _freezeEndEdge);
        
        public void AddRange(IReadOnlyCollection<TItem> items, bool freezeEndEdge, bool keepVelocityOnCountChange) =>
            InsertItemsAtEndInternal(items, freezeEndEdge, keepVelocityOnCountChange);

        public void Insert(int index, TItem item) =>
            InsertOneInternal(index, item, _freezeEndEdge, _keepVelocityOnCountChange);
        
        public void Insert(int index, TItem item, bool freezeEndEdge, bool keepVelocityOnCountChange) =>
            InsertOneInternal(index, item, freezeEndEdge, keepVelocityOnCountChange);

        public void InsertRange(int index, IReadOnlyCollection<TItem> items) =>
            InsertItemsInternal(index, items, _freezeEndEdge);

        public bool Contains(TItem item) =>
            _list.Contains(item);

        public int IndexOf(TItem item) =>
            _list.IndexOf(item);

        public void CopyTo(TItem[] array, int arrayIndex) =>
            _list.CopyTo(array, arrayIndex);

        public bool Remove(TItem item) =>
            RemoveItemInternal(item, _freezeEndEdge);

        public void RemoveAt(int index) =>
            RemoveOneInternal(index, _freezeEndEdge);

        public void Clear() =>
            ClearInternal();

        public IEnumerator<TItem> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

        #region Insert internal

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InsertOneAtStartInternal(TItem item, bool freezeEndEdge = false) =>
            InsertOneInternal(0, item, freezeEndEdge);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InsertOneAtEndInternal(TItem item, bool freezeEndEdge = false) =>
            InsertOneInternal(_list.Count, item, freezeEndEdge);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InsertOneInternal(int index, TItem item, bool freezeEndEdge = false, bool keepVelocityOnCountChange = true)
        {
            _list.Insert(index, item);

            if (_adapter.InsertAtIndexSupported)
            {
                _adapter.InsertItems(index, 1, freezeEndEdge, keepVelocityOnCountChange);
                return;
            }

            _adapter.ResetItems(_list.Count, freezeEndEdge, keepVelocityOnCountChange);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InsertItemsAtStartInternal(IReadOnlyCollection<TItem> items, bool freezeEndEdge = false) =>
            InsertItemsInternal(0, items, freezeEndEdge);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InsertItemsAtEndInternal(IReadOnlyCollection<TItem> items, bool freezeEndEdge = false, bool keepVelocityOnCountChange = false) =>
            InsertItemsInternal(_list.Count, items, freezeEndEdge, keepVelocityOnCountChange);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InsertItemsInternal(int index, IReadOnlyCollection<TItem> items, bool freezeEndEdge = false, bool keepVelocityOnCountChange = false)
        {
            _list.InsertRange(index, items);

            if (_adapter.InsertAtIndexSupported)
            {
                _adapter.InsertItems(index, items.Count, freezeEndEdge, keepVelocityOnCountChange);
                return;
            }

            _adapter.ResetItems(_list.Count, freezeEndEdge, keepVelocityOnCountChange);
        }

        #endregion

        #region Remove internal

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool RemoveItemInternal(TItem item, bool freezeEndEdge = false)
        {
            if (_list.Remove(item) == false)
                return false;

            _adapter.ResetItems(_list.Count, freezeEndEdge, _keepVelocityOnCountChange);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveOneFromStartInternal(bool freezeEndEdge = false) =>
            RemoveOneInternal(0, freezeEndEdge);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveOneFromEndInternal(bool freezeEndEdge = false) =>
            RemoveOneInternal(_list.Count - 1, freezeEndEdge);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveOneInternal(int index, bool freezeEndEdge = false)
        {
            _list.RemoveAt(index);

            if (_adapter.RemoveFromIndexSupported)
            {
                _adapter.RemoveItems(index, 1, freezeEndEdge, _keepVelocityOnCountChange);
                return;
            }

            _adapter.ResetItems(_list.Count, freezeEndEdge, _keepVelocityOnCountChange);
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClearInternal()
        {
            _list.Clear();
            _adapter.ResetItems(_list.Count, _keepVelocityOnCountChange);
        }
    }
}