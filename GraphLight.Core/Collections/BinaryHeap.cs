using System;
using System.Collections;
using System.Collections.Generic;

namespace GraphLight.Collections
{
    public class BinaryHeap<TKey,TValue> : IEnumerable<TValue>
        where TValue : IBinaryHeapItem<TKey>
        where TKey : IComparable, IComparable<TKey>
    {
        private readonly List<TValue> _heap;
        private readonly int _sgn;

        public BinaryHeap(IEnumerable<TValue> items, HeapType heapType)
        {
            _sgn = heapType == HeapType.Min ? -1 : 1;
            _heap = new List<TValue>(items);
            var i = 0;
            foreach (var value in _heap)
                value.HeapIndex = i++;
            buildHeap();
        }

        #region ICollection<TValue> Members

        public void Add(TValue item)
        {
            item.HeapIndex = _heap.Count;
            _heap.Add(item);
            for (int i = _heap.Count - 1, p = parent(i);
                 i > 0 && compare(p, i) < 0;
                 i = p, p = parent(i))
                swap(p, i);
        }

        public bool Remove(TValue item)
        {
            var count = _heap.Count;
            var i = item.HeapIndex;
            if (i < 0 || i >= count)
                return false;

            var last = count - 1;
            _heap[i].HeapIndex = -1;
            _heap[last].HeapIndex = i;
            _heap[i] = _heap[last];
            _heap.RemoveAt(last);
            heapify(i);
            return true;
        }

        public int Count => _heap.Count;

        public IEnumerator<TValue> GetEnumerator() => _heap.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _heap.GetEnumerator();

        #endregion

        public TValue Root => _heap.Count == 0
            ? throw new InvalidOperationException("Heap is empty.")
            : _heap[0];

        public TValue RemoveRoot()
        {
            var root = Root;
            var last = _heap.Count - 1;
            _heap[0].HeapIndex = -1;
            _heap[last].HeapIndex = 0;
            _heap[0] = _heap[last];
            _heap.RemoveAt(last);
            heapify(0);
            return root;
        }

        private void buildHeap()
        {
            for (var i = (_heap.Count >> 1) - 1; i >= 0; i--)
                heapify(i);
        }

        private void heapify(int i)
        {
            var l = left(i);
            var r = right(i);
            var count = _heap.Count;
            var largest = l < count && compare(l, i) > 0 ? l : i;
            if (r < count && compare(r, largest) > 0)
                largest = r;
            if (largest == i)
                return;
            swap(i, largest);
            heapify(largest);
        }

        private int compare(int i, int j) => _sgn * _heap[i].HeapKey.CompareTo(_heap[j].HeapKey);

        private void swap(int i, int j)
        {
            var a = _heap[i];
            var b = _heap[j];
            _heap[i] = b;
            _heap[j] = a;
            a.HeapIndex = j;
            b.HeapIndex = i;
        }

        private static int parent(int i) => (i - 1) >> 1;

        private static int left(int i) => (i << 1) + 1;

        private static int right(int i) => (i << 1) + 2;
    }
}