using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace GraphLight.Model
{
    internal class GenericGraph<G, V, E> : IGraph<G, V, E>
    {
        private readonly IDictionary<V, IVertex<V, E>> _map = new Dictionary<V, IVertex<V, E>>();
        private readonly List<IEdge<V, E>> _edges = new List<IEdge<V, E>>();
        private readonly List<IVertex<V, E>> _vertices = new List<IVertex<V, E>>();

        internal GenericGraph(G data) => Data = data;

        public G Data { get; }

        public IReadOnlyList<IVertex<V, E>> Vertices => _vertices;

        public IReadOnlyList<IEdge<V, E>> Edges => _edges;

        public IVertex<V, E> this[V key] => _map[key];

        public IVertex<V, E> AddVertex(V data)
        {
            if (!_map.TryGetValue(data, out var vertex))
            {
                vertex = new GenericVertex<V, E>(data);
                _vertices.Add(vertex);
                _map.Add(data, vertex);
            }
            return vertex;
        }

        public void RemoveVertex(IVertex<V, E> vertex)
        {
            if (vertex == null)
                return;
            var edges = GetEdges(vertex).ToArray();
            foreach (var edge in edges)
                RemoveEdge(edge);
            _vertices.Remove(vertex);
            _map.Remove(vertex.Data);
        }

        public IReadOnlyList<IEdge<V, E>> GetEdges(IVertex<V, E> vertex) => 
            ((GenericVertex<V, E>)vertex).Edges;

        public IReadOnlyList<IEdge<V, E>> GetInEdges(IVertex<V, E> vertex) => 
            ((GenericVertex<V, E>)vertex).InEdges;

        public IReadOnlyList<IEdge<V, E>> GetOutEdges(IVertex<V, E> vertex) =>
            ((GenericVertex<V, E>)vertex).OutEdges;

        public IReadOnlyList<IEdge<V, E>> GetLoopEdges(IVertex<V, E> vertex) =>
            ((GenericVertex<V, E>)vertex).SelfEdges;

        public IVertex<V, E> InsertVertex(IEdge<V, E> edge, V vertexData, E edgeData)
        {
            if (!Edges.Contains(edge))
                throw new Exception("Данное ребро не принадлежит графу");
            var newEdge = AddEdge(vertexData, edge.Dst.Data, edgeData);
            ((GenericEdge<V,E>)edge).Dst = newEdge.Src;
            return newEdge.Src;
        }

        public IEdge<V, E> AddEdge(V srcData, V dstData, E data)
        {
            var edge = new GenericEdge<V, E>(AddVertex(srcData), AddVertex(dstData), data);
            _edges.Add(edge);
            return edge;
        }

        public void RemoveEdge(IEdge<V, E> edge)
        {
            _edges.Remove(edge);
            ((GenericEdge<V, E>)edge).Src = null;
            ((GenericEdge<V, E>)edge).Dst = null;
        }

        public void Revert(IEdge<V, E> edge)
        {
            var e = (GenericEdge<V, E>)edge;
            if (e.IsRevert)
                throw new Exception("Edge is already reverted.");
            (e.Src, e.Dst) = (e.Dst, e.Src);
            e.IsRevert = !e.IsRevert;
        }

        public void ChangeSource(IEdge<V, E> edge, IVertex<V, E> vertex) => ((GenericEdge<V, E>)edge).Src = vertex;

        public void ChangeDestination(IEdge<V, E> edge, IVertex<V, E> vertex) => ((GenericEdge<V, E>)edge).Dst = vertex;
    }
}