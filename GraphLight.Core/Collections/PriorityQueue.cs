using System;
using System.Collections;
using System.Collections.Generic;

namespace GraphLight.Collections
{
    internal class PriorityQueue<TKey, TValue> : IEnumerable<TValue>
        where TKey : IComparable<TKey>
    {
        #region Константы и поля

        private readonly BinaryHeap<TKey, TValue> _heap;

        #endregion

        #region Конструкторы

        public PriorityQueue(Func<TValue, TKey> priorityFunc, HeapType heapType)
            : this(new TValue[] { }, priorityFunc, heapType)
        {
        }

        public PriorityQueue(IEnumerable<TValue> items, Func<TValue, TKey> priorityFunc, HeapType heapType) =>
            _heap = new BinaryHeap<TKey, TValue>(items, priorityFunc, heapType);

        #endregion

        #region События, свойства, индексаторы

        public int Count => _heap.Count;

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region IEnumerable<TValue>

        public IEnumerator<TValue> GetEnumerator() => _heap.GetEnumerator();

        #endregion

        #region Другое

        public TValue Dequeue() => _heap.RemoveRoot();

        public void Enqueue(TValue item, TKey priority) => _heap.Add(priority, item);

        public TValue Peek() => _heap.Root;

        public bool Remove(TValue item) => _heap.Remove(item);

        #endregion
    }
}