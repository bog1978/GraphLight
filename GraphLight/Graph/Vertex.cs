using System.Collections.Generic;
using System.Linq;

namespace GraphLight.Graph
{
    public class Vertex : BaseVertex<IVertexData, IEdgeData>, IVertex
    {
        public Vertex() { }

        public Vertex(IVertexData data) : this() => Data = data;

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