using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

using GraphLight.Collections;
using GraphLight.Geometry;
using GraphLight.Layout;

namespace GraphLight.Graph
{
    public abstract class BaseGraph<V, E> : BaseViewModel, IGraph<V, E>
    {
        private readonly IDictionary<V, Vertex> _map = new Dictionary<V, Vertex>();
        private readonly ObservableCollection<Edge> _edges = new ObservableCollection<Edge>();
        private readonly ObservableCollection<Vertex> _vertices = new ObservableCollection<Vertex>();

        private double _width;
        private double _height;

        //internal delegate void EdgeChanged(TEdge edge, TVertex oldVertex, TVertex newVertex);
        internal class EdgeChangedEventArgs : EventArgs
        {
            public EdgeChangedEventArgs(Vertex oldVertex, Vertex newVertex)
            {
                OldVertex = oldVertex;
                NewVertex = newVertex;
            }

            public Vertex OldVertex { get; }
            public Vertex NewVertex { get; }
        }

        public IEnumerable<Vertex> Vertices => _vertices;

        public IEnumerable<Edge> Edges => _edges;

        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        public IEnumerable<IElement> Elements => Vertices.OfType<IElement>().Union(Edges);

        IEnumerable<IEdge> IGraph.Edges => Edges;

        IEnumerable<IVertex> IGraph.Vertices => Vertices;

        IEdge IGraph.AddEdge(object src, object dst) => AddEdge((V)src, (V)dst);

        IEdge IGraph.AddEdge(object src, object dst, object data) => AddEdge((V)src, (V)dst, (E)data);

        IVertex IGraph.InsertVertex(IEdge edge) => InsertVertex((Edge)edge, CreateVertexData());

        void IGraph.RemoveEdge(IEdge edge) => RemoveEdge((Edge)edge);

        void IGraph.RemoveVertex(IVertex vertex) => RemoveVertex((Vertex)vertex);

        IVertex IGraph.this[object key] => this[(V)key];

        public IVertex AddVertex() => AddVertex(CreateVertexData());

        IVertex IGraph.AddVertex(object data) => AddVertex((V)data);

        public Vertex this[V key] => _map[key];

        public Vertex AddVertex(V data)
        {
            if (!_map.TryGetValue(data, out var vertex))
            {
                vertex = new Vertex { Data = data };
                _vertices.Add(vertex);
                _map.Add(data, vertex);
            }
            return vertex;
        }

        public void RemoveVertex(Vertex vertex)
        {
            if (vertex == null)
                return;
            var edges = vertex.Edges.ToArray();
            foreach (var edge in edges)
                RemoveEdge(edge);
            _vertices.Remove(vertex);
            _map.Remove(vertex.Data);
        }

        public Vertex InsertVertex(Edge edge, V vertexData)
        {
            if (!Edges.Contains(edge))
                throw new Exception("Данное ребро не принадлежит графу");
            var newEdge = AddEdge(vertexData, edge.Dst.Data, CreateEdgeData());
            edge.Dst = newEdge.Src;
            return newEdge.Src;
        }

        public Edge AddEdge(V srcData, V dstData)
        {
            return AddEdge(srcData, dstData, CreateEdgeData());
        }

        public Edge AddEdge(Vertex src, Vertex dst, E data = default)
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

        public Edge AddEdge(V srcData, V dstData, E data)
        {
            var edge = new Edge();
            edge.EdgeChanged += OnEdgeChanged;
            edge.Data = data;
            edge.Src = AddVertex(srcData);
            edge.Dst = AddVertex(dstData);
            _edges.Add(edge);
            return edge;
        }

        public void RemoveEdge(Edge edge)
        {
            _edges.Remove(edge);
            edge.Data = default;
            edge.Src = null;
            edge.Dst = null;
            edge.EdgeChanged -= OnEdgeChanged;
        }

        private void OnEdgeChanged(object sender, EdgeChangedEventArgs args)
        {
            args.OldVertex?.UnRegisterEdge((Edge)sender);
            args.NewVertex?.RegisterEdge((Edge)sender);
        }

        protected abstract V CreateVertexData();

        protected abstract E CreateEdgeData();

        public class Vertex : BaseViewModel, IVertex<V, E>
        {
            private readonly ObservableCollection<Edge> _edges = new ObservableCollection<Edge>();
            private readonly ObservableCollection<Edge> _inEdges = new ObservableCollection<Edge>();
            private readonly ObservableCollection<Edge> _outEdges = new ObservableCollection<Edge>();
            private readonly ObservableCollection<Edge> _selfEdges = new ObservableCollection<Edge>();
            private V _data;

            private string _category;
            private double _centerX;
            private double _height;
            private bool _isHighlighted;
            private bool _isSelected;
            private bool _isTmp;
            private string _label;
            private double _left;
            private int _position;
            private int _rank;
            private string _shapeData;
            private double _top;
            private double _width;
            private int _zIndex;

            public Vertex() => ShapeData = "M 0,1 A 1,1 0 1 0 2,1 A 1,1 0 1 0 0,1";

            public Vertex(V data) : this() => Data = data;

            public int ZIndex
            {
                get => _zIndex;
                set => SetProperty(ref _zIndex, value);
            }

            public bool IsSelected
            {
                get => _isSelected;
                set
                {
                    SetProperty(ref _isSelected, value);
                    IsHighlighted = value;
                }
            }

            public bool IsHighlighted
            {
                get => _isHighlighted;
                set => SetProperty(ref _isHighlighted, value);
            }

            public string Label
            {
                get => _label;
                set => SetProperty(ref _label, value);
            }

            public double Width
            {
                get => _width;
                set
                {
                    SetProperty(ref _width, value);
                    RaisePropertyChanged(nameof(Right));
                }
            }

            public double Height
            {
                get => _height;
                set
                {
                    SetProperty(ref _height, value);
                    RaisePropertyChanged(nameof(Bottom));
                }
            }

            public double Left
            {
                get => _left;
                set => SetProperty(ref _left, value);
            }

            public double Top
            {
                get => _top;
                set => SetProperty(ref _top, value);
            }

            public double Right => _left + _width;

            public double Bottom => _top + _height;

            public double CenterX
            {
                get => _centerX;
                set => SetProperty(ref _centerX, value);
            }

            public string ShapeData
            {
                get => _shapeData;
                set => SetProperty(ref _shapeData, value);
            }

            public int Rank
            {
                get => _rank;
                set => SetProperty(ref _rank, value);
            }

            public int Position
            {
                get => _position;
                set => SetProperty(ref _position, value);
            }

            public bool IsTmp
            {
                get => _isTmp;
                set => SetProperty(ref _isTmp, value);
            }

            public string Category
            {
                get => _category;
                set => SetProperty(ref _category, value);
            }

            public V Data
            {
                get => _data;
                internal set => SetProperty(ref _data, value);
            }

            object IElement.Data => Data;

            int IBinaryHeapItem<double>.HeapIndex { get; set; }

            double IBinaryHeapItem<double>.HeapKey { get; set; }

            public IEnumerable<Edge> Edges => _edges;

            public IEnumerable<Edge> InEdges => _inEdges;

            public IEnumerable<Edge> OutEdges => _outEdges;

            public IEnumerable<Edge> SelfEdges => _selfEdges;

            IEnumerable<IEdge> IVertex.Edges => _edges;

            IEnumerable<IEdge> IVertex.InEdges => _inEdges;

            IEnumerable<IEdge> IVertex.OutEdges => _outEdges;

            IEnumerable<IEdge> IVertex.SelfEdges => _selfEdges;

            IEnumerable<IEdge<V, E>> IVertex<V, E>.Edges => _edges;

            IEnumerable<IEdge<V, E>> IVertex<V, E>.InEdges => _inEdges;

            IEnumerable<IEdge<V, E>> IVertex<V, E>.OutEdges => _outEdges;

            IEnumerable<IEdge<V, E>> IVertex<V, E>.SelfEdges => _selfEdges;

            public void Update()
            {
                foreach (var e in Edges)
                {
                    var pts = e.Points;
                    using (e.DeferRefresh())
                    {
                        if (pts.Count == 2 || e.Src == e.Dst)
                        {
                            e.UpdateSrcPort();
                            e.UpdateDstPort();
                        }
                        else if (e.Src == this)
                            e.UpdateSrcPort();
                        else
                            e.UpdateDstPort();
                        var first = e.Points.First();
                        var last = e.Points.Last();
                        e.FixDraggablePoints(first);
                        e.FixDraggablePoints(last);
                    }
                }
            }

            internal void RegisterEdge(Edge edge)
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

            internal void UnRegisterEdge(Edge edge)
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

            public override bool Equals(object obj) => obj is Vertex other && Equals(Data, other.Data);

            public override int GetHashCode() => Data.GetHashCode();
        }

        public class Edge : BaseViewModel, IEdge<V, E>
        {
            private Vertex _src;
            private Vertex _dst;
            private E _data;
            private string _category;
            private string _color;
            private IList<Point2D> _draggablePoints;
            private bool _isHighlighted;
            private bool _isRevert;
            private bool _isSelected;
            private IList<Point2D> _points;
            private double _thickness;
            private double _weight = 1;
            private int _zIndex;

            public Edge()
            {
                Color = "Black";
                Thickness = 1;

                var points = new ObservableCollection<Point2D>();
                Points = points;
                points.CollectionChanged += pointsCollectionChanged;

                var draggablePoints = new ObservableCollection<Point2D>();
                DraggablePoints = draggablePoints;
            }

            public Edge(E data) : this()
            {
                Data = data;
            }

            public string StrokeBrush => Color;

            public double Weight
            {
                get => _weight;
                set => SetProperty(ref _weight, value);
            }

            public bool IsRevert
            {
                get => _isRevert;
                set => SetProperty(ref _isRevert, value);
            }

            public int DstPointIndex { get; set; }

            public IList<Point2D> PolygonPoints { get; set; }

            public IList<Point2D> Points
            {
                get => _points;
                set => SetProperty(ref _points, value);
            }

            public bool IsSelected
            {
                get => _isSelected;
                set
                {
                    SetProperty(ref _isSelected, value);
                    IsHighlighted = value;
                }
            }

            public bool IsHighlighted
            {
                get => _isHighlighted;
                set => SetProperty(ref _isHighlighted, value);
            }

            public IList<Point2D> DraggablePoints
            {
                get => _draggablePoints;
                set => SetProperty(ref _draggablePoints, value);
            }

            public double Length
            {
                get
                {
                    var vector = Src.CenterPoint() - Dst.CenterPoint();
                    return vector.Len * Weight;
                }
            }

            public string Color
            {
                get => _color;
                set
                {
                    SetProperty(ref _color, value);
                    RaisePropertyChanged(nameof(StrokeBrush));
                }
            }

            public double Thickness
            {
                get => _thickness;
                set => SetProperty(ref _thickness, value);
            }

            public int ZIndex
            {
                get => _zIndex;
                set => SetProperty(ref _zIndex, value);
            }

            public string Category
            {
                get => _category;
                set => SetProperty(ref _category, value);
            }

            object IElement.Data => Data;

            public E Data
            {
                get => _data;
                set => SetProperty(ref _data, value);
            }

            public Vertex Src
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

            public Vertex Dst
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

            IVertex IEdge.Src
            {
                get => Src;
                set => Src = (Vertex)value;
            }

            IVertex IEdge.Dst
            {
                get => Dst;
                set => Dst = (Vertex)value;
            }

            IVertex<V, E> IEdge<V, E>.Src
            {
                get => Src;
                set => Src = (Vertex)value;
            }

            IVertex<V, E> IEdge<V, E>.Dst
            {
                get => Dst;
                set => Dst = (Vertex)value;
            }

            internal event EventHandler<EdgeChangedEventArgs> EdgeChanged;

            public override string ToString() => $"{Src} -> {Dst}: {Data}";

            private void OnEdgeChanged(Vertex oldVertex, Vertex newVertex) =>
                EdgeChanged?.Invoke(this, new EdgeChangedEventArgs(oldVertex, newVertex));

            private void pointsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                Point2D p1, p2, p3, p4, p5;
                int i;
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        i = 2 * e.NewStartingIndex;
                        if (e.NewStartingIndex == 0)
                        {
                            // First point. Do nothing.
                            DraggablePoints.Insert(0, Points[0]);
                        }
                        else if (e.NewStartingIndex == Points.Count - 1)
                        {
                            // Last point.
                            p1 = DraggablePoints[i - 2];
                            p3 = Points[e.NewStartingIndex];
                            p2 = p1 + (p3 - p1) / 2;
                            DraggablePoints.Add(p2);
                            DraggablePoints.Add(p3);
                        }
                        else
                        {
                            // Middle point
                            p1 = DraggablePoints[i - 2];
                            p3 = DraggablePoints[i - 1];
                            p5 = DraggablePoints[i];
                            p2 = p1 + (p3 - p1) / 2;
                            p4 = p3 + (p5 - p3) / 2;
                            DraggablePoints.Insert(i - 1, p2);
                            DraggablePoints.Insert(i + 1, p4);
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        i = 2 * e.OldStartingIndex;
                        // Сначала фиксим порты, чтобы правильно вычислить координаты средней точки.
                        UpdateSrcPort();
                        UpdateDstPort();
                        p1 = DraggablePoints[i - 2];
                        p2 = DraggablePoints[i - 1];
                        p3 = DraggablePoints[i];
                        p4 = DraggablePoints[i + 1];
                        p5 = DraggablePoints[i + 2];
                        DraggablePoints.Remove(p2);
                        DraggablePoints.Remove(p4);
                        p3.X = (p1.X + p5.X) / 2;
                        p3.Y = (p1.Y + p5.Y) / 2;
                        RaisePointsChanged();
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        DraggablePoints.Clear();
                        foreach (var point in Points)
                            DraggablePoints.Add(point);
                        break;
                }
            }

            public void UpdateSrcPort()
            {
                if (Points.Count < 2)
                    return;
                if (IsRevert)
                {
                    var p1 = Points[Points.Count - 1];
                    var p2 = Points[Points.Count - 2];
                    var p = Src.GetShapePort(p2);
                    if (p1 != p)
                    {
                        p1.X = p.X;
                        p1.Y = p.Y;
                    }
                }
                else
                {
                    var p1 = Points[0];
                    var p2 = Points[1];
                    var p = Src.GetShapePort(p2);
                    if (p1 != p)
                    {
                        p1.X = p.X;
                        p1.Y = p.Y;
                    }
                }
            }

            public void UpdateDstPort()
            {
                if (Points.Count < 2)
                    return;
                if (IsRevert)
                {
                    var p1 = Points[0];
                    var p2 = Points[1];
                    var p = Dst.GetShapePort(p2);
                    if (p1 != p)
                    {
                        p1.X = p.X;
                        p1.Y = p.Y;
                    }
                }
                else
                {
                    var p1 = Points[Points.Count - 1];
                    var p2 = Points[Points.Count - 2];
                    var p = Dst.GetShapePort(p2);
                    if (p1 != p)
                    {
                        p1.X = p.X;
                        p1.Y = p.Y;
                    }
                }
            }

            public void RaisePointsChanged() => RaisePropertyChanged(nameof(Points));

            public void Revert()
            {
                if (IsRevert)
                    throw new Exception("Edge is already reverted.");
                var tmp = Src;
                Src = Dst;
                Dst = tmp;
                IsRevert = true;
            }

            public void UpdatePoint(Point2D data)
            {
                var i = Points.IndexOf(data);
                if (i == 1 && !IsRevert)
                    UpdateSrcPort();
                else if (i == 1 && IsRevert)
                    UpdateDstPort();
                if (i == Points.Count - 2 && !IsRevert)
                    UpdateDstPort();
                else if (i == Points.Count - 2 && IsRevert)
                    UpdateSrcPort();
                FixDraggablePoints(data);
            }

            public void FixDraggablePoints(Point2D data)
            {
                var j = DraggablePoints.IndexOf(data);
                var p3 = DraggablePoints[j];
                if (j > 1)
                {
                    var p1 = DraggablePoints[j - 2];
                    var p2 = DraggablePoints[j - 1];
                    p2.X = (p1.X + p3.X) / 2;
                    p2.Y = (p1.Y + p3.Y) / 2;
                }
                if (j < DraggablePoints.Count - 2)
                {
                    var p4 = DraggablePoints[j + 1];
                    var p5 = DraggablePoints[j + 2];
                    p4.X = (p3.X + p5.X) / 2;
                    p4.Y = (p3.Y + p5.Y) / 2;
                }
            }

            public IDisposable DeferRefresh() => new RefreshHelper(this);

            private class RefreshHelper : IDisposable
            {
                private readonly Edge _edge;

                internal RefreshHelper(Edge edge) => _edge = edge;

                #region IDisposable Members

                public void Dispose() => _edge.RaisePointsChanged();

                #endregion
            }
        }
    }
}