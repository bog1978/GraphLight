using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Model
{
    [TestClass]
    public class DrawingGraphTests
    {
        private static readonly ICollection _emptyEdges = new List<IEdge<object, object>>();

        [TestMethod]
        public void Test1()
        {
            var g = Graph.CreateInstance<object, object, object>("");
            var a = g.AddVertex("a");
            var b = g.AddVertex("b");
            var c = g.AddVertex("c");
            var aa = g.AddEdge(a.Data, a.Data, new object());
            var ab = g.AddEdge(a.Data, b.Data, new object());
            var ac = g.AddEdge(a.Data, c.Data, new object());
            var bc = g.AddEdge(b.Data, c.Data, new object());

            checkEdges(g, a, new[] { aa, ab, ac }, _emptyEdges, new[] { ab, ac }, new[] { aa });
            checkEdges(g, b, new[] { ab, bc }, new[] { ab }, new[] { bc }, _emptyEdges);
            checkEdges(g, c, new[] { ac, bc }, new[] { ac, bc }, _emptyEdges, _emptyEdges);
            checkGraph(g, new[] { aa, ab, ac, bc }, new[] { a, b, c });

            g.ChangeSource(ab, c);
            checkEdges(g, a, new[] { aa, ac }, _emptyEdges, new[] { ac }, new[] { aa });
            checkEdges(g, b, new[] { ab, bc }, new[] { ab }, new[] { bc }, _emptyEdges);
            checkEdges(g, c, new[] { ab, ac, bc }, new[] { ac, bc }, new[] { ab }, _emptyEdges);
            checkGraph(g, new[] { aa, ab, ac, bc }, new[] { a, b, c });

            g.ChangeSource(ab, null);
            checkEdges(g, a, new[] { aa, ac }, _emptyEdges, new[] { ac }, new[] { aa });
            checkEdges(g, b, new[] { ab, bc }, new[] { ab }, new[] { bc }, _emptyEdges);
            checkEdges(g, c, new[] { ac, bc }, new[] { ac, bc }, _emptyEdges, _emptyEdges);
            checkGraph(g, new[] { aa, ab, ac, bc }, new[] { a, b, c });

            g.ChangeDestination(ab, null);
            checkEdges(g, a, new[] { aa, ac }, _emptyEdges, new[] { ac }, new[] { aa });
            checkEdges(g, b, new[] { bc }, _emptyEdges, new[] { bc }, _emptyEdges);
            checkEdges(g, c, new[] { ac, bc }, new[] { ac, bc }, _emptyEdges, _emptyEdges);
            checkGraph(g, new[] { aa, ab, ac, bc }, new[] { a, b, c });

            g.ChangeDestination(ab, b);
            checkEdges(g, a, new[] { aa, ac }, _emptyEdges, new[] { ac }, new[] { aa });
            checkEdges(g, b, new[] { ab, bc }, new[] { ab }, new[] { bc }, _emptyEdges);
            checkEdges(g, c, new[] { ac, bc }, new[] { ac, bc }, _emptyEdges, _emptyEdges);
            checkGraph(g, new[] { aa, ab, ac, bc }, new[] { a, b, c });

            g.ChangeSource(ab, a);
            checkEdges(g, a, new[] { aa, ab, ac }, _emptyEdges, new[] { ab, ac }, new[] { aa });
            checkEdges(g, b, new[] { ab, bc }, new[] { ab }, new[] { bc }, _emptyEdges);
            checkEdges(g, c, new[] { ac, bc }, new[] { ac, bc }, _emptyEdges, _emptyEdges);
            checkGraph(g, new[] { aa, ab, ac, bc }, new[] { a, b, c });

            g.RemoveVertex(b);
            checkEdges(g, a, new[] { aa, ac }, _emptyEdges, new[] { ac }, new[] { aa });
            checkEdges(g, b, _emptyEdges, _emptyEdges, _emptyEdges, _emptyEdges);
            checkEdges(g, c, new[] { ac }, new[] { ac }, _emptyEdges, _emptyEdges);
            checkGraph(g, new[] { aa, ac }, new[] { a, c });

            g.RemoveEdge(ac);
            checkEdges(g, a, new[] { aa }, _emptyEdges, _emptyEdges, new[] { aa });
            checkEdges(g, b, _emptyEdges, _emptyEdges, _emptyEdges, _emptyEdges);
            checkEdges(g, c, _emptyEdges, _emptyEdges, _emptyEdges, _emptyEdges);
            checkGraph(g, new[] { aa }, new[] { a, c });

            g.ChangeSource(aa, null);
            checkEdges(g, a, new[] { aa }, new[] { aa }, _emptyEdges, _emptyEdges);
            checkGraph(g, new[] { aa }, new[] { a, c });

            g.ChangeSource(aa, a);
            checkEdges(g, a, new[] { aa }, _emptyEdges, _emptyEdges, new[] { aa });
            checkGraph(g, new[] { aa }, new[] { a, c });

            g.ChangeDestination(aa, null);
            checkEdges(g, a, new[] { aa }, _emptyEdges, new[] { aa }, _emptyEdges);
            checkGraph(g, new[] { aa }, new[] { a, c });

            g.ChangeDestination(aa, c);
            checkEdges(g, a, new[] { aa }, _emptyEdges, new[] { aa }, _emptyEdges);
            checkEdges(g, c, new[] { aa }, new[] { aa }, _emptyEdges, _emptyEdges);
            checkGraph(g, new[] { aa }, new[] { a, c });
        }

        private static void checkEdges(
            IGraph<object, object, object> graph,
            IVertex<object, object> vertex,
            ICollection allEdges,
            ICollection inEdges,
            ICollection outEdges,
            ICollection selfEdges)
        {
            CollectionAssert.AreEquivalent(graph.GetEdges(vertex).ToList(), allEdges);
            CollectionAssert.AreEquivalent(graph.GetInEdges(vertex).ToList(), inEdges);
            CollectionAssert.AreEquivalent(graph.GetOutEdges(vertex).ToList(), outEdges);
            CollectionAssert.AreEquivalent(graph.GetLoopEdges(vertex).ToList(), selfEdges);
        }

        private static void checkGraph(IGraph<object, object, object> graph, ICollection edges, ICollection verteces)
        {
            CollectionAssert.AreEquivalent(edges, graph.Edges.ToArray());
            CollectionAssert.AreEquivalent(verteces, graph.Vertices.ToArray());
        }
    }
}
