using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GraphLight.ViewModel;

namespace GraphLight.Graph
{
    public abstract class BaseGraph<TVertex, TEdge, TVertexData, TEdgeData> : BaseViewModel
        where TVertex : BaseGraph<TVertex, TEdge, TVertexData, TEdgeData>.Vertex, new()
        where TEdge : BaseGraph<TVertex, TEdge, TVertexData, TEdgeData>.Edge, new()
    {
        private readonly IDictionary<TVertexData, TVertex> _map = new Dictionary<TVertexData, TVertex>();
        private readonly ObservableCollection<TEdge> _edges = new ObservableCollection<TEdge>();
        private readonly ObservableCollection<TVertex> _vertices = new ObservableCollection<TVertex>();
        //internal delegate void EdgeChanged(TEdge edge, TVertex oldVertex, TVertex newVertex);
        internal class EdgeChangedEventArgs : EventArgs
        {
            public EdgeChangedEventArgs(TVertex oldVertex, TVertex newVertex)
            {
                OldVertex = oldVertex;
                NewVertex = newVertex;
            }

            public TVertex OldVertex { get; private set; }
            public TVertex NewVertex { get; private set; }
        }

        public IEnumerable<TVertex> Verteces
        {
            get { return _vertices; }
        }

        public IEnumerable<TEdge> Edges
        {
            get { return _edges; }
        }

        public TVertex this[TVertexData key]
        {
            get { return _map[key]; }
        }

        public TVertex AddVertex(TVertexData data)
        {
            TVertex vertex;
            if (!_map.TryGetValue(data, out vertex))
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

        public TEdge AddEdge(TVertex src, TVertex dst, TEdgeData data = default(TEdgeData))
        {
            if (src == null)
                throw new ArgumentNullException("src");

            if (dst == null)
                throw new ArgumentNullException("dst");

            TVertex existingSrc, existingDst;

            _map.TryGetValue(src.Data, out existingSrc);
            _map.TryGetValue(dst.Data, out existingDst);

            if (existingSrc != src)
                throw new ArgumentOutOfRangeException("src");

            if (existingDst != dst)
                throw new ArgumentOutOfRangeException("dst");

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
            edge.Data = default(TEdgeData);
            edge.Src = null;
            edge.Dst = null;
            edge.EdgeChanged -= OnEdgeChanged;
        }

        private void OnEdgeChanged(object sender, EdgeChangedEventArgs args)
        {
            if (args.OldVertex != null)
                args.OldVertex.UnregisterEdge((TEdge)sender);
            if (args.NewVertex != null)
                args.NewVertex.RegisterEdge((TEdge)sender);
        }

        protected abstract TVertexData CreateVertexData();
        protected abstract TEdgeData CreateEdgeData();

        public abstract class Vertex : BaseViewModel
        {
            private readonly ObservableCollection<TEdge> _edges = new ObservableCollection<TEdge>();
            private readonly ObservableCollection<TEdge> _inEdges = new ObservableCollection<TEdge>();
            private readonly ObservableCollection<TEdge> _outEdges = new ObservableCollection<TEdge>();
            private readonly ObservableCollection<TEdge> _selfEdges = new ObservableCollection<TEdge>();
            private TVertexData _data;

            public TVertexData Data
            {
                get { return _data; }
                internal set { SetProperty(ref _data, value, "Data"); }
            }

            public IEnumerable<TEdge> Edges
            {
                get { return _edges; }
            }

            public IEnumerable<TEdge> InEdges
            {
                get { return _inEdges; }
            }

            public IEnumerable<TEdge> OutEdges
            {
                get { return _outEdges; }
            }

            public IEnumerable<TEdge> SelfEdges
            {
                get { return _selfEdges; }
            }

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

            internal void UnregisterEdge(TEdge edge)
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

            public override string ToString()
            {
                return Data.ToString();
            }
        }

        public abstract class Edge : BaseViewModel
        {
            private TVertex _src;
            private TVertex _dst;
            private TEdgeData _data;

            public TEdgeData Data
            {
                get { return _data; }
                set { SetProperty(ref _data, value, "Data"); }
            }

            public TVertex Src
            {
                get { return _src; }
                set
                {
                    if (_src == value)
                        return;
                    var oldValue = _src;
                    _src = value;
                    RaisePropertyChanged("Src");
                    OnEdgeChanged(oldValue, value);
                }
            }

            public TVertex Dst
            {
                get { return _dst; }
                set
                {
                    var oldValue = _dst;
                    if (oldValue == value)
                        return;
                    _dst = value;
                    RaisePropertyChanged("Dst");
                    OnEdgeChanged(oldValue, value);
                }
            }

            internal event EventHandler<EdgeChangedEventArgs> EdgeChanged;

            public override string ToString()
            {
                return string.Format("{0} -> {1}: {2}", Src, Dst, Data);
            }

            private void OnEdgeChanged(TVertex oldVertex, TVertex newVertex)
            {
                var handler = EdgeChanged;
                if (handler != null)
                    handler(this, new EdgeChangedEventArgs(oldVertex, newVertex));
            }
        }
    }
}