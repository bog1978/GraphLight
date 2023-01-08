using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GraphLight.Collections
{
    public class BinaryHeap2<TElement, TPriority> : IReadOnlyCollection<TElement>
        where TPriority : IComparable<TPriority>
    {
        private readonly List<HeapItem> _heap;
        private readonly int _sgn;

        public BinaryHeap2(HeapType heapType)
        {
            _sgn = heapType == HeapType.Min ? -1 : 1;
            _heap = new List<HeapItem>();
        }

        public BinaryHeap2(IEnumerable<TElement> items, Func<TElement, TPriority> priorityFunc, HeapType heapType)
        {
            _sgn = heapType == HeapType.Min ? -1 : 1;
            var heapItems = items.Select(x => new HeapItem(x, priorityFunc(x)));
            _heap = new List<HeapItem>(heapItems);
            BuildHeap();
        }

        #region ICollection<TValue> Members

        public void Add(TElement element, TPriority priority)
        {
            _heap.Add(new HeapItem(element, priority));
            for (int i = _heap.Count - 1, p = Parent(i);
                 i > 0 && Compare(p, i) < 0;
                 i = p, p = Parent(i))
                Swap(p, i);
        }

        public int Count => _heap.Count;

        public IEnumerator<TElement> GetEnumerator() => new HeapEnumerator(_heap.GetEnumerator());

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        public TElement Root => _heap.Count == 0
            ? throw new InvalidOperationException("Heap is empty.")
            : _heap[0].Element;

        public TElement RemoveRoot()
        {
            var root = Root;
            var last = _heap.Count - 1;
            _heap[0] = _heap[last];
            _heap.RemoveAt(last);
            Heapify(0);
            return root;
        }

        private void BuildHeap()
        {
            for (var i = (_heap.Count >> 1) - 1; i >= 0; i--)
                Heapify(i);
        }

        private void Heapify(int i)
        {
            var l = Left(i);
            var r = Right(i);
            var count = _heap.Count;
            var largest = l < count && Compare(l, i) > 0 ? l : i;
            if (r < count && Compare(r, largest) > 0)
                largest = r;
            if (largest == i)
                return;
            Swap(i, largest);
            Heapify(largest);
        }

        private int Compare(int i, int j) => _sgn * _heap[i].Priority.CompareTo(_heap[j].Priority);

        private void Swap(int i, int j)
        {
            var a = _heap[i];
            var b = _heap[j];
            _heap[i] = b;
            _heap[j] = a;
        }

        private static int Parent(int i) => (i - 1) >> 1;

        private static int Left(int i) => (i << 1) + 1;

        private static int Right(int i) => (i << 1) + 2;

        private class HeapItem
        {
            public HeapItem(TElement element, TPriority priority)
            {
                Element = element;
                Priority = priority;
            }

            public readonly TElement Element;
            public readonly TPriority Priority;
        }

        private class HeapEnumerator : IEnumerator<TElement>
        {
            private readonly IEnumerator<HeapItem> _enumerator;

            public HeapEnumerator(IEnumerator<HeapItem> enumerator)
            {
                _enumerator = enumerator;
            }

            public void Dispose() => _enumerator.Dispose();

            public bool MoveNext() => _enumerator.MoveNext();

            public void Reset() => _enumerator.Reset();

            public TElement Current => _enumerator.Current.Element;

            object? IEnumerator.Current => Current;
        }
    }
}