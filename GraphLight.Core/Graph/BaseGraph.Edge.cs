using System;

namespace GraphLight.Graph
{
    public abstract partial class BaseGraph<TVertex, TEdge, TVertexData, TEdgeData> where TVertex : BaseGraph<TVertex, TEdge, TVertexData, TEdgeData>.Vertex, new()
        where TEdge : BaseGraph<TVertex, TEdge, TVertexData, TEdgeData>.Edge, new()
    {
        public abstract class Edge : BaseViewModel
        {
            private TVertex _src;
            private TVertex _dst;
            private TEdgeData _data;

            public TEdgeData Data
            {
                get => _data;
                set => SetProperty(ref _data, value);
            }

            public TVertex Src
            {
                get => _src;
                set
                {
                    if (_src == value)
                        return;
                    var oldValue = _src;
                    _src = value;
                    RaisePropertyChanged();
                    OnEdgeChanged(oldValue, value);
                }
            }

            public TVertex Dst
            {
                get => _dst;
                set
                {
                    var oldValue = _dst;
                    if (oldValue == value)
                        return;
                    _dst = value;
                    RaisePropertyChanged();
                    OnEdgeChanged(oldValue, value);
                }
            }

            internal event EventHandler<EdgeChangedEventArgs> EdgeChanged;

            public override string ToString() => $"{Src} -> {Dst}: {Data}";

            private void OnEdgeChanged(TVertex oldVertex, TVertex newVertex) => 
                EdgeChanged?.Invoke(this, new EdgeChangedEventArgs(oldVertex, newVertex));
        }
    }
}