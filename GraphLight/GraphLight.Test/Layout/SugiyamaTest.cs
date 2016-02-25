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
            var graph = new Graph<VertexAttrs, EdgeAttrs>();
            var a = graph.AddVertex(new VertexAttrs("A"));
            var b = graph.AddVertex(new VertexAttrs("B"));
            var c = graph.AddVertex(new VertexAttrs("C"));

            var ab = graph.AddEdge(a.Data, b.Data, new EdgeAttrs());
            var bc = graph.AddEdge(b.Data, c.Data, new EdgeAttrs());
            var ca = graph.AddEdge(c.Data, a.Data, new EdgeAttrs());
            var ba = graph.AddEdge(b.Data, a.Data, new EdgeAttrs());
            var alg = new SugiyamaLayout<VertexAttrs, EdgeAttrs>(graph);
            alg.Layout();
        }
    }
}
