using System;
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
    }
}
