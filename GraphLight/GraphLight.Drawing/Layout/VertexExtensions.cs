using System;
using GraphLight.Geometry;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    public static class VertexExtensions
    {
        public static Point2D CenterPoint<TVertex, TEdge>(this Vertex<TVertex, TEdge> node)
        {
            return new Point2D(node.Left + node.Width / 2, node.Top + node.Height / 2);
        }

        public static Point2D CustomPoint<TVertex, TEdge>(this Vertex<TVertex, TEdge> node, double wK, double hK)
        {
            return new Point2D(node.Left + node.Width * wK, node.Top + node.Height * hK);
        }

        public static Point2D GetShapePort<TVertex, TEdge>(this Vertex<TVertex, TEdge> node, Point2D point)
        {
            var center = node.CenterPoint();
            var a = node.Width / 2;
            var b = node.Height / 2;
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
