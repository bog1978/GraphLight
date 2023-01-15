using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GraphLight.Collections
{
    public class BinaryHeap2<TValue, TKey> : IEnumerable<TValue>
        where TKey : IComparable<TKey>
    {
        #region Константы и поля

        private readonly IDictionary<TValue, HeapItem> _map;
        private readonly int _sgn;
        private readonly List<HeapItem> _heap;

        #endregion

        #region Конструкторы

        public BinaryHeap2(IEnumerable<TValue> items, Func<TValue, TKey> priorityFunc, HeapType heapType)
        {
            _sgn = heapType == HeapType.Min
                ? -1
                : 1;
            _heap = items
               .Select((x, i) => new HeapItem(x, priorityFunc(x), i))
               .ToList();
            _map = _heap.ToDictionary(x => x.Element);
            BuildHeap();
        }

        #endregion

        #region События, свойства, индексаторы

        public int Count => _heap.Count;

        public TValue Root =>
            _heap.Count == 0
                ? throw new InvalidOperationException("Heap is empty.")
                : _heap[0].Element;

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region IEnumerable<TValue>

        public IEnumerator<TValue> GetEnumerator() => new HeapEnumerator(_heap.GetEnumerator());

        #endregion

        #region Другое

        private static int Left(int i) => (i << 1) + 1;

        private static int Parent(int i) => (i - 1) >> 1;

        private static int Right(int i) => (i << 1) + 2;

        public void Add(TKey key, TValue value)
        {
            var item = new HeapItem(value, key, _heap.Count);
            _heap.Add(item);
            _map.Add(value, item);
            for (int i = _heap.Count - 1, p = Parent(i);
                 i > 0 && Compare(p, i) < 0;
                 i = p, p = Parent(i))
                Swap(p, i);
        }

        public bool Remove(TValue element)
        {
            if (!_map.TryGetValue(element, out var item))
                return false;
            var count = _heap.Count;
            var i = item.HeapIndex;
            if (i < 0 || i >= count)
                return false;

            var last = count - 1;
            _heap[i].HeapIndex = -1;
            _heap[last].HeapIndex = i;
            _heap[i] = _heap[last];
            _heap.RemoveAt(last);
            _map.Remove(element);
            Heapify(i);
            return true;
        }

        public TValue RemoveRoot()
        {
            var root = Root;
            var last = _heap.Count - 1;
            _heap[0].HeapIndex = -1;
            _heap[last].HeapIndex = 0;
            _heap[0] = _heap[last];
            _heap.RemoveAt(last);
            _map.Remove(root);
            Heapify(0);
            return root;
        }

        private void BuildHeap()
        {
            for (var i = (_heap.Count >> 1) - 1; i >= 0; i--)
                Heapify(i);
        }

        private int Compare(int i, int j) => _sgn * _heap[i].HeapKey.CompareTo(_heap[j].HeapKey);

        private void Heapify(int i)
        {
            var l = Left(i);
            var r = Right(i);
            var count = _heap.Count;
            var largest = l < count && Compare(l, i) > 0
                ? l
                : i;
            if (r < count && Compare(r, largest) > 0)
                largest = r;
            if (largest == i)
                return;
            Swap(i, largest);
            Heapify(largest);
        }

        private void Swap(int i, int j)
        {
            var a = _heap[i];
            var b = _heap[j];
            _heap[i] = b;
            _heap[j] = a;
            a.HeapIndex = j;
            b.HeapIndex = i;
        }

        #endregion

        private class HeapEnumerator : IEnumerator<TValue>
        {
            #region Константы и поля

            private readonly IEnumerator<HeapItem> _enumerator;

            #endregion

            #region Конструкторы

            public HeapEnumerator(IEnumerator<HeapItem> enumerator)
            {
                _enumerator = enumerator;
            }

            #endregion

            #region IDisposable

            public void Dispose() => _enumerator.Dispose();

            #endregion

            #region IEnumerator

            public bool MoveNext() => _enumerator.MoveNext();

            public void Reset() => _enumerator.Reset();

            object? IEnumerator.Current => Current;

            #endregion

            #region IEnumerator<TElement>

            public TValue Current => _enumerator.Current.Element;

            #endregion
        }

        private class HeapItem
        {
            #region Константы и поля

            public readonly TKey HeapKey;

            public readonly TValue Element;

            public int HeapIndex;

            #endregion

            #region Конструкторы

            public HeapItem(TValue element, TKey heapKey, int index)
            {
                Element = element;
                HeapKey = heapKey;
                HeapIndex = index;
            }

            #endregion
        }
    }
}