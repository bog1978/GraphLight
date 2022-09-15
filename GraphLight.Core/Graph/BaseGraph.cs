using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GraphLight.Graph
{
    public abstract class BaseGraph<V, E> : BaseViewModel, IGraph<V, E>
    {
        private readonly IDictionary<V, IVertex<V, E>> _map = new Dictionary<V, IVertex<V, E>>();
        private readonly ICollection<IEdge<V, E>> _edges = new ObservableCollection<IEdge<V, E>>();
        private readonly ICollection<IVertex<V, E>> _vertices = new ObservableCollection<IVertex<V, E>>();

        public IEnumerable<IVertex<V, E>> Vertices => _vertices;

        public IEnumerable<IEdge<V, E>> Edges => _edges;

        public IEnumerable<object> Elements => Enumerable.Union(
            Vertices.Cast<object>(),
            Edges.Cast<object>());

        public IEnumerable<object> All => 
            Enumerable.Union(
                Vertices.Cast<object>(),
                Edges.Cast<object>());

        public IVertex<V, E> this[V key] => _map[key];

        public IVertex<V, E> AddVertex(V data)
        {
            if (!_map.TryGetValue(data, out var vertex))
            {
                vertex = CreateVertex(data);
                _vertices.Add(vertex);
                _map.Add(data, vertex);
            }
            return vertex;
        }

        public void RemoveVertex(IVertex<V, E> vertex)
        {
            if (vertex == null)
                return;
            var edges = vertex.Edges.ToArray();
            foreach (var edge in edges)
                RemoveEdge(edge);
            _vertices.Remove(vertex);
            _map.Remove(vertex.Data);
        }

        public IVertex<V, E> InsertVertex(IEdge<V, E> edge, V vertexData, E edgeData)
        {
            if (!Edges.Contains(edge))
                throw new Exception("Данное ребро не принадлежит графу");
            var newEdge = AddEdge(vertexData, edge.Dst.Data, edgeData);
            edge.Dst = newEdge.Src;
            return newEdge.Src;
        }

        public IEdge<V, E> AddEdge(IVertex<V, E> src, IVertex<V, E> dst, E data)
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

        public IEdge<V, E> AddEdge(V srcData, V dstData, E data)
        {
            var edge = CreateEdge(data);
            edge.EdgeChanged += OnEdgeChanged;
            edge.Src = AddVertex(srcData);
            edge.Dst = AddVertex(dstData);
            _edges.Add(edge);
            return edge;
        }

        public void RemoveEdge(IEdge<V, E> edge)
        {
            _edges.Remove(edge);
            edge.Src = null;
            edge.Dst = null;
            edge.EdgeChanged -= OnEdgeChanged;
        }

        private void OnEdgeChanged(object sender, EdgeChangedEventArgs<V, E> args)
        {
            args.OldVertex?.UnRegisterEdge((IEdge<V, E>)sender);
            args.NewVertex?.RegisterEdge((IEdge<V, E>)sender);
        }

        protected abstract IVertex<V, E> CreateVertex(V data);
        protected abstract IEdge<V, E> CreateEdge(E data);
        public abstract V CreateVertexData(object id);
        public abstract E CreateEdgeData();
    }
}