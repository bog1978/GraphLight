using System;
using System.Collections;
using System.Collections.Generic;

namespace GraphLight.Collections
{
    public class PriorityQueue2<TElement, TPriority> : IReadOnlyCollection<TElement>
        where TPriority : IComparable<TPriority>
    {
        #region Константы и поля

        private readonly BinaryHeap2<TElement, TPriority> _heap;

        #endregion

        #region Конструкторы

        //public PriorityQueue2(HeapType heapType)
        //{
        //    _heap = new BinaryHeap2<TElement, TPriority>(heapType);
        //}

        public PriorityQueue2(IEnumerable<TElement> items, Func<TElement, TPriority> priorityFunc, HeapType heapType) =>
            _heap = new BinaryHeap2<TElement, TPriority>(items, priorityFunc, heapType);

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region IEnumerable<TElement>

        public IEnumerator<TElement> GetEnumerator() => _heap.GetEnumerator();

        #endregion

        #region IReadOnlyCollection<TElement>

        public int Count => _heap.Count;

        #endregion

        #region Другое

        public TElement Dequeue() => _heap.RemoveRoot();

        public void Enqueue(TElement item, TPriority priority) => _heap.Add(priority, item);

        public TElement Peek() => _heap.Root;

        #endregion
    }
}