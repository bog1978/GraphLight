using System.Collections.Generic;
using System.Linq;

namespace GraphLight.Graph
{
    public class Vertex : BaseVertex<IVertexData, IEdgeData>, IVertex
    {
        private string _category;
        private bool _isHighlighted;
        private bool _isSelected;
        private int _zIndex;

        public Vertex() { }

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

        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

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