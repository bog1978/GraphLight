using System;
using System.Linq;
using GraphLight.Geometry;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    public static class VertexExtensions
    {
        public static Point2D CenterPoint<V, E>(this IVertex<V, E> node)
            where V : IVertexDataLocation
        {
            return new Point2D(node.Data.Left + node.Data.Width / 2, node.Data.Top + node.Data.Height / 2);
        }

        public static Point2D CustomPoint<V, E>(this IVertex<V, E> node, double wK, double hK)
            where V : IVertexDataLocation
        {
            return new Point2D(node.Data.Left + node.Data.Width * wK, node.Data.Top + node.Data.Height * hK);
        }

        public static Point2D GetShapePort<V, E>(this IVertex<V, E> node, Point2D point)
            where V : IVertexDataLocation
        {
            var center = node.CenterPoint();
            var a = node.Data.Width / 2;
            var b = node.Data.Height / 2;
            var dx = point.X - center.X;
            var dy = point.Y - center.Y;

            if (dx == 0)
                return new Point2D(center.X, center.Y + Math.Sign(dy) * b);

            var c = dy / dx;
            var denom = Math.Sign(dx) * Math.Sqrt(a * a * c * c + b * b);
            var x = a * b / denom;
            var y = a * b * c / denom;
            return new Point2D(center.X + x, center.Y + y);
        }

        public static void Update<V, E>(this IVertex<V, E> vertex)
            where V : IVertexData
            where E : IEdgeData
        {
            foreach (var e in vertex.Edges)
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
