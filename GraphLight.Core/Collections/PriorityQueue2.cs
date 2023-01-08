using System;
using System.Collections;
using System.Collections.Generic;

namespace GraphLight.Collections
{
    public class PriorityQueue2<TElement, TPriority> : IReadOnlyCollection<TElement>
        where TPriority : IComparable<TPriority>
    {
        private readonly BinaryHeap2<TElement, TPriority> _heap;

        public PriorityQueue2(HeapType heapType)
        {
            _heap = new BinaryHeap2<TElement, TPriority>(heapType);
        }

        public PriorityQueue2(IEnumerable<TElement> items, Func<TElement, TPriority> priorityFunc, HeapType heapType) =>
            _heap = new BinaryHeap2<TElement, TPriority>(items, priorityFunc, heapType);

        public void Enqueue(TElement item, TPriority priority) => _heap.Add(item, priority);

        public TElement Dequeue() => _heap.RemoveRoot();

        public TElement Peek() => _heap.Root;

        public IEnumerator<TElement> GetEnumerator() => _heap.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => _heap.Count;
    }
}