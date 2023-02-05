using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphLight.Model
{
    internal class GenericGraph<G, V, E> : IGraph<G, V, E>
    {
        private readonly IDictionary<V, IVertex<V>> _map = new Dictionary<V, IVertex<V>>();
        private readonly List<IEdge<V, E>> _edges = new List<IEdge<V, E>>();
        private readonly List<IVertex<V>> _vertices = new List<IVertex<V>>();

        internal GenericGraph(G data) => Data = data;

        public G Data { get; }

        public IReadOnlyList<IVertex<V>> Vertices => _vertices;

        public IReadOnlyList<IEdge<V, E>> Edges => _edges;

        public IVertex<V> this[V key] => _map[key];

        public IVertex<V> AddVertex(V data)
        {
            if (!_map.TryGetValue(data, out var vertex))
            {
                vertex = new GenericVertex<V, E>(data);
                _vertices.Add(vertex);
                _map.Add(data, vertex);
            }
            return vertex;
        }

        public void RemoveVertex(IVertex<V> vertex)
        {
            if (vertex == null)
                return;
            var edges = GetEdges(vertex).ToArray();
            foreach (var edge in edges)
                RemoveEdge(edge);
            _vertices.Remove(vertex);
            _map.Remove(vertex.Data);
        }

        public IReadOnlyList<IEdge<V, E>> GetEdges(IVertex<V> vertex) => 
            ((GenericVertex<V, E>)vertex).Edges;

        public IReadOnlyList<IEdge<V, E>> GetInEdges(IVertex<V> vertex) => 
            ((GenericVertex<V, E>)vertex).InEdges;

        public IReadOnlyList<IEdge<V, E>> GetOutEdges(IVertex<V> vertex) =>
            ((GenericVertex<V, E>)vertex).OutEdges;

        public IReadOnlyList<IEdge<V, E>> GetLoopEdges(IVertex<V> vertex) =>
            ((GenericVertex<V, E>)vertex).SelfEdges;

        public IVertex<V> InsertVertex(IEdge<V, E> edge, V vertexData, E edgeData)
        {
            if (!Edges.Contains(edge))
                throw new Exception("Данное ребро не принадлежит графу");
            var newEdge = AddEdge(vertexData, edge.Dst.Data, edgeData);
            ChangeDestination(edge, newEdge.Src);
            return newEdge.Src;
        }

        public IEdge<V, E> AddEdge(V srcData, V dstData, E data)
        {
            var src = AddVertex(srcData);
            var dst = AddVertex(dstData);
            var edge = new GenericEdge<V, E>(src, dst, data);
            RegisterEdge(src, edge);
            RegisterEdge(dst, edge);
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

        public void ChangeSource(IEdge<V, E> edge, IVertex<V> vertex)
        {
            var e = (GenericEdge<V, E>)edge;
            var oldVertex = e.Src;
            e.Src = vertex;
            OnEdgeChanged(edge, oldVertex, vertex);
        }

        public void ChangeDestination(IEdge<V, E> edge, IVertex<V> vertex)
        {
            var e = (GenericEdge<V, E>)edge;
            var oldVertex = e.Dst;
            e.Dst = vertex;
            OnEdgeChanged(edge, oldVertex, vertex);
        }

        private void OnEdgeChanged(IEdge<V, E> edge, IVertex<V>? oldVertex, IVertex<V>? newVertex)
        {
            if (oldVertex != null)
                UnRegisterEdge(oldVertex, edge);
            if (newVertex != null)
                RegisterEdge(newVertex, edge);
        }

        private static void RegisterEdge(IVertex<V> vertex, IEdge<V, E> edge)
        {
            var v = (GenericVertex<V,E>)vertex;
            var collection = edge.Src == vertex && edge.Dst == vertex
                ? v.SelfEdges
                : edge.Src == vertex
                    ? v.OutEdges
                    : edge.Dst == vertex
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

        public static void UnRegisterEdge(IVertex<V> vertex, IEdge<V, E> edge)
        {
            var v = (GenericVertex<V, E>)vertex;
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