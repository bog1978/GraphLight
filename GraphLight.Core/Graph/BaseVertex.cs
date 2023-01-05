﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using GraphLight.Collections;

namespace GraphLight.Graph
{
    public class BaseVertex<V, E> : IVertex<V, E>
    {
        private readonly ICollection<IEdge<V, E>> _edges = new ObservableCollection<IEdge<V, E>>();
        private readonly ICollection<IEdge<V, E>> _inEdges = new ObservableCollection<IEdge<V, E>>();
        private readonly ICollection<IEdge<V, E>> _outEdges = new ObservableCollection<IEdge<V, E>>();
        private readonly ICollection<IEdge<V, E>> _selfEdges = new ObservableCollection<IEdge<V, E>>();
        private readonly V _data;

        public BaseVertex(V data) => _data = data;

        public V Data => _data;

        public IEnumerable<IEdge<V, E>> Edges => _edges;

        public IEnumerable<IEdge<V, E>> InEdges => _inEdges;

        public IEnumerable<IEdge<V, E>> OutEdges => _outEdges;

        public IEnumerable<IEdge<V, E>> SelfEdges => _selfEdges;

        int IBinaryHeapItem<double>.HeapIndex { get; set; }

        double IBinaryHeapItem<double>.HeapKey { get; set; }

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
            obj is BaseVertex<V, E> other && Equals(Data, other.Data);

        public override int GetHashCode() =>
            Data.GetHashCode();
    }
}