using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    /// <summary>
    /// Depth-First Search algorithm for directed graph with edge classification.
    /// </summary> 
    /// <remarks>
    /// Depth-first search is a systematic way to find all the vertices reachable from a source vertex.
    /// DFS traverse a connected component of a given graph and defines a spanning tree. The basic idea
    /// of depth-first search is this: It methodically explore every edge. We start over from different
    /// vertices as necessary. As soon as we discover a vertex, DFS starts exploring from it.
    /// Consider a directed graph G = (V, E). After a DFS of graph G we can put each edge into one of four classes:
    /// 1. A tree edge is an edge in a DFS-tree.
    /// 2. A back edge connects a vertex to an ancestor in a DFS-tree. Note that a self-loop is a back edge.
    /// 3. A forward edge is a non-tree edge that connects a vertex to a descendent in a DFS-tree.
    /// 4. A cross edge is any other edge in graph G. It connects vertices in two different DFS-tree
    ///    or two vertices in the same DFS-tree neither of which is the ancestor of the other.
    /// More details can be found here:
    /// http://www.personal.kent.edu/~rmuhamma/Algorithms/MyAlgorithms/GraphAlgor/depthSearch.htm
    /// </remarks>
    internal class DepthFirstSearch<G, V, E> : IDepthFirstSearch<V, E>
    where V : IEquatable<V>
    {
        private readonly IGraph<G, V, E> _graph;
        private readonly TraverseRule _rule;
        private readonly Dictionary<V, DfsVertexAttr> _attrs;
        private int _depth;
        private int _vOrder;
        private int _eOrder;

        public DepthFirstSearch(IGraph<G, V, E> graph, TraverseRule rule)
        {
            _graph = graph;
            _rule = rule;
            // Initially we mark all nodes as white.
            _attrs = _graph.Vertices.ToDictionary(x => x, x => new DfsVertexAttr());
        }

        public Action<IVertexInfo<V>>? OnNode { get; set; }

        public Action<IEdgeInfo<V, E>>? OnEdge { get; set; }

        public void Execute()
        {
            _vOrder = 0;
            _eOrder = 0;
            // Первый проход - обходим корни деревьев (нет входящих ребер).
            foreach (var node in _graph.Vertices)
            {
                var attr = _attrs[node];
                if (attr.Color != VertexColor.White)
                    continue;
                var inEdges = _graph.GetInEdges(node);
                if (inEdges.Count > 0)
                    continue;
                _depth = 0;
                Dfs(node, true);
            }
            // Второй проход - обходим то, что осталось. Могли быть циклы.
            foreach (var node in _graph.Vertices)
            {
                var attr = _attrs[node];
                if (attr.Color != VertexColor.White)
                    continue;
                _depth = 0;
                Dfs(node, true);
            }
        }

        private void Dfs(V vertex, bool isRoot)
        {
            var outEdges = _graph.GetOutEdges(vertex);
            var vertexType = ClassifyVertex(outEdges, isRoot);
            var srcAttr = _attrs[vertex];
            srcAttr.Color = VertexColor.Gray;
            srcAttr.Depth = _depth;

            if (_rule == TraverseRule.PreOrder)
                EnterVertex(vertex, vertexType);

            foreach (var edge in outEdges)
            {
                var dstAttr = _attrs[edge.Dst];
                switch (dstAttr.Color)
                {
                    case VertexColor.Black when srcAttr.Depth < dstAttr.Depth:
                        EnterEdge(edge, DfsEdgeType.Forward);
                        break;
                    case VertexColor.Black:
                        EnterEdge(edge, DfsEdgeType.Cross);
                        break;
                    case VertexColor.Gray:
                        EnterEdge(edge, DfsEdgeType.Back);
                        break;
                    case VertexColor.White when _rule == TraverseRule.PreOrder:
                        EnterEdge(edge, DfsEdgeType.Tree);
                        _depth++;
                        Dfs(edge.Dst, false);
                        _depth--;
                        break;
                    case VertexColor.White when _rule == TraverseRule.PostOrder:
                        _depth++;
                        Dfs(edge.Dst, false);
                        _depth--;
                        EnterEdge(edge, DfsEdgeType.Tree);
                        break;
                }
            }

            srcAttr.Color = VertexColor.Black;
            if (_rule == TraverseRule.PostOrder)
                EnterVertex(vertex, vertexType);
        }

        private DfsVertexType ClassifyVertex(IReadOnlyCollection<IEdge<V, E>> outEdges, bool isRoot)
        {
            var cntOut = outEdges.Count;
            if (isRoot)
                return DfsVertexType.Root;
            if (cntOut == 0)
                return DfsVertexType.Leaf;
            var cntOutWhite = CountWhiteDst(outEdges);
            if (cntOut == cntOutWhite)
                return DfsVertexType.Middle;
            if (cntOutWhite == 0)
                return DfsVertexType.LeafCycle;
            return DfsVertexType.MiddleCycle;
        }

        private int CountWhiteDst(IEnumerable<IEdge<V, E>> edges) =>
            edges.Count(x => _attrs[x.Dst].Color == VertexColor.White);

        private void EnterEdge(IEdge<V, E> edge, DfsEdgeType edgeType) =>
            OnEdge?.Invoke(new EdgeInfo(edge, edgeType, _eOrder++));

        private void EnterVertex(V vertex, DfsVertexType vertexType) =>
            OnNode?.Invoke(new VertexInfo(vertex, _vOrder++, _depth, vertexType));

        #region Nested type: DfsNodeAttr

        [DebuggerDisplay("Color={Color}, Depth={Depth}")]
        private sealed class DfsVertexAttr
        {
            public VertexColor Color;
            public int Depth;

            public DfsVertexAttr()
            {
                Color = VertexColor.White;
                Depth = 0;
            }
        }

        private class EdgeInfo : IEdgeInfo<V, E>
        {
            public EdgeInfo(IEdge<V, E> edge, DfsEdgeType edgeType, int order)
            {
                Edge = edge;
                EdgeType = edgeType;
                Order = order;
            }

            public IEdge<V, E> Edge { get; }

            public DfsEdgeType EdgeType { get; }

            public int Order { get; }
            public override string ToString() => $"{Edge}, N:{Order}, T:{EdgeType}";
        }

        private class VertexInfo : IVertexInfo<V>
        {
            public VertexInfo(V vertex, int order, int depth, DfsVertexType vertexType)
            {
                Vertex = vertex;
                Order = order;
                Depth = depth;
                VertexType = vertexType;
            }

            public V Vertex { get; }

            public int Order { get; }

            public int Depth { get; }

            public DfsVertexType VertexType { get; }

            public override string ToString() => $"{Vertex}: N:{Order}, D:{Depth}, T:{VertexType}";
        }

        #endregion
    }
}