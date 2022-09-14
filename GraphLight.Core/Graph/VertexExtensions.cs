using System.Linq;

namespace GraphLight.Graph
{
    public static class VertexExtensions
    {
        public static void Update(this IVertex vertex)
        {
            foreach (var e in vertex.Edges.Cast<IEdge>())
            {
                var pts = e.Data.Points;
                using (e.Data.DeferRefresh())
                {
                    if (pts.Count == 2 || e.Src == e.Dst)
                    {
                        e.UpdateSrcPort();
                        e.UpdateDstPort();
                    }
                    else if (e.Src == vertex)
                        e.UpdateSrcPort();
                    else
                        e.UpdateDstPort();
                    var first = e.Data.Points.First();
                    var last = e.Data.Points.Last();
                    e.Data.FixDraggablePoints(first);
                    e.Data.FixDraggablePoints(last);
                }
            }
        }
    }
}