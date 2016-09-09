using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GraphLight.Collections;
using GraphLight.ViewModel;

namespace GraphLight.Graph
{
    public interface IVertex
    {
        int Rank { get; set; }
        int Position { get; set; }
        bool IsTmp { get; set; }
        string Category { get; set; }
        int ZIndex { get; set; }
        bool IsSelected { get; set; }
        bool IsHighlighted { get; set; }
        string Label { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        double Left { get; set; }
        double Top { get; set; }
        double Right { get; }
        double Bottom { get; }
        double CenterX { get; set; }
        double CenterY { get; }
        string ShapeData { get; set; }
        IEnumerable<IEdge> Edges { get; }
        IEnumerable<IEdge> InEdges { get; }
        IEnumerable<IEdge> OutEdges { get; }
        IEnumerable<IEdge> SelfEdges { get; }
        void Update();
    }

    public class Vertex<TVertex, TEdge> : BaseViewModel, IBinaryHeapItem<double>, IVertex
    {
        #region Private fields

        private readonly ICollection<Edge<TVertex, TEdge>> _edges;
        private readonly ICollection<Edge<TVertex, TEdge>> _inEdges;
        private readonly ICollection<Edge<TVertex, TEdge>> _outEdges;
        private readonly ICollection<Edge<TVertex, TEdge>> _selfEdges;
        private TVertex _data;
        private int _rank;
        private int _position;
        private bool _isTmp;
        private string _category;

        #endregion

        #region Constructors

        public Vertex(TVertex data)
        {
            _edges = new ObservableCollection<Edge<TVertex, TEdge>>();
            _inEdges = new ObservableCollection<Edge<TVertex, TEdge>>();
            _outEdges = new ObservableCollection<Edge<TVertex, TEdge>>();
            _selfEdges = new ObservableCollection<Edge<TVertex, TEdge>>();
            _data = data;
            ShapeData = "M 0,1 A 1,1 0 1 0 2,1 A 1,1 0 1 0 0,1"; // ������
        }

        #endregion

        #region IVertex<TVertex,TEdge> Members

        public TVertex Data
        {
            get { return _data; }
            set { SetProperty(ref _data, value, "Data"); }
        }

        public int Rank
        {
            get { return _rank; }
            set { SetProperty(ref _rank, value, "Rank"); }
        }

        public int Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value, "Position"); }
        }

        public bool IsTmp
        {
            get { return _isTmp; }
            set { SetProperty(ref _isTmp, value, "IsTmp"); }
        }

        public string Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value, "Category"); }
        }

        public IEnumerable<Edge<TVertex, TEdge>> Edges
        {
            get { return _edges; }
        }

        public IEnumerable<Edge<TVertex, TEdge>> InEdges
        {
            get { return _inEdges; }
        }

        public IEnumerable<Edge<TVertex, TEdge>> OutEdges
        {
            get { return _outEdges; }
        }

        public IEnumerable<Edge<TVertex, TEdge>> SelfEdges
        {
            get { return _selfEdges; }
        }

        IEnumerable<IEdge> IVertex.Edges
        {
            get { return _edges; }
        }

        IEnumerable<IEdge> IVertex.InEdges
        {
            get { return _inEdges; }
        }

        IEnumerable<IEdge> IVertex.OutEdges
        {
            get { return _outEdges; }
        }

        IEnumerable<IEdge> IVertex.SelfEdges
        {
            get { return _selfEdges; }
        }

        public void RegisterEdge(Edge<TVertex, TEdge> edge)
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

        public void UnregisterEdge(Edge<TVertex, TEdge> edge)
        {
            _edges.Remove(edge);
            _selfEdges.Remove(edge);
            _inEdges.Remove(edge);
            _outEdges.Remove(edge);
        }

        public int HeapIndex { get; set; }

        public double HeapKey { get; set; }

        #endregion

        private double _centerX;
        private double _height;
        private bool _isHighlighted;
        private bool _isSelected;
        private string _label;
        private double _left;
        private double _top;
        private double _width;
        private int _zIndex;
        private string _shapeData;

        public int ZIndex
        {
            get { return _zIndex; }
            set
            {
                _zIndex = value;
                RaisePropertyChanged("ZIndex");
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
                IsHighlighted = value;
            }
        }

        public bool IsHighlighted
        {
            get { return _isHighlighted; }
            set
            {
                _isHighlighted = value;
                RaisePropertyChanged("IsHighlighted");
            }
        }

        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                RaisePropertyChanged("Label");
            }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                RaisePropertyChanged("Width");
                RaisePropertyChanged("Right");
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                RaisePropertyChanged("Height");
                RaisePropertyChanged("Bottom");
            }
        }

        public double Left
        {
            get { return _left; }
            set
            {
                _left = value;
                RaisePropertyChanged("Left");
            }
        }

        public double Top
        {
            get { return _top; }
            set
            {
                _top = value;
                RaisePropertyChanged("Top");
            }
        }

        public double Right
        {
            get { return _left + _width; }
        }

        public double Bottom
        {
            get { return _top + _height; }
        }

        public double CenterX
        {
            get { return _centerX; }
            set
            {
                _centerX = value;
                RaisePropertyChanged("CenterX");
            }
        }

        public double CenterY
        {
            get { return Top + Height / 2; }
        }

        public string ShapeData
        {
            get { return _shapeData; }
            set { SetProperty(ref _shapeData, value, "ShapeData"); }
        }

        public override bool Equals(object obj)
        {
            var other = obj as Vertex<TVertex, TEdge>;
            if (other == null)
                return false;
            return Equals(Data, other.Data);
        }

        public override int GetHashCode()
        {
            return Data.GetHashCode();
        }

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
    }
}