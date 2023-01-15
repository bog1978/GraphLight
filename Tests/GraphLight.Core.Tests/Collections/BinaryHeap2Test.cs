using System;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Collections
{
    [TestClass]
    public class BinaryHeap2Test
    {
        private readonly int[] _testData = { 4, 1, 3, 2, 16, 9, 10, 14, 8, 7 };

        #region Max binary heap

        [TestMethod]
        public void MaxCreateHeapTest()
        {
            var heap = new BinaryHeap2<int, int>(_testData, x => x, HeapType.Max);
            var actual = heap.ToArray();
            var expected = new[] { 16, 14, 10, 8, 7, 9, 3, 2, 4, 1 };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MaxGetRootPositiveTest()
        {
            var heap = new BinaryHeap2<int, int>(_testData, x => x, HeapType.Max);
            var root = heap.Root;
            Assert.AreEqual(16, root);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void MaxGetRootNegativeTest()
        {
            var heap = new BinaryHeap2<int, int>(new int[] { }, x => x, HeapType.Max);
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
            var heap = new BinaryHeap2<int, int>(_testData, x => x, HeapType.Max) { { 20, 20 } };
            var actual = heap.ToArray();
            var expected = new[] { 20, 16, 10, 8, 14, 9, 3, 2, 4, 1, 7 };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MaxRemoveTest()
        {
            var heap = new BinaryHeap2<int, int>(_testData, x =>x, HeapType.Max);

            var isOk = heap.Remove(8);
            Assert.IsTrue(isOk);

            isOk = heap.Remove(8);
            Assert.IsFalse(isOk);

            var actual = heap.ToArray();
            var expected = new[] { 16, 14, 10, 4, 7, 9, 3, 2, 1 };
            CollectionAssert.AreEqual(expected, actual);
        }


        #endregion

        #region Min binary heap

        [TestMethod]
        public void MinCreateHeapTest()
        {
            var heap = new BinaryHeap2<int, int>(_testData, x => x, HeapType.Min);
            var actual = heap.ToArray();
            var expected = new[] { 1, 2, 3, 4, 7, 9, 10, 14, 8, 16 };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MinGetRootPositiveTest()
        {
            var heap = new BinaryHeap2<int, int>(_testData, x => x, HeapType.Min);
            var root = heap.Root;
            Assert.AreEqual(1, root);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void MinGetRootNegativeTest()
        {
            var heap = new BinaryHeap2<int, int>(new int[] { }, x => x, HeapType.Min);
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
            var heap = new BinaryHeap2<int, int>(_testData, x => x, HeapType.Min) { { 0, 0 } };
            var actual = heap.ToArray();
            var expected = new[] { 0, 1, 3, 4, 2, 9, 10, 14, 8, 16, 7 };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MinRemoveTest()
        {
            var heap = new BinaryHeap2<int, int>(_testData, x => x, HeapType.Min);

            var isOk = heap.Remove(2);
            Assert.IsTrue(isOk);

            isOk = heap.Remove(2);
            Assert.IsFalse(isOk);

            var actual = heap.ToArray();
            var expected = new[] { 1, 4, 3, 8, 7, 9, 10, 14, 16 };
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion
    }
}