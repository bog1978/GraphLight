using System.Collections.Generic;
using System.Linq;
using GraphLight.Drawing;
using GraphLight.Graph;
using GraphLight.Layout;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using GraphLight.Algorithm;

namespace GraphLight.Test.Layout
{
    [TestClass]
    public class NetworkSimplexTests
    {
        /*[TestMethod]
        public void TestNetworkSimplex()
        {
            var a = new VertexAttrs("a");
            var b = new VertexAttrs("b");
            var c = new VertexAttrs("c");
            var d = new VertexAttrs("d");
            var e = new VertexAttrs("e");
            var f = new VertexAttrs("f");
            var g = new VertexAttrs("g");
            var h = new VertexAttrs("h");

            a.Rank = 0;
            b.Rank = 1;
            e.Rank = f.Rank = c.Rank = 2;
            g.Rank = d.Rank = 3;
            h.Rank = 4;

            var graph = new DrawingGraph();

            var ab = graph.AddEdge(a, b, new EdgeAttrs());
            var ae = graph.AddEdge(a, e, new EdgeAttrs());
            var af = graph.AddEdge(a, f, new EdgeAttrs());
            var bc = graph.AddEdge(b, c, new EdgeAttrs());
            var eg = graph.AddEdge(e, g, new EdgeAttrs());
            var fg = graph.AddEdge(f, g, new EdgeAttrs());
            var cd = graph.AddEdge(c, d, new EdgeAttrs());
            var gh = graph.AddEdge(g, h, new EdgeAttrs());
            var dh = graph.AddEdge(d, h, new EdgeAttrs());

            foreach (var edge in graph.Edges)
                edge.Weight = 1;
            gh.Weight = 10;

            // Instead of given ranked acyclic graph Async and InitRank should be called.
            graph.Acyclic();
            var alg = new RankNetworkSimplex<VertexAttrs, EdgeAttrs>(graph);
            alg.Execute();

            //Assert.IsTrue(graph.Edges.All(x => x.Data.CutValue >= 0), "Must be: CutValue >= 0");
            foreach (var vertex in graph.Verteces)
                Debug.WriteLine("{0}: Rank={1}", vertex.Id, vertex.Data.Rank);
        }*/

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

        private static void checkRanks(IGraph<IVertexData, IEdgeData> graph, IDictionary<IVertex<IVertexData, IEdgeData>, int> expectedRanks)
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