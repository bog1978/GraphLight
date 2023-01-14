﻿using System.Collections.Generic;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Test.Algorithm
{
    [TestClass]
    public class DepthFirstSearchTest
    {
        private readonly IEnumerable<IEdge<object, EdgeDataWeight>> _emptyEdges = Enumerable.Empty<IEdge<object, EdgeDataWeight>>();

        [TestMethod]
        public void DfsTest1()
        {
            var graph = GraphFactory.CreateInstance<object, object, EdgeDataWeight>("");
            var a = graph.AddVertex("A");
            var b = graph.AddVertex("B");
            var c = graph.AddVertex("C");

            var ab = graph.AddEdge("A", "B", 1);
            var bc = graph.AddEdge("B", "C", 1);
            var ca = graph.AddEdge("C", "A", 1);
            var ba = graph.AddEdge("B", "A", 1);

            checkResults(graph,
                new[] { a, b, c },
                new[] { ab, bc },
                _emptyEdges,
                new[] { ca, ba },
                _emptyEdges);
        }

        [TestMethod]
        public void DfsTest2()
        {
            var graph = GraphFactory.CreateInstance<object, object, EdgeDataWeight>("");
            var a = graph.AddVertex("A");
            var b = graph.AddVertex("B");
            var c = graph.AddVertex("C");
            var d = graph.AddVertex("D");

            var ab = graph.AddEdge("A", "B", 1);
            var ac = graph.AddEdge("A", "C", 1);
            var bd = graph.AddEdge("B", "D", 1);
            var cd = graph.AddEdge("C", "D", 1);
            var da = graph.AddEdge("D", "A", 1);
            checkResults(graph,
                new[] { a, b, d, c },
                new[] { ab, bd, ac },
                _emptyEdges,
                new[] { da },
                new[] { cd });
        }

        [TestMethod]
        public void DfsTest3()
        {
            var graph = GraphFactory.CreateInstance<object, object, EdgeDataWeight>("");
            var a = graph.AddVertex("A");
            var b = graph.AddVertex("B");

            var ab1 = graph.AddEdge("A", "B", 1);
            var ab2 = graph.AddEdge("A", "B", 2);

            checkResults(graph,
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
            var graph = GraphFactory.CreateInstance<object, object, EdgeDataWeight>("");
            var a = graph.AddVertex("A");
            var b = graph.AddVertex("B");
            var c = graph.AddVertex("C");
            var d = graph.AddVertex("D");
            var e = graph.AddVertex("E");
            var f = graph.AddVertex("F");
            var g = graph.AddVertex("G");

            var ab = graph.AddEdge("A", "B", 1);
            var ac = graph.AddEdge("A", "C", 1);
            var ae = graph.AddEdge("A", "E", 1);
            var bd = graph.AddEdge("B", "D", 1);
            var bf = graph.AddEdge("B", "F", 1);
            var fe = graph.AddEdge("F", "E", 1);
            var cg = graph.AddEdge("C", "G", 1);

            checkResults(graph,
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
            var graph = GraphFactory.CreateInstance<object, object, EdgeDataWeight>("");
            var n1 = graph.AddVertex("1");
            var n2 = graph.AddVertex("2");
            var n3 = graph.AddVertex("3");
            var n4 = graph.AddVertex("4");
            var n5 = graph.AddVertex("5");
            var n6 = graph.AddVertex("6");
            var n7 = graph.AddVertex("7");
            var n8 = graph.AddVertex("8");

            var e12 = graph.AddEdge("1", "2", 1);
            var e23 = graph.AddEdge("2", "3", 1);
            var e34 = graph.AddEdge("3", "4", 1);

            var e15 = graph.AddEdge("1", "5", 1);
            var e56 = graph.AddEdge("5", "6", 1);
            var e67 = graph.AddEdge("6", "7", 1);
            var e68 = graph.AddEdge("6", "8", 1);

            var e18 = graph.AddEdge("1", "8", 1);
            var e63 = graph.AddEdge("6", "3", 1);
            var e42 = graph.AddEdge("4", "2", 1);

            checkResults(graph,
                new[] { n1, n2, n3, n4, n5, n6, n7, n8 },
                new[] { e12, e23, e34, e15, e56, e67, e68 },
                new[] { e18 },
                new[] { e42 },
                new[] { e63 });
        }

        private static void checkResults(
            IGraph<object, object, EdgeDataWeight> graph,
            IEnumerable<IVertex<object, EdgeDataWeight>> nodesExpected,
            IEnumerable<IEdge<object, EdgeDataWeight>> treeEdgesExpected,
            IEnumerable<IEdge<object, EdgeDataWeight>> forwardExpected,
            IEnumerable<IEdge<object, EdgeDataWeight>> backwardExpected,
            IEnumerable<IEdge<object, EdgeDataWeight>> crossExpected)
        {
            var nodes = new List<IVertex<object, EdgeDataWeight>>();
            var backward = new List<IEdge<object, EdgeDataWeight>>();
            var forward = new List<IEdge<object, EdgeDataWeight>>();
            var tree = new List<IEdge<object, EdgeDataWeight>>();
            var cross = new List<IEdge<object, EdgeDataWeight>>();

            var alg = graph.DepthFirstSearch();
            alg.OnNode = nodes.Add;
            alg.OnTreeEdge = tree.Add;
            alg.OnBackEdge = backward.Add;
            alg.OnForwardEdge = forward.Add;
            alg.OnCrossEdge = cross.Add;

            alg.Execute();
            CollectionAssert.AreEqual(nodesExpected.ToList(), nodes, "Wrong nodes collection");
            CollectionAssert.AreEqual(treeEdgesExpected.ToList(), tree, "Wrong tree edges collection");
            CollectionAssert.AreEqual(forwardExpected.ToList(), forward, "Wrong forward edges collection");
            CollectionAssert.AreEqual(backwardExpected.ToList(), backward, "Wrong bckward edges collection");
            CollectionAssert.AreEqual(crossExpected.ToList(), cross, "Wrong cross edges collection");
        }
    }
}