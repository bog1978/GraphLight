using System.Collections.Generic;
using System.Linq;
using GraphLight.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Algorithm
{
    [TestClass]
    public class SpanningTree
    {
        [TestMethod]
        public void KruskalSampleMinSpanningTree()
        {
            var graph = TestData.SampleMinSpanningTree();
            var alg = graph.KruskalSpanningTree(x => x.Data.Weight);
            var expectedEdges = new[] { "AB", "BC", "HG", "IC", "GF", "CF", "CD", "DE" };
            var expectedSum = 37.0;
            SpanningTreeTest(graph, alg, expectedSum, expectedEdges);
        }

        [TestMethod]
        public void KruskalWikipediaMinSpanningTree()
        {
            var graph = TestData.WikipediaMinSpanningTree();
            var alg = graph.KruskalSpanningTree(x => x.Data.Weight);
            var expectedEdges = new[] { "ac", "be", "ce", "df", "ef", "fg", "gh", "hi", "ij" };
            var expectedSum = 38.0;
            SpanningTreeTest(graph, alg, expectedSum, expectedEdges);
        }

        [TestMethod]
        public void PrimSampleMinSpanningTree()
        {
            var graph = TestData.SampleMinSpanningTree();
            var alg = graph.PrimSpanningTree(x => x.Data.Weight);
            var expectedEdges = new[] { "AB", "BC", "HG", "IC", "GF", "CF", "CD", "DE" };
            var expectedSum = 37.0;
            SpanningTreeTest(graph, alg, expectedSum, expectedEdges);
        }

        [TestMethod]
        public void PrimWikipediaMinSpanningTree()
        {
            var graph = TestData.WikipediaMinSpanningTree();
            var alg = graph.PrimSpanningTree(x => x.Data.Weight);
            var expectedEdges = new[] { "ac", "be", "ce", "df", "ef", "fg", "gh", "hi", "ij" };
            var expectedSum = 38.0;
            SpanningTreeTest(graph, alg, expectedSum, expectedEdges);
        }

        private void SpanningTreeTest(IGraph<object, string, EdgeDataWeight> graph, ISpanningTree<string, EdgeDataWeight> alg, double expectedSum, string[] expectedEdges)
        {
            var edges = new List<IEdge<string, EdgeDataWeight>>();
            alg.EnterEdge = edges.Add;
            alg.Execute(graph.Vertices.First());

            var actualEdges = edges.Select(x => $"{x.Src}{x.Dst}").ToArray();
            CollectionAssert.AreEquivalent(expectedEdges, actualEdges, "Found spanning tree is not mimimal.");

            var actualSum = edges.Sum(x => x.Data.Weight);
            Assert.AreEqual(expectedSum, actualSum, "Found spanning tree is not mimimal.");
        }
    }
}