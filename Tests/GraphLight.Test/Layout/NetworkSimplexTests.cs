using System.Collections.Generic;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Layout
{
    [TestClass]
    public class NetworkSimplexTests
    {
        [TestMethod]
        public void TestNetworkSimplex()
        {
            var graph = Graph.CreateInstance<GraphData, VertexData, EdgeData>(new GraphData());

            graph.AddEdge("a", "b", new EdgeData());
            graph.AddEdge("a", "e", new EdgeData { Weight = 2 });
            graph.AddEdge("a", "f", new EdgeData());
            graph.AddEdge("b", "c", new EdgeData());
            graph.AddEdge("e", "g", new EdgeData());
            graph.AddEdge("f", "g", new EdgeData { Weight = 2 });
            graph.AddEdge("c", "d", new EdgeData());
            graph.AddEdge("g", "h", new EdgeData { Weight = 10 });
            graph.AddEdge("d", "h", new EdgeData());

            graph.Acyclic();
            var alg = graph.RankNetworkSimplex();
            alg.Execute();

            var ranksExpected = new[] { 0, 1, 2, 3, 1, 2, 3, 4 };
            var ranksActual = graph.Vertices
               .OrderBy(x =>x.Data.Id)
               .Select(x => x.Data)
               .OfType<IVertexDataLayered>()
               .Select(x => x.Rank)
               .ToArray();
            CollectionAssert.AreEqual(ranksExpected, ranksActual);
        }

        //[TestMethod]
        //public void TestAllTestData()
        //{
        //    foreach (var lazy in TestData.GraphStreams)
        //    {
        //        IGraph graph;
        //        using (var stream = lazy.Value)
        //            graph = GraphHelper.ReadFromFile(stream);
        //        graph.Acyclic();
        //        var expectedRanks = graph.Vertices.ToDictionary(x => x, x => x.Data.Rank);
        //        var alg = graph.RankNetworkSimplex();

        //        alg.Execute();
        //        checkRanks(graph, expectedRanks);
        //    }
        //}

        //[TestMethod]
        //public void LayoutAllTestData()
        //{
        //    foreach (var lazy in TestData.GraphStreams)
        //    {
        //        IGraph graph;
        //        using (var stream = lazy.Value)
        //            graph = GraphHelper.ReadFromFile(stream);

        //        var expectedRanks = graph.Vertices.ToDictionary(x => x, x => x.Data.Rank);

        //        using (var f1 = File.Create("d:\\temp\\out0.graph"))
        //            graph.WriteToFile(f1);
        //        var engine = new GraphVizLayout<IVertexData, IEdgeData>
        //        {
        //            NodeMeasure = new WpfNodeMeasure<IVertexData, IEdgeData>(),
        //            Graph = graph
        //        };

        //        // First layout works fine
        //        engine.Layout();
        //        using (var f1 = File.Create("d:\\temp\\out1.graph"))
        //            graph.WriteToFile(f1);
        //        checkRanks(graph, expectedRanks);

        //        // Second layout must make the same ranking
        //        engine.Layout();
        //        using (var f1 = File.Create("d:\\temp\\out2.graph"))
        //            graph.WriteToFile(f1);
        //        checkRanks(graph, expectedRanks);
        //    }
        //}

        private static void checkRanks(IGraph<IGraphData, IVertexData, IEdgeData> graph, IDictionary<IVertex<IVertexData, IEdgeData>, int> expectedRanks)
        {
            foreach (var vertex in graph.Vertices)
            {
                var expected = expectedRanks[vertex];
                var actual = vertex.Data.Rank;
                Assert.AreEqual(expected, actual,
                    "Vertex {0}: rank={1} but expected {2}",
                    vertex.Data, actual, expected);
            }
        }
    }
}