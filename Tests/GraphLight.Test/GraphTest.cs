using System.Linq;
using GraphLight.Drawing;
using GraphLight.Graph;
using GraphLight.Layout;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Test
{
    [TestClass]
    public class GraphTest
    {
        [TestMethod]
        public void TestAddInsertRemove()
        {
            var graph = new GraphModel();
            var g = graph;
            var ab = graph.AddEdge("A", "B");
            var a = ab.Src;
            var b = ab.Dst;

            var cp = g.InsertControlPoint((IEdge)ab);
            Assert.IsTrue(ReferenceEquals(ab, graph.Edges.First()));
            Assert.IsTrue(ReferenceEquals(ab, a.OutEdges.First()));
            Assert.IsTrue(ReferenceEquals(ab, cp.InEdges.First()));
            Assert.IsTrue(ReferenceEquals(ab.Src, a));
            Assert.IsTrue(ReferenceEquals(ab.Dst, cp));

            g.RemoveControlPoint(cp);
            Assert.IsTrue(ReferenceEquals(ab, graph.Edges.First()));
            Assert.IsTrue(ReferenceEquals(ab, a.OutEdges.First()));
            Assert.IsTrue(ReferenceEquals(ab, b.InEdges.First()));
            Assert.IsTrue(ReferenceEquals(ab.Src, a));
            Assert.IsTrue(ReferenceEquals(ab.Dst, b));
        }

        [TestMethod]
        public void ShortesdWayTest()
        {
            var arr = new[] { 22, 5, 3, 9, 7, 8, 93, 4, 21, 5, 67 };
            int result = arr.Aggregate((a, b) => a < b ? a : b);
            Assert.AreEqual(result, 3);
        }

        [TestMethod]
        public void Issue7610Test_1()
        {
            var graph = new GraphModel();
            graph.AddEdge("A", "B");
            graph.AddEdge("B", "C");
            graph.AddEdge("A", "C");
            var engine = new GraphVizLayout<object, object>
                {
                    NodeMeasure = new NodeMeasure<object, object>(),
                    Graph = graph
                };
            engine.Layout();
        }

        [TestMethod]
        public void Issue7610Test_2()
        {
            var graph = new GraphModel();
            graph.AddEdge("A", "B");
            graph.AddEdge("B", "C");
            graph.AddEdge("C", "D");
            graph.AddEdge("D", "A");
            var engine = new GraphVizLayout<object, object>
            {
                    NodeMeasure = new NodeMeasure<object, object>(),
                    Graph = graph
                };
            engine.Layout();
        }
    }
}