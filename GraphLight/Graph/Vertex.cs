using System.Collections.Generic;
using System.Linq;
using GraphLight.Collections;

namespace GraphLight.Graph
{
    public class Vertex : BaseVertex<IVertexData, IEdgeData>, IVertex
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

        public Vertex() => ShapeData = "M 0,1 A 1,1 0 1 0 2,1 A 1,1 0 1 0 0,1";

        public Vertex(IVertexData data) : this() => Data = data;

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

        int IBinaryHeapItem<double>.HeapIndex { get; set; }

        double IBinaryHeapItem<double>.HeapKey { get; set; }

        IEnumerable<IEdge> IVertex.Edges => Edges.Cast<IEdge>();

        IEnumerable<IEdge> IVertex.InEdges => InEdges.Cast<IEdge>();

        IEnumerable<IEdge> IVertex.OutEdges => OutEdges.Cast<IEdge>();

        IEnumerable<IEdge> IVertex.SelfEdges => SelfEdges.Cast<IEdge>();

        public void Update()
        {
            foreach (var e in Edges.Cast<IEdge>())
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
            return obj is Vertex other && Equals(Data, other.Data);
        }

        public override int GetHashCode() => Data.GetHashCode();
    }
}