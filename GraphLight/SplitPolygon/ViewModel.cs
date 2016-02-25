using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight;
using GraphLight.Geometry;
using GraphLight.Graph;
using GraphLight.Algorithm;
using GraphLight.Collections;

namespace SplitPolygon
{
    public class ViewModel
    {
        public int SplitCount;

        public ViewModel()
        {
            Polygon = new Polygon2D();
        }

        public Polygon2D Polygon { get; private set; }

        public void CalcConvexTypes()
        {
            IList<Point2D> convex;
            IList<Point2D> concave;
            Polygon.ClassifyPoints(out convex, out concave);
        }

        public void Simplify()
        {
            var points = Polygon.Points;
            var cnt = points.Count;
            var toTemove = new List<Point2D>();
            for (var i = 0; i < cnt; i++)
            {
                var p1 = points[i];
                var p2 = points[(i + 1) % cnt];
                var p3 = points[(i + 2) % cnt];
                if (p1.Sgn < 0 && p2.Sgn > 0 && p3.Sgn < 0)
                    toTemove.Add(p2);
            }
            foreach (var p in toTemove)
                points.Remove(p);
        }

        public List<Point2D> Split(out Point2D r)
        {
            var points = Polygon.Points;
            var tmp = new List<Point2D>();
            r = new Point2D();
            var i = 0;
            foreach (var p in points)
            {
                if (p.Sgn < 0)
                {
                    if (i < SplitCount)
                    {
                        i++;
                        continue;
                    }
                    foreach (var q in points)
                    {
                        if (Polygon.IsInner(p, q))
                            tmp.Add(q);
                    }
                    r = p;
                    break;
                }
            }
            SplitCount++;
            return tmp;
        }

        public int ConvexIndex = 1;
        public List<Point2D> GetPiecewiseCurve()
        {
            var start = Polygon.Points[0];
            var end = Polygon.Points[Math.Min(Polygon.Points.Count - 1, ConvexIndex++)];
            var startPols = new List<Polygon2D>();
            var endPols = new List<Polygon2D>();

            // Разбиваем на выпуклые многоугольники.
            var convexPolygons = Polygon.SplitConvex();

            // Словарь: ребро -> список многоугольников, его содержащих.  
            var linePolMap = new Dictionary<Line2D, List<Polygon2D>>();
            foreach (var polygon in convexPolygons)
            {
                var item = polygon;
                polygon.Points.Iter((a, b) =>
                    {
                        var l = new Line2D(a, b);
                        List<Polygon2D> pols;
                        if (!linePolMap.TryGetValue(l, out pols))
                        {
                            pols = new List<Polygon2D>();
                            linePolMap[l] = pols;
                        }
                        pols.Add(item);
                        if (a == start)
                            startPols.Add(item);
                        if (a == end)
                            endPols.Add(item);
                    });
            }

            // Создаем граф полигонов.
            var polygonGraph = new PolygonGraph();
            foreach (var item in linePolMap)
            {
                switch (item.Value.Count)
                {
                    case 1: // Внешнее ребро
                        break;
                    case 2: // Смежное ребро.
                        var edge = polygonGraph.AddEdge(item.Value[0], item.Value[1]);
                        edge.Data = item.Key;
                        break;
                    default: // Этого быть не должно.
                        throw new InvalidProgramException("Ошибка в алгоритме.");
                }
            }

            // Создаем граф точек.
            var pointGraph = new UndirectedGraph<Point2D, object>();
            foreach (var node in polygonGraph.Nodes)
            {
                var midLines = combinate(node.Edges.Select(x => x.Data));
                midLines.Iter(x => pointGraph.AddEdge(x.P1, x.P2));
            }

            // Добавляем в граф начальную точку.
            // Причем если в некотором многоугольнике содержится и конечная точка,
            // то добавляем ребро start -> end
            foreach (var startPol in startPols)
                startPol.Points.Iter((a, b) =>
                    pointGraph.AddEdge(start, b == end ? end : a + (b - a) / 2));

            // Добавляем в граф конечную точку.
            foreach (var endPol in endPols)
                endPol.Points.Iter((a, b) =>
                    pointGraph.AddEdge(end, a + (b - a) / 2));

            // Инициализируем вес ребер
            pointGraph.Edges.Iter(x => x.Weight = (x.Dst.Id - x.Src.Id).Len);

            var shortestPath = pointGraph
                .GetShortestWayNodes(start, end)
                .Select(x => x.Id)
                .ToList();

            for (var i = shortestPath.Count - 2; i > 0; i--)
                if (Polygon.IsInner(shortestPath[i - 1], shortestPath[i + 1]))
                    shortestPath.RemoveAt(i);

            return prepare(shortestPath);
        }

        private static List<Point2D> prepare(List<Point2D> points)
        {
            var LEN = 10;
            int i = 0;

            while (i < points.Count - 1)
            {
                var p1 = points[i];
                var p2 = points[i + 1];
                var v = p2 - p1;
                if (v.Len > LEN)
                {
                    var q = p2 - v / 2;
                    points.Insert(i + 1, q);
                }
                i++;
            }

            return points;
        }

        private static List<Line2D> combinate_(IEnumerable<Line2D> lines)
        {
            var pts =
                from l in lines
                let v = l.P2 - l.P1
                let p1 = l.P1 + v / 4
                let p2 = l.P1 + v / 2
                let p3 = l.P1 + v * 3 / 4
                select new { p1, p2, p3 };

            var midPoints = pts.ToArray();

            var cl = new List<Line2D>();

            for (var i = 0; i < midPoints.Length; i++)
            {
                for (var j = i + 1; j < midPoints.Length; j++)
                {
                    var grp1 = midPoints[i];
                    var grp2 = midPoints[j];
                    cl.Add(new Line2D(grp1.p1, grp2.p1));
                    cl.Add(new Line2D(grp1.p1, grp2.p2));
                    cl.Add(new Line2D(grp1.p1, grp2.p3));
                    cl.Add(new Line2D(grp1.p2, grp2.p1));
                    cl.Add(new Line2D(grp1.p2, grp2.p2));
                    cl.Add(new Line2D(grp1.p2, grp2.p3));
                    cl.Add(new Line2D(grp1.p3, grp2.p1));
                    cl.Add(new Line2D(grp1.p3, grp2.p2));
                    cl.Add(new Line2D(grp1.p3, grp2.p3));
                }
            }

            return cl;
        }

        private static List<Line2D> combinate(IEnumerable<Line2D> lines)
        {
            var midPoints = lines
                .Select(x => x.P1 + (x.P2 - x.P1) / 2)
                .ToArray();
            var cl = new List<Line2D>();

            for (var i = 0; i < midPoints.Length; i++)
            {
                for (var j = i + 1; j < midPoints.Length; j++)
                {
                    var p1 = midPoints[i];
                    var p2 = midPoints[j];
                    cl.Add(new Line2D(p1, p2));
                }
            }

            return cl;
        }
    }

    public class PolygonGraph : UndirectedGraph<Polygon2D, Line2D> { }

    public class Pair<T>
    {
        public Pair(T a, T b)
        {
            A = a;
            B = b;
        }

        public T A { get; private set; }
        public T B { get; private set; }
    }
}