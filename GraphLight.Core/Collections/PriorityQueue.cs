using System;
using System.Collections;
using System.Collections.Generic;

namespace GraphLight.Collections
{
    public class PriorityQueue<TKey, TValue> : IEnumerable<TValue>
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

        public bool Remove(TValue item) => _heap.Remove(item);

        #endregion

        public void Enqueue(TValue item) => _heap.Add(item);

        public TValue Dequeue() => _heap.RemoveRoot();

        public TValue Peek() => _heap.Root;
    }
}
