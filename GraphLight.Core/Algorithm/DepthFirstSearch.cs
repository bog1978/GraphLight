#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GraphLight.Graph;

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
    internal class DepthFirstSearch<V, E> : IDepthFirstSearch<V, E>
    {
        private readonly IGraph<V, E> _graph;
        private readonly Dictionary<IVertex<V, E>, DfsVertexAttr> _attrs;
        private int _time;

        public DepthFirstSearch(IGraph<V, E> graph)
        {
            _graph = graph;
            // Initially we mark all nodes as white.
            _attrs = _graph.Vertices.ToDictionary(x => x, x => new DfsVertexAttr());
        }

        public Action<IVertex<V, E>>? OnNode { get; set; }

        public Action<IEdge<V, E>>? OnTreeEdge { get; set; }

        public Action<IEdge<V, E>>? OnBackEdge { get; set; }

        public Action<IEdge<V, E>>? OnForwardEdge { get; set; }

        public Action<IEdge<V, E>>? OnCrossEdge { get; set; }

        public Action<IEdge<V, E>, DfsEdgeType>? OnEdge { get; set; }

        public void Execute()
        {
            foreach (var node in _graph.Vertices.Where(node => _attrs[node].Color == VertexColor.White))
                dfs(node);
        }

        private void dfs(IVertex<V, E> vertex)
        {
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
                        OnForwardEdge?.Invoke(edge);
                        break;
                    case VertexColor.Black:
                        OnEdge?.Invoke(edge, DfsEdgeType.Cross);
                        OnCrossEdge?.Invoke(edge);
                        break;
                    case VertexColor.Gray:
                        OnEdge?.Invoke(edge, DfsEdgeType.Back);
                        OnBackEdge?.Invoke(edge);
                        break;
                    case VertexColor.White:
                        OnEdge?.Invoke(edge, DfsEdgeType.Tree);
                        OnTreeEdge?.Invoke(edge);
                        dfs(edge.Dst);
                        break;
                }
            }

            srcAttr.Color = VertexColor.Black;
            _time++;
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