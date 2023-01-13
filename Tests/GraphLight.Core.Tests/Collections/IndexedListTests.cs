using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Collections
{
    [TestClass]
    public class IndexedListTests
    {
        [TestMethod]
        public void WithCapacityTest()
        {
            var a = new IndexedItem("aaa");
            var b = new IndexedItem("bbb");
            var list = new IndexedList<IndexedItem>(32) { a, b };
            Assert.AreEqual(2, list.Count);
            Assert.IsFalse(list.IsReadOnly);
            Assert.IsFalse(list.IsSynchronized);
            Assert.IsNotNull(list.SyncRoot);
        }

        [TestMethod]
        public void PropsTest()
        {
            var a = new IndexedItem("aaa");
            var b = new IndexedItem("bbb");
            var list = new IndexedList<IndexedItem> { a, b };
            Assert.AreEqual(2, list.Count);
            Assert.IsFalse(list.IsReadOnly);
            Assert.IsFalse(list.IsSynchronized);
            Assert.IsNotNull(list.SyncRoot);
        }

        [TestMethod]
        public void AddSuccessTest()
        {
            var a = new IndexedItem("aaa");
            var b = new IndexedItem("bbb");
            var list = new IndexedList<IndexedItem> { a, b };
            Assert.AreEqual(0, a.Index);
            Assert.AreEqual(1, b.Index);
            CollectionAssert.AreEqual(list, new[] { a, b });
        }

        [TestMethod]
        public void ContainsTest()
        {
            var a = new IndexedItem("aaa");
            var b = new IndexedItem("bbb");
            var x = new IndexedItem("xxx");
            var list = new IndexedList<IndexedItem> { a, b };
            Assert.IsTrue(list.Contains(a));
            Assert.IsTrue(list.Contains(b));
            Assert.IsFalse(list.Contains(x));
        }


        [TestMethod]
        public void CopyToTest()
        {
            var a = new IndexedItem("aaa");
            var b = new IndexedItem("bbb");
            var list = new IndexedList<IndexedItem> { a, b };

            var arrT = new IndexedItem[list.Count];
            list.CopyTo(arrT, 0);
            CollectionAssert.AreEqual(arrT, new[] { a, b });

            Array arr = new IndexedItem[list.Count];
            list.CopyTo(arr, 0);
            CollectionAssert.AreEqual(arr, new[] { a, b });
        }

        [TestMethod]
        public void ClearTest()
        {
            var a = new IndexedItem("aaa");
            var b = new IndexedItem("bbb");
            var list = new IndexedList<IndexedItem> { a, b };
            Assert.AreEqual(2, list.Count);
            list.Clear();
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddfailedTest()
        {
            var a = new IndexedItem("aaa");
            var b = new IndexedItem("bbb");
            var list1 = new IndexedList<IndexedItem> { a, b };
            var list2 = new IndexedList<IndexedItem> { a };
        }

        [TestMethod]
        public void RemoveTest()
        {
            var a = new IndexedItem("aaa");
            var b = new IndexedItem("bbb");
            var c = new IndexedItem("ccc");
            var d = new IndexedItem("ddd");
            var e = new IndexedItem("eee");
            var x = new IndexedItem("xxx");

            int[] IndexListFunc() => new[] { a.Index, b.Index, c.Index, d.Index, e.Index, x.Index };

            var list1 = new IndexedList<IndexedItem> { a, b, c, d, e, };

            CollectionAssert.AreEqual(list1, new[] { a, b, c, d, e });
            CollectionAssert.AreEqual(IndexListFunc(), new[] { 0, 1, 2, 3, 4, -1 });

            // Удаляем "c"
            Assert.IsTrue(list1.Remove(c));
            CollectionAssert.AreEqual(list1, new[] { a, b, e, d });
            CollectionAssert.AreEqual(IndexListFunc(), new[] { 0, 1, -1, 3, 2, -1 });

            Assert.IsFalse(list1.Remove(x));
            var list2 = new IndexedList<IndexedItem> { x };
            Assert.IsFalse(list1.Remove(x));
            Assert.IsFalse(list2.Remove(a));
            Assert.IsTrue(list2.Remove(x));
        }

        private class IndexedItem : IIndexedItem
        {
            public IndexedItem(string name)
            {
                Name = name;
                Index = -1;
            }

            public string Name { get; }

            public int Index { get; set; }
        }
    }
}
