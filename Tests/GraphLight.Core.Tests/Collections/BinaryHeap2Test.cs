using System;
using System.Linq;
using GraphLight.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Test.Collections
{
    [TestClass]
    public class BinaryHeap2Test
    {
        private readonly int _0 = 0;
        private readonly int _4 = 4;
        private readonly int _1 = 1;
        private readonly int _3 = 3;
        private readonly int _2 = 2;
        private readonly int _16 = 16;
        private readonly int _9 = 9;
        private readonly int _10 = 10;
        private readonly int _14 = 14;
        private readonly int _8 = 8;
        private readonly int _7 = 7;
        private readonly int _20 = 20;
        private readonly int[] _testData;

        public BinaryHeap2Test()
        {
            _testData = new[] { _4, _1, _3, _2, _16, _9, _10, _14, _8, _7 };
        }

        #region Max binary heap

        [TestMethod]
        public void MaxCreateHeapTest()
        {
            var heap = new BinaryHeap2<int, int>(_testData, x => x, HeapType.Max);
            var actual = heap.ToArray();
            var expected = new[] { _16, _14, _10, _8, _7, _9, _3, _2, _4, _1 };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MaxGetRootPositiveTest()
        {
            var heap = new BinaryHeap2<int, int>(_testData, x => x, HeapType.Max);
            var root = heap.Root;
            Assert.AreEqual(_16, root);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void MaxGetRootNegativeTest()
        {
            var heap = new BinaryHeap2<int, int>(HeapType.Max);
            var root = heap.Root;
        }

        [TestMethod]
        public void MaxExtractTest()
        {
            var heap = new BinaryHeap2<int, int>(_testData, x => x, HeapType.Max);
            foreach (var expected in _testData.OrderByDescending(x => x))
            {
                var actual = heap.RemoveRoot();
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void MaxAddTest()
        {
            var heap = new BinaryHeap2<int, int>(_testData, x => x, HeapType.Max);
            heap.Add(_20, _20);
            var actual = heap.ToArray();
            var expected = new[] { _20, _16, _10, _8, _14, _9, _3, _2, _4, _1, _7 };
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion

        #region Min binary heap

        [TestMethod]
        public void MinCreateHeapTest()
        {
            var heap = new BinaryHeap2<int, int>(_testData, x => x, HeapType.Min);
            var actual = heap.ToArray();
            var expected = new[] { _1, _2, _3, _4, _7, _9, _10, _14, _8, _16 };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MinGetRootPositiveTest()
        {
            var heap = new BinaryHeap2<int, int>(_testData, x => x, HeapType.Min);
            var root = heap.Root;
            Assert.AreEqual(_1, root);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void MinGetRootNegativeTest()
        {
            var heap = new BinaryHeap2<int, int>(HeapType.Min);
            var root = heap.Root;
        }

        [TestMethod]
        public void MinExtractTest()
        {
            var heap = new BinaryHeap2<int, int>(_testData, x => x, HeapType.Min);
            foreach (var expected in _testData.OrderBy(x => x))
            {
                var actual = heap.RemoveRoot();
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void MinAddTest()
        {
            var heap = new BinaryHeap2<int, int>(_testData, x => x, HeapType.Min);
            heap.Add(_0, _0);
            var actual = heap.ToArray();
            var expected = new[] { _0, _1, _3, _4, _2, _9, _10, _14, _8, _16, _7 };
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion
    }
}