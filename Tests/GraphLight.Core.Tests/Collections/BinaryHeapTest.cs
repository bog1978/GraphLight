using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Test.Collections
{
    [TestClass]
    public class BinaryHeapTest
    {
        private readonly MyData _0 = new MyData(0);
        private readonly MyData _4 = new MyData(4);
        private readonly MyData _1 = new MyData(1);
        private readonly MyData _3 = new MyData(3);
        private readonly MyData _2 = new MyData(2);
        private readonly MyData _16 = new MyData(16);
        private readonly MyData _9 = new MyData(9);
        private readonly MyData _10 = new MyData(10);
        private readonly MyData _14 = new MyData(14);
        private readonly MyData _8 = new MyData(8);
        private readonly MyData _7 = new MyData(7);
        private readonly MyData _20 = new MyData(20);
        private readonly MyData[] _testData;

        public BinaryHeapTest()
        {
            _testData = new[] { _4, _1, _3, _2, _16, _9, _10, _14, _8, _7 };
        }

        #region Max binary heap

        [TestMethod]
        public void MaxCreateHeapTest()
        {
            var heap = new BinaryHeap<int, MyData>(_testData, HeapType.Max);
            var actual = heap.ToArray();
            var expected = new[] { _16, _14, _10, _8, _7, _9, _3, _2, _4, _1 };
            CollectionAssert.AreEqual(expected, actual);
            checkHeapIndex(heap);
        }

        [TestMethod]
        public void MaxGetRootPositiveTest()
        {
            var heap = new BinaryHeap<int, MyData>(_testData, HeapType.Max);
            var root = heap.Root;
            Assert.AreEqual(_16, root);
            checkHeapIndex(heap);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void MaxGetRootNegativeTest()
        {
            var heap = new BinaryHeap<int, MyData>(new MyData[] { }, HeapType.Max);
            var root = heap.Root;
            checkHeapIndex(heap);
        }

        [TestMethod]
        public void MaxExtractTest()
        {
            var heap = new BinaryHeap<int, MyData>(_testData, HeapType.Max);
            foreach (var expected in _testData.OrderByDescending(x => x.HeapKey))
            {
                var actual = heap.RemoveRoot();
                Assert.AreEqual(expected, actual);
            }
            checkHeapIndex(heap);
        }

        [TestMethod]
        public void MaxAddTest()
        {
            var heap = new BinaryHeap<int, MyData>(_testData, HeapType.Max) { _20 };
            var actual = heap.ToArray();
            var expected = new[] { _20, _16, _10, _8, _14, _9, _3, _2, _4, _1, _7 };
            CollectionAssert.AreEqual(expected, actual);
            checkHeapIndex(heap);
        }

        [TestMethod]
        public void MaxRemoveTest()
        {
            var heap = new BinaryHeap<int, MyData>(_testData, HeapType.Max);

            var isOk = heap.Remove(_8);
            Assert.IsTrue(isOk);

            isOk = heap.Remove(_8);
            Assert.IsFalse(isOk);

            var actual = heap.ToArray();
            var expected = new[] { _16, _14, _10, _4, _7, _9, _3, _2, _1 };
            CollectionAssert.AreEqual(expected, actual);
            checkHeapIndex(heap);
        }

        #endregion

        #region Min binary heap

        [TestMethod]
        public void MinCreateHeapTest()
        {
            var heap = new BinaryHeap<int, MyData>(_testData, HeapType.Min);
            var actual = heap.ToArray();
            var expected = new[] { _1, _2, _3, _4, _7, _9, _10, _14, _8, _16 };
            CollectionAssert.AreEqual(expected, actual);
            checkHeapIndex(heap);
        }

        [TestMethod]
        public void MinGetRootPositiveTest()
        {
            var heap = new BinaryHeap<int, MyData>(_testData, HeapType.Min);
            var root = heap.Root;
            Assert.AreEqual(_1, root);
            checkHeapIndex(heap);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void MinGetRootNegativeTest()
        {
            var heap = new BinaryHeap<int, MyData>(new MyData[] { }, HeapType.Min);
            var root = heap.Root;
            checkHeapIndex(heap);
        }

        [TestMethod]
        public void MinExtractTest()
        {
            var heap = new BinaryHeap<int, MyData>(_testData, HeapType.Min);
            foreach (var expected in _testData.OrderBy(x => x.HeapKey))
            {
                var actual = heap.RemoveRoot();
                Assert.AreEqual(expected, actual);
            }
            checkHeapIndex(heap);
        }

        [TestMethod]
        public void MinAddTest()
        {
            var heap = new BinaryHeap<int, MyData>(_testData, HeapType.Min) { _0 };
            var actual = heap.ToArray();
            var expected = new[] { _0, _1, _3, _4, _2, _9, _10, _14, _8, _16, _7 };
            CollectionAssert.AreEqual(expected, actual);
            checkHeapIndex(heap);
        }

        [TestMethod]
        public void MinRemoveTest()
        {
            var heap = new BinaryHeap<int, MyData>(_testData, HeapType.Min);

            var isOk = heap.Remove(_2);
            Assert.IsTrue(isOk);

            isOk = heap.Remove(_2);
            Assert.IsFalse(isOk);

            var actual = heap.ToArray();
            var expected = new[] { _1, _4, _3, _8, _7, _9, _10, _14, _16 };
            CollectionAssert.AreEqual(expected, actual);
            checkHeapIndex(heap);
        }

        #endregion

        private static void checkHeapIndex(IEnumerable<MyData> heap)
        {
            var i = 0;
            foreach (var data in heap)
            {
                Assert.AreEqual(i, data.HeapIndex);
                i++;
            }
        }
    }
}