using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GraphLight.Graph
{
    public abstract partial class BaseGraph<TVertex, TEdge, TVertexData, TEdgeData> : BaseViewModel
        where TVertex : BaseGraph<TVertex, TEdge, TVertexData, TEdgeData>.Vertex, new()
        where TEdge : BaseGraph<TVertex, TEdge, TVertexData, TEdgeData>.Edge, new()
    {
        private readonly IDictionary<TVertexData, TVertex> _map = new Dictionary<TVertexData, TVertex>();
        private readonly ICollection<TEdge> _edges = new ObservableCollection<TEdge>();
        private readonly ICollection<TVertex> _vertices = new ObservableCollection<TVertex>();

        public IEnumerable<TVertex> Vertices => _vertices;

        public IEnumerable<TEdge> Edges => _edges;

        public TVertex this[TVertexData key] => _map[key];

        public TVertex AddVertex(TVertexData data)
        {
            if (!_map.TryGetValue(data, out var vertex))
            {
                vertex = new TVertex { Data = data };
                _vertices.Add(vertex);
                _map.Add(data, vertex);
            }
            return vertex;
        }

        public void RemoveVertex(TVertex vertex)
        {
            if (vertex == null)
                return;
            var edges = vertex.Edges.ToArray();
            foreach (var edge in edges)
                RemoveEdge(edge);
            _vertices.Remove(vertex);
            _map.Remove(vertex.Data);
        }

        public TVertex InsertVertex(TEdge edge, TVertexData vertexData)
        {
            if (!Edges.Contains(edge))
                throw new Exception("Данное ребро не принадлежит графу");
            var newEdge = AddEdge(vertexData, edge.Dst.Data, CreateEdgeData());
            edge.Dst = newEdge.Src;
            return newEdge.Src;
        }

        public TEdge AddEdge(TVertexData srcData, TVertexData dstData)
        {
            return AddEdge(srcData, dstData, CreateEdgeData());
        }

        public TEdge AddEdge(TVertex src, TVertex dst, TEdgeData data = default)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));

            if (dst == null)
                throw new ArgumentNullException(nameof(dst));

            _map.TryGetValue(src.Data, out var existingSrc);
            _map.TryGetValue(dst.Data, out var existingDst);

            if (existingSrc != src)
                throw new ArgumentOutOfRangeException(nameof(src));

            if (existingDst != dst)
                throw new ArgumentOutOfRangeException(nameof(dst));

            return AddEdge(src.Data, dst.Data, data);
        }

        public TEdge AddEdge(TVertexData srcData, TVertexData dstData, TEdgeData data)
        {
            var edge = new TEdge();
            edge.EdgeChanged += OnEdgeChanged;
            edge.Data = data;
            edge.Src = AddVertex(srcData);
            edge.Dst = AddVertex(dstData);
            _edges.Add(edge);
            return edge;
        }

        public void RemoveEdge(TEdge edge)
        {
            _edges.Remove(edge);
            edge.Data = default;
            edge.Src = null;
            edge.Dst = null;
            edge.EdgeChanged -= OnEdgeChanged;
        }

        private void OnEdgeChanged(object sender, EdgeChangedEventArgs args)
        {
            args.OldVertex?.UnRegisterEdge((TEdge)sender);
            args.NewVertex?.RegisterEdge((TEdge)sender);
        }

        protected abstract TVertexData CreateVertexData();

        protected abstract TEdgeData CreateEdgeData();
    }
}