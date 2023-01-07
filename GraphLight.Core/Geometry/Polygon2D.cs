using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace GraphLight.Geometry
{
    public class Polygon2D
    {
        #region Конструкторы

        public Polygon2D()
        {
            Points = new List<Point2D>();
        }

        public Polygon2D(IEnumerable<Point2D> points)
        {
            Points = new List<Point2D>(points);
        }

        #endregion

        #region Свойства

        public List<Point2D> Points { get; }

        public IEnumerable<Line2D> Edges
        {
            get
            {
                var cnt = Points.Count;
                for (var i = 0; i < cnt; i++)
                {
                    var j = (i + 1) % cnt;
                    var a = Points[i];
                    var b = Points[j];
                    yield return new Line2D(a, b);
                }
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Проверка выпуклости полигона.
        /// </summary>
        /// <returns>
        /// true - если полигон выпуклый, иначе - false.
        /// </returns>
        public bool IsConvex()
        {
            ClassifyPoints(out var convex, out var concave);

            var cnt = Points.Count;
            for (var i = 0; i < cnt; i++)
            {
                var p1 = Points[i];
                var p2 = Points[(i + 1) % cnt];
                var p3 = Points[(i + 2) % cnt];
                var z = (p2 - p1) ^ (p3 - p2);
                ((z > 0) ? convex : concave).Add(p2);
            }
            return concave.Count == 0;
        }

        public void ClassifyPoints(out IList<Point2D> convex, out IList<Point2D> concave)
        {
            convex = new List<Point2D>();
            concave = new List<Point2D>();

            var cnt = Points.Count;
            for (var i = 0; i < cnt; i++)
            {
                var p1 = Points[i];
                var p2 = Points[(i + 1) % cnt];
                var p3 = Points[(i + 2) % cnt];
                var z = (p2 - p1) ^ (p3 - p2);
                (z >= 0 ? convex : concave).Add(p2);
            }
        }

        public bool IsInner(Point2D p)
        {
            //return getCrossingNumber(p) == 1;
            return getWindingNumber(p) != 0;
        }

        public bool IsInner(Point2D a, Point2D b)
        {
            var crossTypes = new Dictionary<CrossType, int>
                {
                    {CrossType.Cross, 0},
                    {CrossType.Equal, 0},
                    {CrossType.Join, 0},
                    {CrossType.None, 0},
                    {CrossType.Overlap, 0},
                };
            var cnt = Points.Count;

            for (var i = 0; i < cnt; i++)
            {
                var p1 = Points[i];
                var p2 = Points[(i + 1) % cnt];
                var ct = Line2D.Cross(p1, p2, a, b, out _);
                crossTypes[ct]++;
            }

            if (crossTypes[CrossType.Overlap] > 0
                || crossTypes[CrossType.Equal] > 0
                //|| crossTypes[CrossType.Join] != 4
                || crossTypes[CrossType.Cross] > 0)
                return false;

            return IsInner(a + (b - a) / 2);
        }

        /// <summary>
        /// Разбиение полигона на два поменьше.
        /// </summary>
        /// <param name="n1">Индекс начальной точки нового полигона</param>
        /// <param name="n2">Индекс конечной точки нового полигона</param>
        /// <returns>Откушенный полигон.</returns>
        public Polygon2D Split(int n1, int n2)
        {
            if (n1 >= Points.Count || n2 >= Points.Count)
                throw new IndexOutOfRangeException();
            var count = Math.Abs(n1 - n2) + 1;
            var index = Math.Min(n1, n2);
            if (count < 3 || index < 0)
                throw new IndexOutOfRangeException();
            var range = Points.GetRange(index, count);
            Points.RemoveRange(index + 1, count - 2);
            return new Polygon2D(range);
        }

        public ICollection<Polygon2D> SplitConvex()
        {
            try
            {
                var convexPolygons = new List<Polygon2D>();
                var pol = new Polygon2D(Points);
                split(pol, convexPolygons);
                return convexPolygons;
            }
            catch (Exception)
            {
                Debug.WriteLine(ToOctaveCmd());
                Debug.WriteLine(ToTestCode());
                throw;
            }
        }

        private static void split(Polygon2D srcPol, ICollection<Polygon2D> splitLines)
        {
            srcPol.ClassifyPoints(out var convex, out var concave);

            if (concave.Count == 0)
            {
                splitLines.Add(srcPol);
                return;
            }

            Point2D dstPoint = null;
            var minDistance = double.MaxValue;
            var p = concave.First();
            var iStart = srcPol.Points.IndexOf(p);
            int iEnd;

            // Сначала ищем в вогнутых узлах.
            foreach (var q in concave)
            {
                iEnd = srcPol.Points.IndexOf(q);
                if (Math.Abs(iEnd - iStart) < 2)
                    continue;

                if (srcPol.IsInner(p, q))
                {
                    var curDistance = (p - q).Len;
                    if (curDistance < minDistance)
                    {
                        dstPoint = q;
                        minDistance = curDistance;
                    }
                }
            }

            // Если не находим в вогнутых, ищем в выпуклых.
            //if (dstPoint == null)
            foreach (var q in convex)
            {
                iEnd = srcPol.Points.IndexOf(q);
                if (Math.Abs(iEnd - iStart) < 2)
                    continue;

                if (srcPol.IsInner(p, q))
                {
                    var curDistance = (p - q).Len;
                    if (curDistance < minDistance)
                    {
                        dstPoint = q;
                        minDistance = curDistance;
                    }
                }
            }

            if (dstPoint == null)
                throw new Exception("Не удалось найти подходящую точку.");

            var dstPol = srcPol.Split(
                srcPol.Points.IndexOf(p),
                srcPol.Points.IndexOf(dstPoint));

            split(srcPol, splitLines);
            split(dstPol, splitLines);
        }

        #endregion

        #region Проверка принадлежности точки полигону

        /// <summary>
        /// Winding number test for a point in a polygon
        /// </summary>
        /// <param name="p">Point to test</param>
        /// <returns>The winding number (0 only if p is outside polygon)</returns>
        private int getWindingNumber(Point2D p)
        {
            var wn = 0;
            for (var i = 0; i < Points.Count; i++)
            {
                var p1 = Points[i];
                var p2 = Points[(i + 1) % Points.Count];
                if (p1.Y <= p.Y)
                {
                    if (p2.Y > p.Y && isLeft(p1, p2, p) > 0)
                        ++wn;
                }
                else if (p2.Y <= p.Y && isLeft(p1, p2, p) < 0)
                    --wn;
            }
            return wn;
        }

        /// <summary>
        /// Tests if a point is Left|On|Right of an infinite line.
        /// </summary>
        /// <param name="p">Point to test</param>
        /// <param name="a">First line point</param>
        /// <param name="b">Second line point</param>
        /// <returns>
        /// 1  - for p left of the line through a and b
        /// 0  - for p on the line
        /// -1 - for p right of the line
        /// </returns>
        private static int isLeft(Point2D p, Point2D a, Point2D b)
        {
            return Math.Sign((a.X - p.X) * (b.Y - p.Y) - (b.X - p.X) * (a.Y - p.Y));
        }

        #endregion

        public string ToOctaveCmd()
        {
            var points = Points.ToList();
            //points.Add(Points[0]);
            var xs = points.Select(x => x.X.ToString()).ToArray();
            var ys = points.Select(x => x.Y.ToString()).ToArray();
            var cmd = $@"plot([{string.Join(" ", xs).Replace(",", ".")}],[{string.Join(" ", ys).Replace(",", ".")}], ""-o"")";
            return cmd;
        }

        public string ToTestCode()
        {
            var strs = Points.Select(p => $"new Point2D({p.X},{p.Y}),").ToArray();
            return string.Join(" \r\n", strs);
        }

        public int[] Simplify(params int[] indexes)
        {
            // Удаляем дублирующиеся точки.
            for (var i = Points.Count - 2; i >= 0; i--)
            {
                var j = i + 1;
                if (indexes.Contains(j))
                    continue;
                var a = Points[i];
                var b = Points[j];
                if (a == b)
                {
                    Points.RemoveAt(j);
                    for (var idx = 0; idx < indexes.Length; idx++)
                    {
                        if (j <= indexes[idx])
                            indexes[idx]--;
                    }
                }
            }

            // Удаляем точки, лежащие на одной прямой.
            if (Points.Count > 3)
                for (var i = Points.Count - 3; i >= 0; i--)
                {
                    var j = i + 1;
                    if (indexes.Contains(j))
                        continue;
                    var k = i + 2;
                    var a = Points[i];
                    var b = Points[j];
                    var c = Points[k];
                    if (((b - a) ^ (c - b)) == 0)
                    {
                        Points.RemoveAt(j);
                        for (var idx = 0; idx < indexes.Length; idx++)
                        {
                            if (j <= indexes[idx])
                                indexes[idx]--;
                        }
                    }
                }
            return indexes;
        }
    }
}