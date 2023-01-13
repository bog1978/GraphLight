using System.Collections.Generic;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Test.Algorithm
{
    [TestClass]
    public class KruskalTest
    {
        [TestMethod]
        public void FindSpanningTree()
        {
            var graph = new GenericGraph<object, string, EdgeDataWeight>("");

            var a = graph.AddVertex("A");
            var b = graph.AddVertex("B");
            var c = graph.AddVertex("C");
            var d = graph.AddVertex("D");
            var e = graph.AddVertex("E");
            var f = graph.AddVertex("F");
            var g = graph.AddVertex("G");
            var h = graph.AddVertex("H");
            var i = graph.AddVertex("I");

            var ab = graph.AddEdge(a, b, 4);
            var bc = graph.AddEdge(b, c, 8);
            var ah = graph.AddEdge(a, h, 8);
            var bh = graph.AddEdge(b, h, 11);
            var hi = graph.AddEdge(h, i, 7);
            var hg = graph.AddEdge(h, g, 1);
            var ig = graph.AddEdge(i, g, 6);
            var ic = graph.AddEdge(i, c, 2);
            var gf = graph.AddEdge(g, f, 2);
            var cf = graph.AddEdge(c, f, 4);
            var cd = graph.AddEdge(c, d, 7);
            var df = graph.AddEdge(d, f, 14);
            var de = graph.AddEdge(d, e, 9);
            var fe = graph.AddEdge(f, e, 10);

            var edges = new List<IEdge<string, EdgeDataWeight>>();

            var alg = graph.KruskalSpanningTree(x => x.Data.Weight);
            alg.EnterEdge += edges.Add;
            alg.Execute(graph.Vertices.First());

            var sum = edges.Sum(x => x.Data.Weight);
            Assert.AreEqual(sum, 37.0, "Found spanning tree is not mimimal.");
        }
    }
}