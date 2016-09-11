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
            Graph<object, object> graph = new Graph<object, object>();

            var a = graph.AddVertex("A");
            var b = graph.AddVertex("B");
            var c = graph.AddVertex("C");
            var d = graph.AddVertex("D");

            var ab = graph.AddEdge(a, b, null);
            var bc = graph.AddEdge(b, c, null);
            var cd = graph.AddEdge(c, d, null);
            var ad = graph.AddEdge(a, d, null);
            ab.Weight = 1;
            bc.Weight = 1;
            cd.Weight = 1;
            ad.Weight = 10;

            var vertices = new List<IVertex>();
            var edges = new List<IEdge>();

            var alg = new UndirectedDijkstra(graph);
            alg.EnterNode += vertices.Add;
            alg.EnterEdge += edges.Add;
            alg.Find(a, d);

            CollectionAssert.AreEqual(new[] { ab, bc, cd }, edges);
            CollectionAssert.AreEqual(new[] { a, b, c, d }, vertices);
        }
    }
}