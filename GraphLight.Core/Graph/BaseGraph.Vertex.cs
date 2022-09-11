using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GraphLight.Graph
{
    public abstract partial class BaseGraph<TVertex, TEdge, TVertexData, TEdgeData> where TVertex : BaseGraph<TVertex, TEdge, TVertexData, TEdgeData>.Vertex, new()
        where TEdge : BaseGraph<TVertex, TEdge, TVertexData, TEdgeData>.Edge, new()
    {
        public abstract class Vertex : BaseViewModel
        {
            private readonly ICollection<TEdge> _edges = new ObservableCollection<TEdge>();
            private readonly ICollection<TEdge> _inEdges = new ObservableCollection<TEdge>();
            private readonly ICollection<TEdge> _outEdges = new ObservableCollection<TEdge>();
            private readonly ICollection<TEdge> _selfEdges = new ObservableCollection<TEdge>();
            private TVertexData _data;

            public TVertexData Data
            {
                get => _data;
                protected internal set => SetProperty(ref _data, value);
            }

            public IEnumerable<TEdge> Edges => _edges;

            public IEnumerable<TEdge> InEdges => _inEdges;

            public IEnumerable<TEdge> OutEdges => _outEdges;

            public IEnumerable<TEdge> SelfEdges => _selfEdges;

            internal void RegisterEdge(TEdge edge)
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

            internal void UnRegisterEdge(TEdge edge)
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
        }
    }
}