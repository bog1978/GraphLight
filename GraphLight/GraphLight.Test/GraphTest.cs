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
            var graph = new DrawingGraph("test");
            DrawingGraph g = graph;
            IEdge<VertexAttrs, EdgeAttrs> ab = graph.AddEdge(new VertexAttrs("A"), new VertexAttrs("B"), new EdgeAttrs());
            IVertex<VertexAttrs, EdgeAttrs> a = ab.Src;
            IVertex<VertexAttrs, EdgeAttrs> b = ab.Dst;

            IVertex<VertexAttrs, EdgeAttrs> cp = g.InsertControlPoint(ab);
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
            var graph = new DrawingGraph("Label");
            graph.AddEdge(new VertexAttrs("A"), new VertexAttrs("B"), new EdgeAttrs());
            graph.AddEdge(new VertexAttrs("B"), new VertexAttrs("C"), new EdgeAttrs());
            graph.AddEdge(new VertexAttrs("A"), new VertexAttrs("C"), new EdgeAttrs());
            var engine = new GraphVizLayout<VertexAttrs, EdgeAttrs>
                {
                    NodeMeasure = new NodeMeasure(),
                    Graph = graph
                };
            engine.Layout();
        }

        [TestMethod]
        public void Issue7610Test_2()
        {
            var graph = new DrawingGraph("Label");
            graph.AddEdge(new VertexAttrs("A"), new VertexAttrs("B"), new EdgeAttrs());
            graph.AddEdge(new VertexAttrs("B"), new VertexAttrs("C"), new EdgeAttrs());
            graph.AddEdge(new VertexAttrs("C"), new VertexAttrs("D"), new EdgeAttrs());
            graph.AddEdge(new VertexAttrs("D"), new VertexAttrs("A"), new EdgeAttrs());
            var engine = new GraphVizLayout<VertexAttrs, EdgeAttrs>
                {
                    NodeMeasure = new NodeMeasure(),
                    Graph = graph
                };
            engine.Layout();
        }
    }
}