﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GraphLight.Graph
{
    internal class GenericGraph<G, V, E> : IGraph<G, V, E>
    {
        private readonly IDictionary<V, IVertex<V, E>> _map = new Dictionary<V, IVertex<V, E>>();
        private readonly ICollection<IEdge<V, E>> _edges = new ObservableCollection<IEdge<V, E>>();
        private readonly ICollection<IVertex<V, E>> _vertices = new ObservableCollection<IVertex<V, E>>();

        internal GenericGraph(G data) => Data = data;

        public G Data { get; }

        public IEnumerable<IVertex<V, E>> Vertices => _vertices;

        public IEnumerable<IEdge<V, E>> Edges => _edges;

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

        public IEdge<V, E> AddEdge(V srcData, V dstData, E data)
        {
            var edge = new GenericEdge<V, E>(data);
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

        private static void OnEdgeChanged(object sender, EdgeChangedEventArgs<V, E> args)
        {
            args.OldVertex?.UnRegisterEdge((IEdge<V, E>)sender);
            args.NewVertex?.RegisterEdge((IEdge<V, E>)sender);
        }
    }
}