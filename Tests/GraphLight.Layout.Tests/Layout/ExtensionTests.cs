using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Layout
{
    [TestClass]
    public class ExtensionTests
    {
        //[TestMethod]
        //public void TestTightTreeAll()
        //{
        //    foreach (var lazy in TestData.GraphStreams)
        //    {
        //        using (var stream = lazy.Value)
        //        {
        //            var graph = DrawingGraph.ReadFromFile(stream);
        //            graph.Acyclic();
        //            var root = graph.CreateRootVertex();
        //            graph.InitRank();

        //            foreach (var vertex in graph.Verteces)
        //            {
        //                var inOptimal = vertex.InEdges.Any(x => x.Lenght() == x.MinLength);
        //                var outOptimal = vertex.OutEdges.Any(x => x.Lenght() == x.MinLength);
        //                if (vertex.InEdges.Any() && vertex.OutEdges.Any())
        //                    Assert.IsTrue(inOptimal || outOptimal, "Wrong ranking. Vertex: {0}, Rank={1}", vertex.Data.Id, vertex.Data.Rank);
        //                else if (vertex.InEdges.Any())
        //                    Assert.IsTrue(inOptimal, "Wrong ranking. Vertex: {0}, Rank={1}", vertex.Data.Id, vertex.Data.Rank);
        //                else if (vertex.OutEdges.Any())
        //                    Assert.IsTrue(outOptimal, "Wrong ranking. Vertex: {0}, Rank={1}", vertex.Data.Id, vertex.Data.Rank);
        //            }

        //            var tightEdges = graph.TightTree(graph.Verteces.First());
        //            foreach (var e1 in graph.Edges)
        //                e1.IsNonTree = true;
        //            foreach (var e1 in tightEdges)
        //                e1.IsNonTree = false;

        //            foreach (var edge in tightEdges)
        //            {
        //                var lenght = edge.Lenght();
        //                if (lenght != 1)
        //                    DgmlFormatProvider<VertexAttrs, EdgeAttrs>.DumpGraph(graph,
        //                        "TestTightTreeAll.dgml", v => v.Data.Label);
        //                Assert.IsTrue(lenght == 1, "Edge is not tight");
        //            }
        //        }
        //    }
        //}
    }
}