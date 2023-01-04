using System;
using GraphLight.Geometry;
using GraphLight.Graph;
using static GraphLight.Graph.VertexShape;

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
            where V : IVertexData =>
            node.Data.Shape switch
            {
                None => GetRectPort(node, point),
                Ellipse => GetEllipsePort(node, point),
                Rectangle => GetRectPort(node, point),
                Diamond => GetDiamondPort(node, point),
                _ => node.CenterPoint()
            };

        private static Point2D GetEllipsePort<V, E>(IVertex<V, E> node, Point2D point) where V : IVertexData
        {
            var d = node.Data;

            var center = node.CenterPoint();
            var a = d.Width / 2;
            var b = d.Height / 2;
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

        private static Point2D GetRectPort<V, E>(IVertex<V, E> node, Point2D point) where V : IVertexData =>
            GetPoligonPort(
                point,
                node.CenterPoint(),
                node.CustomPoint(0, 0),
                node.CustomPoint(1, 0),
                node.CustomPoint(1, 1),
                node.CustomPoint(0, 1));

        private static Point2D GetDiamondPort<V, E>(IVertex<V, E> node, Point2D point) where V : IVertexData =>
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
                if (line.Cross(innerPoint, outerPoint) != CrossType.Cross)
                    continue;
                var crossPoint = Line2D.CrossPoint(line.P1, line.P2, innerPoint, outerPoint);
                return crossPoint;
            }
            return innerPoint;
        }
    }
}
