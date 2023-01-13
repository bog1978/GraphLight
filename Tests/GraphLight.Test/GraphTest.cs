﻿using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Drawing;
using GraphLight.Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Test
{
    [TestClass]
    public class GraphTest
    {
        [TestMethod]
        public void TestAddInsertRemove()
        {
            var graph = new GenericGraph<object, object, object>("");
            var g = graph;
            var ab = graph.AddEdge("A", "B", new object());
            var a = ab.Src;
            var b = ab.Dst;

            var cp = g.InsertControlPoint(ab, new object(), new object());
            Assert.IsTrue(ReferenceEquals(ab, graph.Edges.First()));
            Assert.IsTrue(ReferenceEquals(ab, a.OutEdges.First()));
            Assert.IsTrue(ReferenceEquals(ab, cp.InEdges.First()));
            Assert.IsTrue(ReferenceEquals(ab.Src, a));
            Assert.IsTrue(ReferenceEquals(ab.Dst, cp));

            g.RemoveControlPoint(cp);
            Assert.IsTrue(ReferenceEquals(ab, graph.Edges.First()));
            Assert.IsTrue(ReferenceEquals(ab, a.OutEdges.First()));
            Assert.IsTrue(ReferenceEquals(ab, b.InEdges.First()));
            Assert.IsTrue(ReferenceEquals(ab.Src, a));
            Assert.IsTrue(ReferenceEquals(ab.Dst, b));
        }

        [TestMethod]
        public void ShortesdWayTest()
        {
            var arr = new[] { 22, 5, 3, 9, 7, 8, 93, 4, 21, 5, 67 };
            int result = arr.Aggregate((a, b) => a < b ? a : b);
            Assert.AreEqual(result, 3);
        }

        [TestMethod]
        public void Issue7610Test_1()
        {
            var graph = new GenericGraph<IGraphData, IVertexData, IEdgeData>(new GraphData());
            var a = graph.AddVertex(new VertexData("A"));
            var b = graph.AddVertex(new VertexData("B"));
            var c = graph.AddVertex(new VertexData("C"));
            graph.AddEdge(a, b, new EdgeData(null, null));
            graph.AddEdge(b, c, new EdgeData(null, null));
            graph.AddEdge(a, c, new EdgeData(null, null));
            var engine = new GraphVizLayout
                {
                    NodeMeasure = new WpfNodeMeasure(),
                    Graph = graph
                };
            engine.Layout();
        }

        [TestMethod]
        public void Issue7610Test_2()
        {
            var graph = new GenericGraph<IGraphData, IVertexData, IEdgeData>(new GraphData());
            var a = graph.AddVertex(new VertexData("A"));
            var b = graph.AddVertex(new VertexData("B"));
            var c = graph.AddVertex(new VertexData("C"));
            var d = graph.AddVertex(new VertexData("D"));
            graph.AddEdge(a, b, new EdgeData(null, null));
            graph.AddEdge(b, c, new EdgeData(null, null));
            graph.AddEdge(c, d, new EdgeData(null, null));
            graph.AddEdge(d, a, new EdgeData(null, null));
            var engine = new GraphVizLayout
            {
                    NodeMeasure = new WpfNodeMeasure(),
                    Graph = graph
                };
            engine.Layout();
        }
    }
}