using System;
using System.Diagnostics;

namespace GraphLight.Geometry
{
    [DebuggerDisplay("P1=({P1.X}, {P1.Y}), P2=({P2.X}, {P2.Y}), L={Len}")]
    public class Line2D : IEquatable<Line2D>
    {
        #region Конструкторы

        public Line2D() { }

        public Line2D(Point2D p1, Point2D p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public Line2D(double x1, double y1, double x2, double y2)
        {
            P1 = new Point2D(x1, y1);
            P2 = new Point2D(x2, y2);
        }

        #endregion

        #region Свойства

        public static readonly Line2D Empty = new Line2D(Point2D.Empty, Point2D.Empty);
        private const int ROUND_DIGITS = 10;
        public Point2D P1 { get; set; }
        public Point2D P2 { get; set; }

        public bool IsEmpty => P1.IsEmpty || P2.IsEmpty;

        public double Len => (P2 - P1).Len;

        #endregion

        #region Методы

        public bool Overlap(Point2D q1, Point2D q2) => Overlap(P1, P2, q1, q2);

        public bool Overlap(Line2D line) => Overlap(P1, P2, line.P1, line.P2);

        public static bool Overlap(Point2D p1, Point2D p2, Point2D q1, Point2D q2)
        {
            var result = Classify(p1, q1, q2)
                | Classify(p2, q1, q2)
                    | Classify(q1, p1, p2)
                        | Classify(q2, p1, p2);
            return (result & (PointLocation.Left | PointLocation.Right | PointLocation.Between))
                == PointLocation.Between;
        }

        public PointLocation Classify(Point2D p) => Classify(p, P1, P2);

        public static PointLocation Classify(Point2D p, Point2D p1, Point2D p2)
        {
            var a = p2 - p1;
            var b = p - p1;
            var sa = a.X * b.Y - b.X * a.Y;
            if (sa > 0.0)
                return PointLocation.Left;
            if (sa < 0.0)
                return PointLocation.Right;
            if (a.X * b.X < 0.0 || a.Y * b.Y < 0.0)
                return PointLocation.Behind;
            if (a.Len < b.Len)
                return PointLocation.Beyond;
            if (p1 == p)
                return PointLocation.Origin;
            if (p2 == p)
                return PointLocation.Destination;
            return PointLocation.Between;
        }

        public CrossType Cross(Line2D line, out Point2D? cross) => Cross(P1, P2, line.P1, line.P2, out cross);

        public CrossType Cross(Point2D q1, Point2D q2, out Point2D? cross) => Cross(P1, P2, q1, q2, out cross);

        public static CrossType Cross(Point2D p1, Point2D p2, Point2D q1, Point2D q2, out Point2D? cross)
        {
            cross = null;
            if (p1 == q1 && p2 == q2 || p1 == q2 && p2 == q1)
                return CrossType.Equal;

            if (Overlap(p1, p2, q1, q2))
                return CrossType.Overlap;

            var a1 = p2.Y - p1.Y;
            var b1 = p2.X - p1.X;
            var c1 = p1.X * p2.Y - p2.X * p1.Y;

            var a2 = q2.Y - q1.Y;
            var b2 = q2.X - q1.X;
            var c2 = q1.X * q2.Y - q2.X * q1.Y;

            var znam = Math.Round(a1 * b2 - a2 * b1, ROUND_DIGITS);
            if (znam == 0)
                return CrossType.None;
            
            var t = Math.Round(+(a2 * p1.X - b2 * p1.Y - c2) / znam, ROUND_DIGITS);
            var u = Math.Round(-(a1 * q1.X - b1 * q1.Y - c1) / znam, ROUND_DIGITS);

            if (t == 0 || t == 1 && u == 0 || u == 1)
                return CrossType.Join;
            if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
            {
                var x = (b2 * c1 - b1 * c2) / znam;
                var y = (a2 * c1 - a1 * c2) / znam;
                cross = new Point2D(x, y);
                return CrossType.Cross;
            }

            return CrossType.None;
        }

        /// <summary>
        /// Returns point laying on beam P1P2.
        /// GetPoint(0) = P1
        /// GetPoint(1) = P2
        /// GetPoint(0.5) = P1 + (P2-P1)/2
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public Point2D GetPoint(double k) => new Point2D(P1.X + k * (P2.X - P1.X), P1.Y + k * (P2.Y - P1.Y));

        #endregion

        #region Операторы

        public static bool operator ==(Line2D a, Line2D b)
        {
            var aIsNull = ReferenceEquals(a, null);
            var bIsNull = ReferenceEquals(b, null);
            return aIsNull ? bIsNull : a.Equals(b);
        }

        public static bool operator !=(Line2D a, Line2D b) => !(a == b);

        #endregion

        #region Равенство

        public bool Equals(Line2D other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            var eq1 = Equals(other.P1, P1) && Equals(other.P2, P2);
            var eq2 = Equals(other.P1, P2) && Equals(other.P2, P1);
            return eq1 || eq2;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Line2D)) return false;
            return Equals((Line2D)obj);
        }

        public override int GetHashCode()
        {
            return ((P1 != null ? P1.GetHashCode() : 0) * 397)
                ^ ((P2 != null ? P2.GetHashCode() : 0) * 397);
        }

        #endregion
    }
}