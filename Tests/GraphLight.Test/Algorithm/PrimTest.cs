using System.Collections.Generic;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Test.Algorithm
{
    [TestClass]
    public class PrimTest
    {
        [TestMethod]
        public void FindSpanningTree()
        {
            var graph = new GraphModel<object, object>();

            var a = graph.AddVertex("A");
            var b = graph.AddVertex("B");
            var c = graph.AddVertex("C");
            var d = graph.AddVertex("D");
            var e = graph.AddVertex("E");
            var f = graph.AddVertex("F");
            var g = graph.AddVertex("G");
            var h = graph.AddVertex("H");
            var i = graph.AddVertex("I");

            var ab = graph.AddEdge(a, b);
            var bc = graph.AddEdge(b, c);
            var ah = graph.AddEdge(a, h);
            var bh = graph.AddEdge(b, h);
            var hi = graph.AddEdge(h, i);
            var hg = graph.AddEdge(h, g);
            var ig = graph.AddEdge(i, g);
            var ic = graph.AddEdge(i, c);
            var gf = graph.AddEdge(g, f);
            var cf = graph.AddEdge(c, f);
            var cd = graph.AddEdge(c, d);
            var df = graph.AddEdge(d, f);
            var de = graph.AddEdge(d, e);
            var fe = graph.AddEdge(f, e);

            ab.Weight = 4;
            ah.Weight = 8;
            bh.Weight = 11;
            bc.Weight = 8;
            hi.Weight = 7;
            hg.Weight = 1;
            ig.Weight = 6;
            ic.Weight = 2;
            gf.Weight = 2;
            cf.Weight = 4;
            cd.Weight = 7;
            df.Weight = 14;
            de.Weight = 9;
            fe.Weight = 10;

            var edges = new List<IEdge<object, object>>();

            var alg = graph.PrimSpanningTree(x => x.Weight);
            alg.EnterEdge += edges.Add;
            alg.Execute(graph.Vertices.First());

            var sum = edges.Sum(x => x.Weight);
            Assert.AreEqual(sum, 37.0, "Found spanning tree is not mimimal.");
        }

        /// <summary>
        /// http://ru.wikipedia.org/wiki/Файл:Minimum_spanning_tree.svg
        /// </summary>
        [TestMethod,Ignore]
        public void WikipediaSampleTest()
        {
            // TODO: Implement test.
        }
    }
}