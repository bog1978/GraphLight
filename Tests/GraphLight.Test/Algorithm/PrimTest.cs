using System.Collections.Generic;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Test.Algorithm
{
    [TestClass]
    public class PrimTest
    {
        [TestMethod]
        public void FindSpanningTree()
        {
            var graph = GraphFactory.CreateInstance<object, string, EdgeDataWeight>("");

            var ab = graph.AddEdge("A", "B", 4);
            var bc = graph.AddEdge("B", "C", 8);
            var ah = graph.AddEdge("A", "H", 8);
            var bh = graph.AddEdge("B", "H", 11);
            var hi = graph.AddEdge("H", "I", 7);
            var hg = graph.AddEdge("H", "G", 1);
            var ig = graph.AddEdge("I", "G", 6);
            var ic = graph.AddEdge("I", "C", 2);
            var gf = graph.AddEdge("G", "F", 2);
            var cf = graph.AddEdge("C", "F", 4);
            var cd = graph.AddEdge("C", "D", 7);
            var df = graph.AddEdge("D", "F", 14);
            var de = graph.AddEdge("D", "E", 9);
            var fe = graph.AddEdge("F", "E", 10);

            var edges = new List<IEdge<string, EdgeDataWeight>>();

            var alg = graph.PrimSpanningTree(x => x.Data.Weight);
            alg.EnterEdge += edges.Add;
            alg.Execute(graph.Vertices.First());

            var sum = edges.Sum(x => x.Data.Weight);
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