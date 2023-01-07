using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Test.Algorithm
{
    [TestClass]
    public class PostorderTest
    {
        //[TestMethod]
        //public void TestAllTestData()
        //{
        //    foreach (var lazy in TestData.GraphStreams)
        //    {
        //        DrawingGraph graph;
        //        using (var stream = lazy.Value)
        //            graph = DrawingGraph.ReadFromFile(stream);
        //        var root = graph.CreateRootVertex();
        //        graph.Acyclic();
        //        graph.TightTree(root);
        //        var alg = new PostorderTraversal<VertexAttrs, EdgeAttrs>(graph);
        //        var map = alg.Execute(root);
        //        var rootAttr = map[root];
        //        Assert.IsNull(rootAttr.Parent);
        //        foreach (var v in graph.Verteces)
        //        {
        //            if (v == root)
        //                continue;
        //            var attr = map[v];
        //            Assert.IsNotNull(attr.Parent);
        //        }
        //    }
        //}
    }
}
