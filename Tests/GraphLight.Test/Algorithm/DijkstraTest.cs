using System.Collections.Generic;
using GraphLight.Algorithm;
using GraphLight.Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Test.Algorithm
{
    [TestClass]
    public class DijkstraTest
    {
        [TestMethod]
        public void FindShortestPathTest()
        {
            var graph = new GenericGraph<object, string, EdgeDataWeight>("");

            var a = graph.AddVertex("A");
            var b = graph.AddVertex("B");
            var c = graph.AddVertex("C");
            var d = graph.AddVertex("D");

            var ab = graph.AddEdge(a, b, 1);
            var bc = graph.AddEdge(b, c, 1);
            var cd = graph.AddEdge(c, d, 1);
            var ad = graph.AddEdge(a, d, 10);

            var vertices = new List<IVertex<string, EdgeDataWeight>>();
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