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

            var a = graph.AddVertex("A");
            var b = graph.AddVertex("B");
            var c = graph.AddVertex("C");
            var d = graph.AddVertex("D");

            var ab = graph.AddEdge("A", "B", 1);
            var bc = graph.AddEdge("B", "C", 1);
            var cd = graph.AddEdge("C", "D", 1);
            var ad = graph.AddEdge("A", "D", 10);

            var vertices = new List<IVertex<string>>();
            var edges = new List<IEdge<string, EdgeDataWeight>>();

            var alg = graph.UndirectedDijkstra();
            alg.EnterNode += vertices.Add;
            alg.EnterEdge += edges.Add;
            alg.Execute(a.Data, d.Data);

            CollectionAssert.AreEqual(new[] { ab, bc, cd }, edges);
            CollectionAssert.AreEqual(new[] { a, b, c, d }, vertices);
        }
    }
}