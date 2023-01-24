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
    {
        private readonly IGraph<G, V, E> _graph;
        private readonly TraverseRule _rule;
        private readonly Dictionary<IVertex<V, E>, DfsVertexAttr> _attrs;
        private int _time;

        public DepthFirstSearch(IGraph<G, V, E> graph, TraverseRule rule)
        {
            _graph = graph;
            _rule = rule;
            // Initially we mark all nodes as white.
            _attrs = _graph.Vertices.ToDictionary(x => x, x => new DfsVertexAttr());
        }

        public Action<IVertex<V, E>>? OnNode { get; set; }

        public Action<IEdge<V, E>, DfsEdgeType>? OnEdge { get; set; }

        public void Execute()
        {
            foreach (var node in _graph.Vertices.Where(node => _attrs[node].Color == VertexColor.White))
                Dfs(node);
        }

        private void Dfs(IVertex<V, E> vertex)
        {
            if (_rule == TraverseRule.PreOrder)
                OnNode?.Invoke(vertex);

            var srcAttr = _attrs[vertex];
            _time++;
            srcAttr.Color = VertexColor.Gray;
            srcAttr.Depth = _time;

            foreach (var edge in vertex.OutEdges)
            {
                var dstAttr = _attrs[edge.Dst];
                switch (dstAttr.Color)
                {
                    case VertexColor.Black when srcAttr.Depth < dstAttr.Depth:
                        OnEdge?.Invoke(edge, DfsEdgeType.Forward);
                        break;
                    case VertexColor.Black:
                        OnEdge?.Invoke(edge, DfsEdgeType.Cross);
                        break;
                    case VertexColor.Gray:
                        OnEdge?.Invoke(edge, DfsEdgeType.Back);
                        break;
                    case VertexColor.White:
                        if (_rule == TraverseRule.PreOrder)
                            OnEdge?.Invoke(edge, DfsEdgeType.Tree);
                        Dfs(edge.Dst);
                        if (_rule == TraverseRule.PostOrder)
                            OnEdge?.Invoke(edge, DfsEdgeType.Tree);
                        break;
                }
            }

            srcAttr.Color = VertexColor.Black;
            _time++;

            if (_rule == TraverseRule.PostOrder)
                OnNode?.Invoke(vertex);
        }

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

        #endregion
    }
}