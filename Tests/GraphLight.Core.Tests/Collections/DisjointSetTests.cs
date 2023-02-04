using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Collections
{
    [TestClass]
    public class DisjointSetTests
    {
        [TestMethod]
        public void Test1()
        {
            var items = new[] { "A", "B", "C" };
            var ds = new DisjointSet<string>(items);

            var i1 = items.Select(ds.Find).ToArray();
            CollectionAssert.AreEqual(new[] { 0, 1, 2 }, i1);

            ds.Unite("A", "B");
            var i2 = items.Select(ds.Find).ToArray();
            CollectionAssert.AreEqual(new[] { 0, 0, 2 }, i2);
        }
    }
}
