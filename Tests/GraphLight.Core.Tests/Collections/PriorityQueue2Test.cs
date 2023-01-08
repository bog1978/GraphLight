using GraphLight.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Test.Collections
{
    [TestClass]
    public class PriorityQueue2Test
    {
        [TestMethod]
        public void Test1()
        {
            var testData = new[]
                {
                    new TestEntity{Id = 1, Name = "quick"},
                    new TestEntity{Id = 2, Name = "brown"},
                    new TestEntity{Id = 3, Name = "fox"},
                    new TestEntity{Id = 4, Name = "jumped"},
                    new TestEntity{Id = 5, Name = "over"},
                    new TestEntity{Id = 6, Name = "lazy"},
                    new TestEntity{Id = 7, Name = "dog"}
                };
            var q = new PriorityQueue2<TestEntity, string>(testData, x  => x.Name, HeapType.Min);
            var item = q.Dequeue();
            Assert.AreEqual(2, item.Id);

            item = q.Dequeue();
            Assert.AreEqual(7, item.Id);

            item = q.Dequeue();
            Assert.AreEqual(3, item.Id);

            item = q.Dequeue();
            Assert.AreEqual(4, item.Id);

            item = q.Dequeue();
            Assert.AreEqual(6, item.Id);

            item = q.Dequeue();
            Assert.AreEqual(5, item.Id);

            item = q.Dequeue();
            Assert.AreEqual(1, item.Id);
        }
    }
}
