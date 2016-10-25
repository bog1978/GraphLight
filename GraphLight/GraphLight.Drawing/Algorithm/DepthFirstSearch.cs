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
    public class DepthFirstSearch
    {
        private readonly IGraph _graph;
        private Dictionary<IVertex, DfsVertexAttr> _attrs;
        private int _time;

        private Action<IEdge> _onBackEdge = x => { };
        private Action<IEdge> _onCrossEdge = x => { };
        private Action<IEdge> _onForwardEdge = x => { };
        private Action<IVertex> _onNode = x => { };
        private Action<IEdge> _onTreeEdge = x => { };

        public DepthFirstSearch(IGraph graph)
        {
            _graph = graph;
        }

        public Action<IVertex> OnNode
        {
            get { return _onNode; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _onNode = value;
            }
        }

        public Action<IEdge> OnTreeEdge
        {
            get { return _onTreeEdge; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _onTreeEdge = value;
            }
        }

        public Action<IEdge> OnBackEdge
        {
            get { return _onBackEdge; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _onBackEdge = value;
            }
        }

        public Action<IEdge> OnForwardEdge
        {
            get { return _onForwardEdge; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _onForwardEdge = value;
            }
        }

        public Action<IEdge> OnCrossEdge
        {
            get { return _onCrossEdge; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _onCrossEdge = value;
            }
        }

        public void Find()
        {
            // Initially we mark all nodes as white.
            _attrs = _graph.Verteces.ToDictionary(x => x, x => new DfsVertexAttr());
            foreach (var node in _graph.Verteces.Where(node => _attrs[node].Color == VertexColor.White))
                dfs(node);
        }

        private void dfs(IVertex vertex)
        {
            OnNode(vertex);
            var srcAttr = _attrs[vertex];
            _time++;
            srcAttr.Color = VertexColor.Gray;
            srcAttr.Depth = _time;

            foreach (var edge in vertex.OutEdges)
            {
                var dstAttr = _attrs[edge.Dst];
                switch (dstAttr.Color)
                {
                    case VertexColor.Black:
                        if (srcAttr.Depth < dstAttr.Depth)
                            OnForwardEdge(edge);
                        else
                            OnCrossEdge(edge);
                        break;
                    case VertexColor.Gray:
                        OnBackEdge(edge);
                        break;
                    case VertexColor.White:
                        OnTreeEdge(edge);
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

    public class DepthFirstSearch<TVertex, TEdge, TVertexData, TEdgeData>
        where TVertex : BaseGraph<TVertex, TEdge, TVertexData, TEdgeData>.Vertex, new()
        where TEdge : BaseGraph<TVertex, TEdge, TVertexData, TEdgeData>.Edge, new()
    {
        private readonly BaseGraph<TVertex, TEdge, TVertexData, TEdgeData> _graph;
        private Dictionary<TVertex, DfsVertexAttr> _attrs;
        private int _time;

        private Action<TEdge> _onBackEdge = x => { };
        private Action<TEdge> _onCrossEdge = x => { };
        private Action<TEdge> _onForwardEdge = x => { };
        private Action<TVertex> _onNode = x => { };
        private Action<TEdge> _onTreeEdge = x => { };

        public DepthFirstSearch(BaseGraph<TVertex, TEdge, TVertexData, TEdgeData> graph)
        {
            _graph = graph;
        }

        public Action<TVertex> OnNode
        {
            get { return _onNode; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _onNode = value;
            }
        }

        public Action<TEdge> OnTreeEdge
        {
            get { return _onTreeEdge; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _onTreeEdge = value;
            }
        }

        public Action<TEdge> OnBackEdge
        {
            get { return _onBackEdge; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _onBackEdge = value;
            }
        }

        public Action<TEdge> OnForwardEdge
        {
            get { return _onForwardEdge; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _onForwardEdge = value;
            }
        }

        public Action<TEdge> OnCrossEdge
        {
            get { return _onCrossEdge; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _onCrossEdge = value;
            }
        }

        public void Find()
        {
            // Initially we mark all nodes as white.
            _attrs = _graph.Verteces.ToDictionary(x => x, x => new DfsVertexAttr());
            foreach (var node in _graph.Verteces.Where(node => _attrs[node].Color == VertexColor.White))
                dfs(node);
        }

        private void dfs(TVertex vertex)
        {
            OnNode(vertex);
            var srcAttr = _attrs[vertex];
            _time++;
            srcAttr.Color = VertexColor.Gray;
            srcAttr.Depth = _time;

            foreach (var edge in vertex.OutEdges)
            {
                var dstAttr = _attrs[edge.Dst];
                switch (dstAttr.Color)
                {
                    case VertexColor.Black:
                        if (srcAttr.Depth < dstAttr.Depth)
                            OnForwardEdge(edge);
                        else
                            OnCrossEdge(edge);
                        break;
                    case VertexColor.Gray:
                        OnBackEdge(edge);
                        break;
                    case VertexColor.White:
                        OnTreeEdge(edge);
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