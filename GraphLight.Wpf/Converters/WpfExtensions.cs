using System.Windows;
using GraphLight.Geometry;

namespace GraphLight.Converters
{
    public static class WpfExtensions
    {
        public static Point ToWpf(this Point2D point) => new Point(point.X, point.Y);
        public static Size ToWpf(this Size2D size) => new Size(size.Width, size.Height);
        public static Rect ToWpf(this Rect2D rect) => new Rect(rect.Left, rect.Top, rect.Width, rect.Height);
        public static Point2D FromWpf(this Point point) => new Point2D(point.X, point.Y);
        public static Size2D FromWpf(this Size size) => new Size2D(size.Width, size.Height);
        public static Rect2D FromWpf(this Rect rect) => new Rect2D(rect.Left, rect.Top, rect.Width, rect.Height);
    }
}