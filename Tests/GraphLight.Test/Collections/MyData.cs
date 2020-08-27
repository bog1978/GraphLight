using System.Diagnostics;
using GraphLight.Collections;

namespace GraphLight.Test.Collections
{
    [DebuggerDisplay("{Key}")]
    public class MyData : IBinaryHeapItem<int>
    {
        public MyData(int key)
        {
            HeapKey = key;
        }

        public int HeapIndex { get; set; }
        public int HeapKey { get; set; }
    }
}