using System.Collections.Generic;
using System.Linq;
using GraphLight.Collections;

namespace GraphLight.Graph
{
    public class Vertex<TVertex, TEdge> : GraphModelBase<TVertex, TEdge>.Vertex, IVertex
    {
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

        public Vertex()
        {
            ShapeData = "M 0,1 A 1,1 0 1 0 2,1 A 1,1 0 1 0 0,1"; // έλλθορ
        }

        public Vertex(TVertex data) : this()
        {
            Data = data;
        }

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

        public string ShapeData
        {
            get { return _shapeData; }
            set { SetProperty(ref _shapeData, value, "ShapeData"); }
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

        int IBinaryHeapItem<double>.HeapIndex { get; set; }

        double IBinaryHeapItem<double>.HeapKey { get; set; }

        IEnumerable<IEdge> IVertex.Edges
        {
            get { return Edges; }
        }

        IEnumerable<IEdge> IVertex.InEdges
        {
            get { return InEdges; }
        }

        IEnumerable<IEdge> IVertex.OutEdges
        {
            get { return OutEdges; }
        }

        IEnumerable<IEdge> IVertex.SelfEdges
        {
            get { return SelfEdges; }
        }

        object IElement.Data
        {
            get { return Data; }
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
    }
}