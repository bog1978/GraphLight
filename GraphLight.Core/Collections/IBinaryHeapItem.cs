using System;

namespace GraphLight.Collections
{
    public interface IBinaryHeapItem<TKey>
        where TKey : IComparable, IComparable<TKey>
    {
        int HeapIndex { get; set; }
        TKey HeapKey { get; set; }
    }
}