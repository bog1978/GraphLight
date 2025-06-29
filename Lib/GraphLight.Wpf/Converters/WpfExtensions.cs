using System.Windows;
using GraphLight.Geometry;

namespace GraphLight.Converters
{
    public static class WpfExtensions
    {
        public static Point ToWpf(this Point2D point) => new(point.X, point.Y);
        public static Size ToWpf(this Size2D size) => new(size.Width, size.Height);
        public static Rect ToWpf(this Rect2D rect) => new(rect.Left, rect.Top, rect.Width, rect.Height);
        public static Point2D FromWpf(this Point point) => new(point.X, point.Y);
        public static Size2D FromWpf(this Size size) => new(size.Width, size.Height);
        public static Rect2D FromWpf(this Rect rect) => new(rect.Left, rect.Top, rect.Width, rect.Height);
    }
}