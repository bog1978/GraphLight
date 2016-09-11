using GraphLight.Drawing;
using GraphLight.Graph;
using GraphLight.Layout;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Test.Layout
{
    [TestClass]
    public class SugiyamaTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var graph = new DrawingGraph();
            var a = graph.AddVertex("A");
            var b = graph.AddVertex("B");
            var c = graph.AddVertex("C");

            var ab = graph.AddEdge(a.Data, b.Data);
            var bc = graph.AddEdge(b.Data, c.Data);
            var ca = graph.AddEdge(c.Data, a.Data);
            var ba = graph.AddEdge(b.Data, a.Data);
            var alg = new SugiyamaLayout(graph);
            alg.Layout();
        }
    }
}
