using System;
using System.Collections.Generic;

namespace GraphLight.Model
{
    internal class GenericVertex<V, E> : IVertex<V, E>
    {
        private readonly List<IEdge<V, E>> _edges = new List<IEdge<V, E>>();
        private readonly List<IEdge<V, E>> _inEdges = new List<IEdge<V, E>>();
        private readonly List<IEdge<V, E>> _outEdges = new List<IEdge<V, E>>();
        private readonly List<IEdge<V, E>> _selfEdges = new List<IEdge<V, E>>();

        public GenericVertex(V data) => Data = data;

        public V Data { get; }

        public IReadOnlyList<IEdge<V, E>> Edges => _edges;

        public IReadOnlyList<IEdge<V, E>> InEdges => _inEdges;

        public IReadOnlyList<IEdge<V, E>> OutEdges => _outEdges;

        public IReadOnlyList<IEdge<V, E>> SelfEdges => _selfEdges;

        public void RegisterEdge(IEdge<V, E> edge)
        {
            var collection = edge.Src == this && edge.Dst == this
                ? _selfEdges
                : edge.Src == this
                    ? _outEdges
                    : edge.Dst == this
                        ? _inEdges
                        : null;

            if (collection == null)
            {
                _edges.Remove(edge);
                _selfEdges.Remove(edge);
                _inEdges.Remove(edge);
                _outEdges.Remove(edge);
            }
            else if (_edges.Contains(edge))
            {
                if (collection.Contains(edge))
                    return;
                _selfEdges.Remove(edge);
                _inEdges.Remove(edge);
                _outEdges.Remove(edge);
                collection.Add(edge);
            }
            else
            {
                _edges.Add(edge);
                collection.Add(edge);
            }
        }

        public void UnRegisterEdge(IEdge<V, E> edge)
        {
            if (_selfEdges.Remove(edge))
            {
                RegisterEdge(edge);
            }
            else
            {
                _edges.Remove(edge);
                _inEdges.Remove(edge);
                _outEdges.Remove(edge);
            }
        }

        public override string ToString() => Data.ToString();

        public override bool Equals(object obj) =>
            obj is GenericVertex<V, E> other && Equals(Data, other.Data);

        public override int GetHashCode() =>
            Data.GetHashCode();
    }
}