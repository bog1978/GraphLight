using System;
using System.Diagnostics;

namespace GraphLight.Geometry
{
    [DebuggerDisplay("X={X}, Y={Y}, L={Len}")]
    public class Vector2D
    {
        #region Конструкторы

        public Vector2D()
        {
        }

        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector2D(Point2D p1, Point2D p2)
        {
            X = p2.X - p1.X;
            Y = p2.Y - p1.Y;
        }

        #endregion

        #region Свойства

        public double X { get; set; }

        public double Y { get; set; }

        public double Len => Math.Sqrt(X * X + Y * Y);

        #endregion

        #region Операторы

        /// <summary>
        /// Векторное произведение.
        /// </summary>
        /// <param name="a">Первый вектор</param>
        /// <param name="b">Второй вектор</param>
        /// <returns>Z-компоненту векторного произведения.</returns>
        public static double operator ^(Vector2D a, Vector2D b) => a.X * b.Y - a.Y * b.X;

        /// <summary>
        /// Скалярное произведение.
        /// </summary>
        /// <param name="a">Первый вектор</param>
        /// <param name="b">Второй вектор</param>
        /// <returns>Скалярное произведение.</returns>
        public static double operator *(Vector2D a, Vector2D b) => a.X * b.X + a.Y * b.Y;

        public static Vector2D operator +(Vector2D a, Vector2D b) => new Vector2D(a.X + b.X, a.Y + b.Y);

        public static Vector2D operator -(Vector2D a, Vector2D b) => new Vector2D(a.X - b.X, a.Y - b.Y);

        public static Vector2D operator *(Vector2D v, double k) => new Vector2D(v.X * k, v.Y * k);

        public static Vector2D operator /(Vector2D v, double k) => new Vector2D(v.X / k, v.Y / k);

        public static bool operator ==(Vector2D p1, Vector2D p2)
        {
            if (ReferenceEquals(p1, p2))
                return true;
            if (ReferenceEquals(p1, null) || ReferenceEquals(p2, null))
                return false;
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(Vector2D p1, Vector2D p2) => !(p1 == p2);

        #endregion
    }
}