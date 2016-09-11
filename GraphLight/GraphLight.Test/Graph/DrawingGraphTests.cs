using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Drawing;
using GraphLight.Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Test.Graph
{
    [TestClass]
    public class DrawingGraphTests
    {
        private static readonly ICollection _emptyEdges = new List<IEdge>();

        [TestMethod]
        public void Test1()
        {
            var g = new GraphModel();
            var a = g.AddVertex("a");
            var b = g.AddVertex("b");
            var c = g.AddVertex("c");
            var aa = g.AddEdge(a.Data, a.Data);
            var ab = g.AddEdge(a.Data, b.Data);
            var ac = g.AddEdge(a.Data, c.Data);
            var bc = g.AddEdge(b.Data, c.Data);

            checkEdges(a, new[] { aa, ab, ac }, _emptyEdges, new[] { ab, ac }, new[] { aa });
            checkEdges(b, new[] { ab, bc }, new[] { ab }, new[] { bc }, _emptyEdges);
            checkEdges(c, new[] { ac, bc }, new[] { ac, bc }, _emptyEdges, _emptyEdges);
            checkGraph(g, new[] { aa, ab, ac, bc }, new[] { a, b, c });

            ab.Src = c;
            checkEdges(a, new[] { aa, ac }, _emptyEdges, new[] { ac }, new[] { aa });
            checkEdges(b, new[] { ab, bc }, new[] { ab }, new[] { bc }, _emptyEdges);
            checkEdges(c, new[] { ab, ac, bc }, new[] { ac, bc }, new[] { ab }, _emptyEdges);
            checkGraph(g, new[] { aa, ab, ac, bc }, new[] { a, b, c });

            ab.Src = null;
            checkEdges(a, new[] { aa, ac }, _emptyEdges, new[] { ac }, new[] { aa });
            checkEdges(b, new[] { ab, bc }, new[] { ab }, new[] { bc }, _emptyEdges);
            checkEdges(c, new[] { ac, bc }, new[] { ac, bc }, _emptyEdges, _emptyEdges);
            checkGraph(g, new[] { aa, ab, ac, bc }, new[] { a, b, c });

            ab.Dst = null;
            checkEdges(a, new[] { aa, ac }, _emptyEdges, new[] { ac }, new[] { aa });
            checkEdges(b, new[] { bc }, _emptyEdges, new[] { bc }, _emptyEdges);
            checkEdges(c, new[] { ac, bc }, new[] { ac, bc }, _emptyEdges, _emptyEdges);
            checkGraph(g, new[] { aa, ab, ac, bc }, new[] { a, b, c });

            ab.Dst = b;
            checkEdges(a, new[] { aa, ac }, _emptyEdges, new[] { ac }, new[] { aa });
            checkEdges(b, new[] { ab, bc }, new[] { ab }, new[] { bc }, _emptyEdges);
            checkEdges(c, new[] { ac, bc }, new[] { ac, bc }, _emptyEdges, _emptyEdges);
            checkGraph(g, new[] { aa, ab, ac, bc }, new[] { a, b, c });

            ab.Src = a;
            checkEdges(a, new[] { aa, ab, ac }, _emptyEdges, new[] { ab, ac }, new[] { aa });
            checkEdges(b, new[] { ab, bc }, new[] { ab }, new[] { bc }, _emptyEdges);
            checkEdges(c, new[] { ac, bc }, new[] { ac, bc }, _emptyEdges, _emptyEdges);
            checkGraph(g, new[] { aa, ab, ac, bc }, new[] { a, b, c });

            g.RemoveVertex(b);
            checkEdges(a, new[] { aa, ac }, _emptyEdges, new[] { ac }, new[] { aa });
            checkEdges(b, _emptyEdges, _emptyEdges, _emptyEdges, _emptyEdges);
            checkEdges(c, new[] { ac }, new[] { ac }, _emptyEdges, _emptyEdges);
            checkGraph(g, new[] { aa, ac }, new[] { a, c });

            g.RemoveEdge(ac);
            checkEdges(a, new[] { aa }, _emptyEdges, _emptyEdges, new[] { aa });
            checkEdges(b, _emptyEdges, _emptyEdges, _emptyEdges, _emptyEdges);
            checkEdges(c, _emptyEdges, _emptyEdges, _emptyEdges, _emptyEdges);
            checkGraph(g, new[] { aa }, new[] { a, c });

            aa.Src = null;
            checkEdges(a, new[] { aa }, new[] { aa }, _emptyEdges, _emptyEdges);
            checkGraph(g, new[] { aa }, new[] { a, c });

            aa.Src = a;
            checkEdges(a, new[] { aa }, _emptyEdges, _emptyEdges, new[] { aa });
            checkGraph(g, new[] { aa }, new[] { a, c });

            aa.Dst = null;
            checkEdges(a, new[] { aa }, _emptyEdges, new[] { aa }, _emptyEdges);
            checkGraph(g, new[] { aa }, new[] { a, c });

            aa.Dst = c;
            checkEdges(a, new[] { aa }, _emptyEdges, new[] { aa }, _emptyEdges);
            checkEdges(c, new[] { aa }, new[] { aa }, _emptyEdges, _emptyEdges);
            checkGraph(g, new[] { aa }, new[] { a, c });
        }

        private static void checkEdges(
            IVertex vertex,
            ICollection allEdges,
            ICollection inEdges,
            ICollection outEdges,
            ICollection selfEdges)
        {
            CollectionAssert.AreEquivalent(vertex.Edges.ToList(), allEdges);
            CollectionAssert.AreEquivalent(vertex.InEdges.ToList(), inEdges);
            CollectionAssert.AreEquivalent(vertex.OutEdges.ToList(), outEdges);
            CollectionAssert.AreEquivalent(vertex.SelfEdges.ToList(), selfEdges);
        }

        private static void checkGraph(IGraph graph, ICollection edges, ICollection verteces)
        {
            CollectionAssert.AreEquivalent(edges, graph.Edges.ToArray());
            CollectionAssert.AreEquivalent(verteces, graph.Verteces.ToArray());
            var elements = edges.OfType<object>().Union(verteces.OfType<object>()).ToList();
            CollectionAssert.AreEquivalent(elements, graph.Elements.ToArray());
        }
    }
}
