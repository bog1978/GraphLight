using System.Collections.Generic;
using GraphLight.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Algorithm
{
    [TestClass]
    public class DijkstraTest
    {
        [TestMethod]
        public void FindShortestPathTest()
        {
            var graph = Graph.CreateInstance<object, string, EdgeDataWeight>("");

            var (a, b, c, d) = ("A", "B", "C", "D");
            graph.AddVertex(a);
            graph.AddVertex(b);
            graph.AddVertex(c);
            graph.AddVertex(d);

            var ab = graph.AddEdge(a, b, 1);
            var bc = graph.AddEdge(b, c, 1);
            var cd = graph.AddEdge(c, d, 1);
            var ad = graph.AddEdge(a, d, 10);

            var vertices = new List<string>();
            var edges = new List<IEdge<string, EdgeDataWeight>>();

            var alg = graph.UndirectedDijkstra();
            alg.EnterNode += vertices.Add;
            alg.EnterEdge += edges.Add;
            alg.Execute(a, d);

            CollectionAssert.AreEqual(new[] { ab, bc, cd }, edges);
            CollectionAssert.AreEqual(new[] { a, b, c, d }, vertices);
        }
    }
}