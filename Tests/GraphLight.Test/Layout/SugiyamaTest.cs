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
            var graph = new GenericGraph<object, object>();
            var a = graph.AddVertex("A");
            var b = graph.AddVertex("B");
            var c = graph.AddVertex("C");

            var ab = graph.AddEdge(a.Data, b.Data, new object());
            var bc = graph.AddEdge(b.Data, c.Data, new object());
            var ca = graph.AddEdge(c.Data, a.Data, new object());
            var ba = graph.AddEdge(b.Data, a.Data, new object());
            var alg = new SugiyamaLayout<object, object>(graph);
            alg.Layout();
        }
    }
}
