using System;
using System.Collections;
using System.Collections.Generic;

namespace GraphLight.Collections
{
    public class PriorityQueue<TKey, TValue> : ICollection<TValue>
        where TKey : IComparable, IComparable<TKey>
        where TValue : IBinaryHeapItem<TKey>
    {
        private readonly BinaryHeap<TKey, TValue> _heap;

        public PriorityQueue(HeapType heapType)
            : this(new TValue[] { }, heapType)
        {
        }

        public PriorityQueue(IEnumerable<TValue> items, HeapType heapType) => 
            _heap = new BinaryHeap<TKey, TValue>(items, heapType);

        public bool IsEmpty => _heap.Count == 0;

        #region ICollection<TValue> Members

        public IEnumerator<TValue> GetEnumerator() => _heap.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _heap.GetEnumerator();

        public void Add(TValue item) => _heap.Add(item);

        public void Clear() => _heap.Clear();

        public bool Contains(TValue item) => _heap.Contains(item);

        public void CopyTo(TValue[] array, int arrayIndex) => _heap.CopyTo(array, arrayIndex);

        public bool Remove(TValue item) => _heap.Remove(item);

        public int Count => _heap.Count;

        public bool IsReadOnly => false;

        #endregion

        public void Enqueue(TValue item) => _heap.Add(item);

        public TValue Dequeue() => _heap.RemoveRoot();

        public TValue Peek() => _heap.Root;
    }
}
