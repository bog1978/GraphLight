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

        public PriorityQueue(IEnumerable<TValue> items, HeapType heapType)
        {
            _heap = new BinaryHeap<TKey, TValue>(items, heapType);
        }

        public bool IsEmpty
        {
            get { return _heap.Count == 0; }
        }

        #region ICollection<TValue> Members

        public IEnumerator<TValue> GetEnumerator()
        {
            return _heap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _heap.GetEnumerator();
        }

        public void Add(TValue item)
        {
            _heap.Add(item);
        }

        public void Clear()
        {
            _heap.Clear();
        }

        public bool Contains(TValue item)
        {
            return _heap.Contains(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            _heap.CopyTo(array, arrayIndex);
        }

        public bool Remove(TValue item)
        {
            return _heap.Remove(item);
        }

        public int Count
        {
            get { return _heap.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        public void Enqueue(TValue item)
        {
            _heap.Add(item);
        }

        public TValue Dequeue()
        {
            return _heap.RemoveRoot();
        }

        public TValue Peek()
        {
            return _heap.Root;
        }
    }
}
