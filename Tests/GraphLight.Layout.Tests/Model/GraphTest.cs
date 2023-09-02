﻿using System.Linq;
using GraphLight.Algorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Model
{
    [TestClass]
    public class GraphTest
    {
        [TestMethod]
        public void TestAddInsertRemove()
        {
            var graph = Graph.CreateInstance<object, string, object>("");
            var g = graph;
            var (a, b) = ("A", "B");
            var ab = graph.AddEdge(a, b, new object());

            var cp = g.InsertControlPoint(ab, "cp", new object());
            Assert.IsTrue(ReferenceEquals(ab, graph.Edges.First()));
            Assert.IsTrue(ReferenceEquals(ab, graph.GetOutEdges(a).First()));
            Assert.IsTrue(ReferenceEquals(ab, graph.GetInEdges(cp).First()));
            Assert.IsTrue(ReferenceEquals(ab.Src, a));
            Assert.IsTrue(ReferenceEquals(ab.Dst, cp));

            g.RemoveControlPoint(cp);
            Assert.IsTrue(ReferenceEquals(ab, graph.Edges.First()));
            Assert.IsTrue(ReferenceEquals(ab, graph.GetOutEdges(a).First()));
            Assert.IsTrue(ReferenceEquals(ab, graph.GetInEdges(b).First()));
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
            var graph = Graph.CreateInstance<IGraphData, IVertexData, IEdgeData>(new GraphData());
            var a = new VertexData("A");
            var b = new VertexData("B");
            var c = new VertexData("C");
            graph.AddVertex(a);
            graph.AddVertex(b);
            graph.AddVertex(c);
            graph.AddEdge(a, b, new EdgeData());
            graph.AddEdge(b, c, new EdgeData());
            graph.AddEdge(a, c, new EdgeData());
            var engine = new GraphVizLayout
            {
                NodeMeasure = new DummyNodeMeasure(),
                Graph = graph
            };
            engine.Layout();
        }

        [TestMethod]
        public void Issue7610Test_2()
        {
            var graph = Graph.CreateInstance<IGraphData, IVertexData, IEdgeData>(new GraphData());
            var a = new VertexData("A");
            var b = new VertexData("B");
            var c = new VertexData("C");
            var d = new VertexData("D");
            graph.AddVertex(a);
            graph.AddVertex(b);
            graph.AddVertex(c);
            graph.AddVertex(d);
            graph.AddEdge(a, b, new EdgeData());
            graph.AddEdge(b, c, new EdgeData());
            graph.AddEdge(c, d, new EdgeData());
            graph.AddEdge(d, a, new EdgeData());
            var engine = new GraphVizLayout
            {
                NodeMeasure = new DummyNodeMeasure(),
                Graph = graph
            };
            engine.Layout();
        }
    }
}