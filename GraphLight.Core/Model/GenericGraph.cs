using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphLight.Model
{
    internal class GenericGraph<G, V, E> : IGraph<G, V, E>
    where V : class, IEquatable<V>
    {
        private readonly IDictionary<V, GenericVertex<V, E>> _map = new Dictionary<V, GenericVertex<V, E>>();
        private readonly List<IEdge<V, E>> _edges = new List<IEdge<V, E>>();
        private readonly List<V> _vertices = new List<V>();

        internal GenericGraph(G data) => Data = data;

        public G Data { get; }

        public IReadOnlyList<V> Vertices => _vertices;

        public IReadOnlyList<IEdge<V, E>> Edges => _edges;

        public void AddVertex(V data)
        {
            if (!_map.TryGetValue(data, out var vertex))
            {
                vertex = new GenericVertex<V, E>(data);
                _vertices.Add(data);
                _map.Add(data, vertex);
            }
        }

        public void RemoveVertex(V vertex)
        {
            if (vertex == null)
                return;
            var edges = GetEdges(vertex).ToArray();
            foreach (var edge in edges)
                RemoveEdge(edge);
            _vertices.Remove(vertex);
            _map.Remove(vertex);
        }

        public IReadOnlyList<IEdge<V, E>> GetEdges(V vertex) => _map[vertex].Edges;

        public IReadOnlyList<IEdge<V, E>> GetInEdges(V vertex) => _map[vertex].InEdges;

        public IReadOnlyList<IEdge<V, E>> GetOutEdges(V vertex) => _map[vertex].OutEdges;

        public IReadOnlyList<IEdge<V, E>> GetLoopEdges(V vertex) => _map[vertex].SelfEdges;

        public void InsertVertex(IEdge<V, E> edge, V vertexData, E edgeData)
        {
            if (!Edges.Contains(edge))
                throw new Exception("Данное ребро не принадлежит графу");
            var newEdge = AddEdge(vertexData, edge.Dst, edgeData);
            ChangeDestination(edge, newEdge.Src);
        }

        public IEdge<V, E> AddEdge(V srcData, V dstData, E data)
        {
            AddVertex(srcData);
            AddVertex(dstData);
            var edge = new GenericEdge<V, E>(srcData, dstData, data);
            RegisterEdge(srcData, edge);
            RegisterEdge(dstData, edge);
            _edges.Add(edge);
            return edge;
        }

        public void RemoveEdge(IEdge<V, E> edge)
        {
            _edges.Remove(edge);
            ChangeSource(edge, null);
            ChangeDestination(edge, null);
        }

        public void Revert(IEdge<V, E> edge)
        {
            var e = (GenericEdge<V, E>)edge;
            if (e.IsRevert)
                throw new Exception("Edge is already reverted.");
            var src = e.Src;
            var dst = e.Dst;
            ChangeSource(edge, dst);
            ChangeDestination(edge, src);
            e.IsRevert = !e.IsRevert;
        }

        public void ChangeSource(IEdge<V, E> edge, V vertex)
        {
            var e = (GenericEdge<V, E>)edge;
            var oldVertex = e.Src;
            e.Src = vertex;
            OnEdgeChanged(edge, oldVertex, vertex);
        }

        public void ChangeDestination(IEdge<V, E> edge, V vertex)
        {
            var e = (GenericEdge<V, E>)edge;
            var oldVertex = e.Dst;
            e.Dst = vertex;
            OnEdgeChanged(edge, oldVertex, vertex);
        }

        private void OnEdgeChanged(IEdge<V, E> edge, V oldVertex, V newVertex)
        {
            if (oldVertex != null)
                UnRegisterEdge(oldVertex, edge);
            if (newVertex != null)
                RegisterEdge(newVertex, edge);
        }

        private void RegisterEdge(V vertex, IEdge<V, E> edge)
        {
            var v = _map[vertex];
            var collection = vertex.Equals(edge.Src) && vertex.Equals(edge.Dst)
                ? v.SelfEdges
                : vertex.Equals(edge.Src)
                    ? v.OutEdges
                    : vertex.Equals(edge.Dst)
                        ? v.InEdges
                        : null;

            if (collection == null)
            {
                v.Edges.Remove(edge);
                v.SelfEdges.Remove(edge);
                v.InEdges.Remove(edge);
                v.OutEdges.Remove(edge);
            }
            else if (v.Edges.Contains(edge))
            {
                if (collection.Contains(edge))
                    return;
                v.SelfEdges.Remove(edge);
                v.InEdges.Remove(edge);
                v.OutEdges.Remove(edge);
                collection.Add(edge);
            }
            else
            {
                v.Edges.Add(edge);
                collection.Add(edge);
            }
        }

        private void UnRegisterEdge(V vertex, IEdge<V, E> edge)
        {
            var v = _map[vertex];
            if (v.SelfEdges.Remove(edge))
            {
                RegisterEdge(vertex, edge);
            }
            else
            {
                v.Edges.Remove(edge);
                v.InEdges.Remove(edge);
                v.OutEdges.Remove(edge);
            }
        }
    }
}