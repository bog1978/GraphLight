using System;
using System.Xml.Linq;

namespace GraphLight.Geometry
{
    public class Rect2D
    {
        public Rect2D() { }

        public Rect2D(Point2D location, Size2D size)
        {
            Left = location.X;
            Top = location.Y;
            Width = size.Width;
            Height = size.Height;
        }

        public Rect2D(double left, double top, double width, double height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public double Width { get; set; }

        public double Height { get; set; }

        public double Left { get; set; }

        public double Top { get; set; }

        public double Right => Left + Width;

        public double Bottom => Top + Height;

        public void SetSize(Size2D size)
        {
            Width = size.Width;
            Height = size.Height;
        }

        public Point2D CustomPoint(double w, double h)
        {
            if (h < 0 || h > 1)
                throw new ArgumentOutOfRangeException(nameof(h), "Expected value: [0,1].");
            if (w < 0 || w > 1)
                throw new ArgumentOutOfRangeException(nameof(w), "Expected value: [0,1].");
            return new Point2D(Left + Width * w, Top + Height * h);
        }
    }
}
