using System.Windows;
using GraphLight.Geometry;

namespace GraphLight
{
    public static class Point2DExtensions
    {
        public static Point ToPoint(this Point2D point2D)
        {
            return new Point(point2D.X, point2D.Y);
        }
    }
}
