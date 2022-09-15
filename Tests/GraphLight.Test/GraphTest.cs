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
            var graph = new GenericGraph<object, object>();
            var g = graph;
            var ab = graph.AddEdge("A", "B", new object());
            var a = ab.Src;
            var b = ab.Dst;

            var cp = g.InsertControlPoint(ab, new object(), new object());
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
            var graph = new LayoutGraphModel();
            var a = graph.CreateVertexData("A");
            var b = graph.CreateVertexData("B");
            var c = graph.CreateVertexData("C");
            graph.AddEdge(a, b, graph.CreateEdgeData());
            graph.AddEdge(b, c, graph.CreateEdgeData());
            graph.AddEdge(a, c, graph.CreateEdgeData());
            var engine = new GraphVizLayout<IVertexData, IEdgeData>
                {
                    NodeMeasure = new NodeMeasure<IVertexData, IEdgeData>(),
                    Graph = graph
                };
            engine.Layout();
        }

        [TestMethod]
        public void Issue7610Test_2()
        {
            var graph = new LayoutGraphModel();
            var a = graph.CreateVertexData("A");
            var b = graph.CreateVertexData("B");
            var c = graph.CreateVertexData("C");
            var d = graph.CreateVertexData("D");
            graph.AddEdge(a, b, graph.CreateEdgeData());
            graph.AddEdge(b, c, graph.CreateEdgeData());
            graph.AddEdge(c, d, graph.CreateEdgeData());
            graph.AddEdge(d, a, graph.CreateEdgeData());
            var engine = new GraphVizLayout<IVertexData, IEdgeData>
            {
                    NodeMeasure = new NodeMeasure<IVertexData, IEdgeData>(),
                    Graph = graph
                };
            engine.Layout();
        }
    }
}