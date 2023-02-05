using System;
using GraphLight.Geometry;
using GraphLight.Model;
using static GraphLight.Model.VertexShape;

namespace GraphLight.Algorithm
{
    internal static class VertexExtensions
    {
        internal static Point2D CenterPoint<V>(this IVertex<V> node)
            where V : IVertexDataLocation =>
            node.Data.Rect.CustomPoint(0.5, 0.5);

        internal static Point2D CustomPoint<V>(this IVertex<V> node, double w, double h)
            where V : IVertexDataLocation =>
            node.Data.Rect.CustomPoint(w, h);

        internal static Point2D GetShapePort<V>(this IVertex<V> node, Point2D point)
            where V : IVertexData =>
            node.Data.Shape switch
            {
                None => GetRectPort(node, point),
                Ellipse => GetEllipsePort(node, point),
                Rectangle => GetRectPort(node, point),
                Diamond => GetDiamondPort(node, point),
                _ => node.CenterPoint()
            };

        private static Point2D GetEllipsePort<V>(IVertex<V> node, Point2D point) where V : IVertexData
        {
            var d = node.Data;

            var center = node.CenterPoint();
            var a = d.Rect.Width / 2;
            var b = d.Rect.Height / 2;
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

        private static Point2D GetRectPort<V>(IVertex<V> node, Point2D point) where V : IVertexData =>
            GetPoligonPort(
                point,
                node.CenterPoint(),
                node.CustomPoint(0, 0),
                node.CustomPoint(1, 0),
                node.CustomPoint(1, 1),
                node.CustomPoint(0, 1));

        private static Point2D GetDiamondPort<V>(IVertex<V> node, Point2D point) where V : IVertexData =>
            GetPoligonPort(
                point,
                node.CenterPoint(),
                node.CustomPoint(0.5, 0),
                node.CustomPoint(1, 0.5),
                node.CustomPoint(0.5, 1), node.CustomPoint(0, 0.5));

        private static Point2D GetPoligonPort(Point2D outerPoint, Point2D innerPoint, params Point2D[] polygonPoints)
        {
            var polygon = new Polygon2D(polygonPoints);
            foreach (var line in polygon.Edges)
            {
                if (line.Cross(innerPoint, outerPoint, out var crossPoint) != CrossType.Cross)
                    continue;
                return crossPoint ?? innerPoint;
            }
            return innerPoint;
        }
    }
}
