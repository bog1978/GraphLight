using System;
using System.Diagnostics;

namespace GraphLight.Geometry
{
    [DebuggerDisplay("x={X}, y={Y}")]
    public class Point2D : IEquatable<Point2D>
    {
        public Point2D()
        {
        }

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        #region Свойства

        public static readonly Point2D Empty = new Point2D(double.NaN, double.NaN);

        public double X { get; set; }

        public double Y { get; set; }

        public bool IsEmpty => double.IsNaN(X) || double.IsNaN(Y);

        #endregion

        #region Операторы

        public static Vector2D operator -(Point2D p1, Point2D p2) => new Vector2D(p2, p1);

        public static Point2D operator +(Point2D p, Vector2D v) => new Point2D(p.X + v.X, p.Y + v.Y);

        public static Point2D operator -(Point2D p, Vector2D v) => new Point2D(p.X - v.X, p.Y - v.Y);

        public static bool operator ==(Point2D a, Point2D b)
        {
            var aIsNull = ReferenceEquals(a, null);
            var bIsNull = ReferenceEquals(b, null);
            return aIsNull ? bIsNull : a.Equals(b);
        }

        public static bool operator !=(Point2D p1, Point2D p2) => !(p1 == p2);

        #endregion

        #region Равенство

        public bool Equals(Point2D other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.X == X && other.Y == Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Point2D)) return false;
            return Equals((Point2D)obj);
        }

        public override int GetHashCode() => (X.GetHashCode() * 397) ^ Y.GetHashCode();

        #endregion

        public override string ToString() => $"x={X}, y={Y}";
    }
}