using System.Collections.Generic;
using System.Linq;
using GraphLight.Collections;

namespace GraphLight.Graph
{
    public class Vertex : BaseVertex<IVertexData, IEdgeData>, IVertex
    {
        private string _category;
        private bool _isHighlighted;
        private bool _isSelected;
        private string _label;
        private string _shapeData;
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
            get => Data.Width;
            set
            {
                Data.Width = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Right));
            }
        }

        public double Height
        {
            get => Data.Height;
            set
            {
                Data.Height = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Bottom));
            }
        }

        public double Left
        {
            get => Data.Left;
            set
            {
                Data.Left = value;
                RaisePropertyChanged();
            }
        }

        public double Top
        {
            get => Data.Top;
            set
            {
                Data.Top = value;
                RaisePropertyChanged();
            }
        }

        public double Right => Data.Right;

        public double Bottom => Data.Bottom;

        public double CenterX
        {
            get => Data.CenterX;
            set
            {
                Data.CenterX = value;
                RaisePropertyChanged();
            }
        }

        public string ShapeData
        {
            get => _shapeData;
            set => SetProperty(ref _shapeData, value);
        }

        public int Rank
        {
            get => Data.Rank;
            set
            {
                Data.Rank = value;
                RaisePropertyChanged();
            }
        }

        public int Position
        {
            get => Data.Position;
            set
            {
                Data.Position = value;
                RaisePropertyChanged();
            }
        }

        public bool IsTmp
        {
            get => Data.IsTmp;
            set
            {
                Data.IsTmp = value;
                RaisePropertyChanged();
            }
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