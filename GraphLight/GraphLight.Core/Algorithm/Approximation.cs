using System;
using System.Collections.Generic;
using GraphLight.Geometry;

namespace GraphLight.Algorithm
{
    public class Approximation
    {
        private double[] _resultx;
        private double[] _resulty;

        public Approximation()
        {
            Points = new List<Point2D>();
        }

        public Approximation(IEnumerable<Point2D> points)
        {
            Points = new List<Point2D>(points);
        }

        public IList<Point2D> Points { get; private set; }

        public void AddPoint(double x, double y)
        {
            Points.Add(new Point2D(x, y));
        }

        public void AddPoint(Point2D p)
        {
            Points.Add(p);
        }

        public IList<Point2D> GeneratePoints(int numPoints)
        {
            var points = new List<Point2D>();
            FillPoints(points, numPoints);
            return points;
        }

        public void FillPoints(IList<Point2D> points, int numPoints)
        {
            //_resultx = findControlPoints(PointCollection.Select(p => p.X).ToArray());
            //_resulty = findControlPoints(PointCollection.Select(p => p.Y).ToArray());
            _resultx = new double[Points.Count];
            _resulty = new double[Points.Count];
            for (var i = 0; i < Points.Count; i++)
            {
                _resultx[i] = Points[i].X;
                _resulty[i] = Points[i].Y;
            }

            var n = Points.Count - 2;
            var dt = 1.0 / (numPoints - 1);

            for (var i = 0; i < numPoints; i++)
            {
                var p = getPoint(i, dt, n);
                points.Add(p);
            }
        }

        private Point2D getPoint(int i, double dt, int n)
        {
            var t = i * dt;
            var x = b(0, n + 1, t) * Points[0].X + b(n + 1, n + 1, t) * Points[n + 1].X;
            var y = b(0, n + 1, t) * Points[0].Y + b(n + 1, n + 1, t) * Points[n + 1].Y;
            for (var j = 1; j <= n; j++)
            {
                x += b(j, n + 1, t) * _resultx[j - 1];
                y += b(j, n + 1, t) * _resulty[j - 1];
            }
            return new Point2D(x, y);
        }

        private static double[] findControlPoints(double[] points)
        {
            var n = points.Length - 2;
            var ax = new double[n, n];
            var bx = new double[n];
            var dt = 1.0 / (n + 1);
            for (var i = 1; i <= n; i++)
            {
                var t = dt * i;
                for (var j = 1; j <= n; j++)
                {
                    ax[i - 1, j - 1] = b(j, n + 1, t);
                }
                bx[i - 1] = points[i] - b(0, n + 1, t) * points[0] - b(n + 1, n + 1, t) * points[n + 1];
            }
            return gauss(ax, bx);
        }

        private static double[] gauss(double[,] a, double[] b)
        {
            var n = b.Length;
            var h = a.GetLength(0);
            var w = a.GetLength(1);
            if (w != n || h != n)
                throw new ArgumentOutOfRangeException();

            // Первый проход
            var processed = new List<int>();
            for (var i = 0; i < n; i++)
            {
                var max = double.MaxValue;
                var jMax = -1;
                for (var j = 0; j < n; j++)
                {
                    if (processed.Contains(j))
                        continue;
                    if (Math.Abs(1 - a[j, i]) < Math.Abs(1 - max))
                    {
                        max = a[j, i];
                        jMax = j;
                    }
                }
                processed.Add(jMax);
                for (var j = 0; j < n; j++)
                {
                    if (processed.Contains(j))
                        continue;
                    var z = a[j, i] / max;
                    for (var k = i; k < n; k++)
                        a[j, k] -= z * a[jMax, k];
                    b[j] -= z * b[jMax];
                }
            }

            // Второй проход
            var result = new double[n];
            for (var i = n - 1; i >= 0; i--)
            {
                var k = processed[i];
                result[i] = b[k];
                for (var j = i + 1; j < n; j++)
                    result[i] -= a[k, j] * result[j];
                result[i] /= a[k, i];
            }
            return result;
        }

        private static double b(int i, int n, double t)
        {
            return Math.Pow(t, i) * Math.Pow(1 - t, n - i) * factorial(n) / factorial(i) / factorial(n - i);
        }

        private static double factorial(int n)
        {
            if (n == 0)
                return 1;
            var result = 1.0;
            for (var i = 1; i <= n; i++)
                result *= i;
            return result;
        }
    }
}