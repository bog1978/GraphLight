﻿using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Algorithm
{
    [TestClass]
    public class DepthFirstSearchPreOrderTest
    {
        private readonly IEnumerable<IEdge<string, EdgeDataWeight>> _emptyEdges = Enumerable.Empty<IEdge<string, EdgeDataWeight>>();

        [TestMethod]
        public void DfsTest1()
        {
            var graph = Graph.CreateInstance<object, string, EdgeDataWeight>("");

            var (a, b, c) = ("A", "B", "C");
            graph.AddVertex(a);
            graph.AddVertex(b);
            graph.AddVertex(c);

            var ab = graph.AddEdge(a, b, 1);
            var bc = graph.AddEdge(b, c, 1);
            var ca = graph.AddEdge(c, a, 1);
            var ba = graph.AddEdge(b, a, 1);

            CheckResults(graph,
                new[] { a, b, c },
                new[] { ab, bc },
                _emptyEdges,
                new[] { ca, ba },
                _emptyEdges);
        }

        [TestMethod]
        public void DfsTest2()
        {
            var graph = Graph.CreateInstance<object, string, EdgeDataWeight>("");

            var (a, b, c, d) = ("A", "B", "C", "D");
            graph.AddVertex(a);
            graph.AddVertex(b);
            graph.AddVertex(c);
            graph.AddVertex(d);

            var ab = graph.AddEdge(a, b, 1);
            var ac = graph.AddEdge(a, c, 1);
            var bd = graph.AddEdge(b, d, 1);
            var cd = graph.AddEdge(c, d, 1);
            var da = graph.AddEdge(d, a, 1);
            CheckResults(graph,
                new[] { a, b, d, c },
                new[] { ab, bd, ac },
                new[] { cd },
                new[] { da },
                _emptyEdges);
        }

        [TestMethod]
        public void DfsTest3()
        {
            var graph = Graph.CreateInstance<object, string, EdgeDataWeight>("");

            var (a, b) = ("A", "B");
            graph.AddVertex(a);
            graph.AddVertex(b);

            var ab1 = graph.AddEdge(a, b, 1);
            var ab2 = graph.AddEdge(a, b, 2);

            CheckResults(graph,
                new[] { a, b },
                new[] { ab1 },
                new[] { ab2 },
                _emptyEdges,
                _emptyEdges);
        }

        /// <summary>
        /// Example from Wikipedia. 
        /// </summary>
        /// <remarks>
        /// http://en.wikipedia.org/wiki/File:Graph.traversal.example.svg
        /// </remarks>
        [TestMethod]
        public void DfsTest4()
        {
            var graph = Graph.CreateInstance<object, string, EdgeDataWeight>("");

            var (a, b, c, d, e, f, g) = ("A", "B", "C", "D", "E", "F", "G");
            graph.AddVertex(a);
            graph.AddVertex(b);
            graph.AddVertex(c);
            graph.AddVertex(d);
            graph.AddVertex(e);
            graph.AddVertex(f);
            graph.AddVertex(g);

            var ab = graph.AddEdge(a, b, 1);
            var ac = graph.AddEdge(a, c, 1);
            var ae = graph.AddEdge(a, e, 1);
            var bd = graph.AddEdge(b, d, 1);
            var bf = graph.AddEdge(b, f, 1);
            var fe = graph.AddEdge(f, e, 1);
            var cg = graph.AddEdge(c, g, 1);

            CheckResults(graph,
                new[] { a, b, d, f, e, c, g },
                new[] { ab, bd, bf, fe, ac, cg },
                new[] { ae },
                _emptyEdges,
                _emptyEdges);
        }

        /// <summary>
        /// Example from Wikipedia. 
        /// </summary>
        /// <remarks>
        /// http://en.wikipedia.org/wiki/File:Tree_edges.svg
        /// </remarks>
        [TestMethod]
        public void DfsTest5()
        {
            var graph = Graph.CreateInstance<object, string, EdgeDataWeight>("");

            var (a1, a2, a3, a4, a5, a6, a7, a8) = ("1", "2", "3", "4", "5", "6", "7", "8");
            graph.AddVertex(a1);
            graph.AddVertex(a2);
            graph.AddVertex(a3);
            graph.AddVertex(a4);
            graph.AddVertex(a5);
            graph.AddVertex(a6);
            graph.AddVertex(a7);
            graph.AddVertex(a8);

            var e12 = graph.AddEdge(a1, a2, 1);
            var e23 = graph.AddEdge(a2, a3, 1);
            var e34 = graph.AddEdge(a3, a4, 1);

            var e15 = graph.AddEdge(a1, a5, 1);
            var e56 = graph.AddEdge(a5, a6, 1);
            var e67 = graph.AddEdge(a6, a7, 1);
            var e68 = graph.AddEdge(a6, a8, 1);

            var e18 = graph.AddEdge(a1, a8, 1);
            var e63 = graph.AddEdge(a6, a3, 1);
            var e42 = graph.AddEdge(a4, a2, 1);

            CheckResults(graph,
                new[] { a1, a2, a3, a4, a5, a6, a7, a8 },
                new[] { e12, e23, e34, e15, e56, e67, e68 },
                new[] { e18 },
                new[] { e42 },
                new[] { e63 });
        }

        /// <summary>
        /// Классификация узлов. 
        /// </summary>
        [TestMethod]
        public void DfsTest6()
        {
            var graph = Graph.CreateInstance<object, string, EdgeDataWeight>("");

            var (a, b, c, d, e, f, g, h, i, j, k) =
                ("A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K");

            graph.AddVertexRange(a, b, c, d, e, f, g, h, i, j, k);
            graph.AddEdgeRange(1,
                (a, b), (b, c), (c, a),
                (d, e), (e, f), (e, g), (g, d),
                (h, i), (i, j), (j, k), (j, h));

            var vertexInf = new List<IVertexInfo<string>>();
            var dfs = graph.DepthFirstSearch(TraverseRule.PreOrder);
            dfs.OnNode += vertexInf.Add;
            dfs.Execute();

            var actual = vertexInf.Select(x => $"{x.Vertex}:{x.VertexType}").ToList();
            var expected = new List<string>
            {
                $"{a}:{DfsVertexType.Root}",
                $"{b}:{DfsVertexType.Middle}",
                $"{c}:{DfsVertexType.LeafCycle}",
                $"{d}:{DfsVertexType.Root}",
                $"{e}:{DfsVertexType.Middle}",
                $"{f}:{DfsVertexType.Leaf}",
                $"{g}:{DfsVertexType.LeafCycle}",
                $"{h}:{DfsVertexType.Root}",
                $"{i}:{DfsVertexType.Middle}",
                $"{j}:{DfsVertexType.MiddleCycle}",
                $"{k}:{DfsVertexType.Leaf}",
            };
            CollectionAssert.AreEqual(expected, actual);
        }

        private static void CheckResults(
            IGraph<object, string, EdgeDataWeight> graph,
            IEnumerable<string> nodesExpected,
            IEnumerable<IEdge<string, EdgeDataWeight>> treeEdgesExpected,
            IEnumerable<IEdge<string, EdgeDataWeight>> forwardExpected,
            IEnumerable<IEdge<string, EdgeDataWeight>> backwardExpected,
            IEnumerable<IEdge<string, EdgeDataWeight>> crossExpected)
        {
            var nodesInf = new List<IVertexInfo<string>>();
            var nodes = new List<string>();
            var backward = new List<IEdge<string, EdgeDataWeight>>();
            var forward = new List<IEdge<string, EdgeDataWeight>>();
            var tree = new List<IEdge<string, EdgeDataWeight>>();
            var cross = new List<IEdge<string, EdgeDataWeight>>();

            var alg = graph.DepthFirstSearch(TraverseRule.PreOrder);
            alg.OnNode = vi =>
            {
                nodesInf.Add(vi);
                nodes.Add(vi.Vertex);
            };
            alg.OnEdge = ei => (ei.EdgeType switch
            {
                DfsEdgeType.Forward => forward,
                DfsEdgeType.Cross => cross,
                DfsEdgeType.Back => backward,
                DfsEdgeType.Tree => tree,
                var t => throw new ArgumentOutOfRangeException(nameof(t), t, null)
            }).Add(ei.Edge);

            alg.Execute();
            CollectionAssert.AreEqual(nodesExpected.ToList(), nodes, "Wrong nodes collection");
            CollectionAssert.AreEqual(treeEdgesExpected.ToList(), tree, "Wrong tree edges collection");
            CollectionAssert.AreEqual(forwardExpected.ToList(), forward, "Wrong forward edges collection");
            CollectionAssert.AreEqual(backwardExpected.ToList(), backward, "Wrong bckward edges collection");
            CollectionAssert.AreEqual(crossExpected.ToList(), cross, "Wrong cross edges collection");
            for (var i = 0; i < nodesInf.Count; i++)
                Assert.AreEqual(i, nodesInf[i].Order);
        }
    }
}