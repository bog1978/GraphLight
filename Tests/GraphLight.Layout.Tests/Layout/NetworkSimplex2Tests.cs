using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Layout
{
    [TestClass]
    public class NetworkSimplex2Tests
    {
        [TestMethod]
        public void TestNetworkSimplex()
        {
            var graph = Graph.CreateInstance<GraphData, IVertexData, EdgeDataWeight>(new GraphData());

            var a = new VertexData("a");
            var b = new VertexData("b");
            var c = new VertexData("c");
            var d = new VertexData("d");
            var e = new VertexData("e");
            var f = new VertexData("f");
            var g = new VertexData("g");
            var h = new VertexData("h");

            graph.AddVertexRange(a, b, c, d, e, f, g, h);

            var ab = graph.AddEdge(a, b, 1);
            var ae = graph.AddEdge(a, e, 2);
            var af = graph.AddEdge(a, f, 1);
            var bc = graph.AddEdge(b, c, 1);
            var eg = graph.AddEdge(e, g, 1);
            var fg = graph.AddEdge(f, g, 2);
            var cd = graph.AddEdge(c, d, 1);
            var gh = graph.AddEdge(g, h, 10);
            var dh = graph.AddEdge(d, h, 1);

            graph.Acyclic();
            var alg = (NetworkSimplex2)graph.RankNetworkSimplex2();
            alg.Step = OnStep;
            alg.Execute();

            //var ranksExpected = new[] { 0, 1, 2, 3, 1, 2, 3, 4 };
            //var ranksActual = graph.Vertices
            //   .OrderBy(x => x.Id)
            //   .Select(x => x.Rank)
            //   .ToArray();
            //CollectionAssert.AreEqual(ranksExpected, ranksActual);

            void OnStep(string step, IGraph<NetworkSimplex2.GraphData, NetworkSimplex2.VertexData, NetworkSimplex2.EdgeData> sg)
            {
                switch (step)
                {
                    case "MakeRootedGraph_0":
                        break;
                    case "FeasibleTree_1":
                        {
                            var map = sg.Vertices.ToDictionary(x => x.Original, x => x.Value);
                            var expected = new[] { 1, 2, 3, 4, 2, 2, 3, 5 };
                            var actual = graph.Vertices.Select(x => map[x]).ToArray();
                            CollectionAssert.AreEqual(expected, actual);
                            break;
                        }
                    case "SpanningTree_2":
                        {
                            foreach (var edge in sg.Edges)
                            {
                                if (HasEnds(edge, e, g))
                                    Assert.IsFalse(edge.Data.IsTree);
                                else if (HasEnds(edge, g, h))
                                    Assert.IsFalse(edge.Data.IsTree);
                                else
                                    Assert.IsTrue(edge.Data.IsTree);
                            }
                            var map = sg.Vertices.ToDictionary(x => x.Original, x => x.ParentEdge);
                            Assert.IsNull(map["_ROOT_"]);
                            Assert.IsTrue(HasEnds(map[a], "_ROOT_", a));
                            Assert.IsTrue(HasEnds(map[b], a, b));
                            Assert.IsTrue(HasEnds(map[c], b, c));
                            Assert.IsTrue(HasEnds(map[d], c, d));
                            Assert.IsTrue(HasEnds(map[e], a, e));
                            Assert.IsTrue(HasEnds(map[f], a, f));
                            Assert.IsTrue(HasEnds(map[g], f, g));
                            Assert.IsTrue(HasEnds(map[h], d, h));
                            break;
                        }
                    case "PostOrderTraversal_3":
                        {
                            var map = sg.Vertices.ToDictionary(x => x.Original);
                            Assert.AreEqual(0, map[h].Lim); Assert.AreEqual(0, map[h].Low);
                            Assert.AreEqual(1, map[d].Lim); Assert.AreEqual(0, map[d].Low);
                            Assert.AreEqual(2, map[c].Lim); Assert.AreEqual(0, map[c].Low);
                            Assert.AreEqual(3, map[b].Lim); Assert.AreEqual(0, map[b].Low);
                            Assert.AreEqual(5, map[e].Lim); Assert.AreEqual(4, map[e].Low);
                            Assert.AreEqual(4, map[g].Lim); Assert.AreEqual(4, map[g].Low);
                            Assert.AreEqual(6, map[f].Lim); Assert.AreEqual(6, map[f].Low);
                            Assert.AreEqual(7, map[a].Lim); Assert.AreEqual(0, map[a].Low);
                            Assert.AreEqual(8, map["_ROOT_"].Lim); Assert.AreEqual(0, map["_ROOT_"].Low);
                        }
                        break;
                    default:
                        Assert.Fail("Этот тест еще не написан.");
                        break;
                }
            }

            static bool HasEnds(IEdge<NetworkSimplex2.VertexData, NetworkSimplex2.EdgeData> edge, object a, object b) =>
                ReferenceEquals(edge.Src.Original, a) && ReferenceEquals(edge.Dst.Original, b);
        }
    }
}