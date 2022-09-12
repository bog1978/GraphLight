using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Collections;
using GraphLight.Geometry;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    partial class GraphVizLayout<V, E>
    {
        protected virtual void RouteEdges()
        {
            calculatePolygons();
            removeTmpNodes();
            route();
        }

        #region Calculating polygons

        private IDictionary<int, List<IVertex>> _rankMap;

        private void calculatePolygons()
        {
            var g = (IGraph)Graph;

            _rankMap = g.GetRankMap();
            g.Edges.Iter(calculate);
        }

        private void calculate(IEdge edge)
        {
            IList<IEdge> edges = new List<IEdge>();
            for (var currEdge = edge; currEdge != null;
                currEdge = currEdge.Dst.OutEdges.FirstOrDefault(x => x.Src.IsTmp))
                edges.Add(currEdge);

            var points = new List<Point2D>();

            foreach (var e in edges)
            {
                calcSrcRightNode(e, points);
                calcDstRightNode(e, points);
            }

            //edge.SrcPointIndex = 0;
            edge.DstPointIndex = points.Count - 1;

            foreach (var e in edges.Reverse())
            {
                calcDstLeftNode(e, points);
                calcSrcLeftNode(e, points);
            }

            var edgePolygon = new Polygon2D(points);
            edge.DstPointIndex = edgePolygon.Simplify(edge.DstPointIndex)[0];
            edge.PolygonPoints = edgePolygon.Points;
        }

        #region Вычисление полигона

        private void calcSrcRightNode(IEdge e, ICollection<Point2D> points)
        {
            var rank = _rankMap[e.Src.Rank];
            var index = rank.IndexOf(e.Src);
            IList<IVertex> nodes = new List<IVertex>();

            for (var i = index; i < rank.Count; i++)
            {
                var node = rank[i];
                if (i == index && node.IsTmp && i < rank.Count - 1)
                {
                    nodes.Add(rank[i + 1]);
                    break;
                }
                var btw = between(node.CenterX, e.Src.CenterX, e.Dst.CenterX);
                if (!btw)
                    break;
                nodes.Add(node);
            }

            var rightNode = nodes.Last();
            foreach (var n in nodes)
            {
                if (n.IsTmp)
                    points.Add(new Point2D(n.Right, e.Src.Bottom));
                else if (n == e.Src)
                {
                    points.Add(n.CenterPoint());
                    if (e.Src.CenterX < e.Dst.CenterX)
                        points.Add(n.CustomPoint(1, 0.5));
                    else
                        points.Add(n.CustomPoint(1, 1));
                    if (n == rightNode && n.Right < e.Dst.CenterX)
                        points.Add(new Point2D(e.Dst.CenterX, n.Bottom));
                }
                else if (n == rightNode)
                {
                    points.Add(n.CustomPoint(0, 1));
                    if (n.Right < e.Dst.CenterX)
                        points.Add(new Point2D(e.Dst.CenterX, n.Bottom));
                }
                else
                {
                    points.Add(n.CustomPoint(0, 1));
                    points.Add(n.CustomPoint(1, 1));
                }
            }
        }

        private void calcDstRightNode(IEdge e, ICollection<Point2D> points)
        {
            var rank = _rankMap[e.Dst.Rank];
            var index = rank.IndexOf(e.Dst);
            IList<IVertex> nodes = new List<IVertex>();

            for (var i = index; i < rank.Count; i++)
            {
                var node = rank[i];
                if (i == index && node.IsTmp && i < rank.Count - 1)
                {
                    nodes.Add(rank[i + 1]);
                    break;
                }
                var btw = between(node.CenterX, e.Src.CenterX, e.Dst.CenterX);
                if (!btw)
                    break;
                nodes.Add(node);
            }

            var rightNode = nodes.Last();
            foreach (var n in nodes.Reverse())
            {
                if (n.IsTmp)
                    points.Add(new Point2D(n.Right, e.Dst.Top));
                else if (n == e.Dst)
                {
                    if (n == rightNode && n.Right < e.Src.CenterX)
                        points.Add(new Point2D(e.Src.CenterX, n.Top));
                    if (e.Src.CenterX > e.Dst.CenterX)
                        points.Add(n.CustomPoint(1, 0.5));
                    else
                        points.Add(n.CustomPoint(1, 0));
                    points.Add(n.CenterPoint());
                }
                else if (n == rightNode)
                {
                    if (n.Right < e.Src.CenterX)
                        points.Add(new Point2D(e.Src.CenterX, n.Top));
                    points.Add(n.CustomPoint(0, 0));
                }
                else
                {
                    points.Add(n.CustomPoint(1, 0));
                    points.Add(n.CustomPoint(0, 0));
                }
            }
        }

        private void calcDstLeftNode(IEdge e, ICollection<Point2D> points)
        {
            var rank = _rankMap[e.Dst.Rank];
            var index = rank.IndexOf(e.Dst);
            IList<IVertex> nodes = new List<IVertex>();

            for (var i = index; i >= 0; i--)
            {
                var node = rank[i];
                if (i == index && node.IsTmp && i > 0)
                {
                    nodes.Add(rank[i - 1]);
                    break;
                }
                var btw = between(node.CenterX, e.Src.CenterX, e.Dst.CenterX);
                if (!btw)
                    break;
                nodes.Add(node);
            }

            var leftNode = nodes.Last();
            foreach (var n in nodes)
            {
                if (n.IsTmp)
                    points.Add(new Point2D(n.Left, e.Dst.Top));
                else if (n == e.Dst)
                {
                    points.Add(n.CenterPoint());
                    if (e.Src.CenterX < e.Dst.CenterX)
                        points.Add(n.CustomPoint(0, 0.5));
                    else
                        points.Add(n.CustomPoint(0, 0));
                    if (n == leftNode && n.Left > e.Src.CenterX)
                        points.Add(new Point2D(e.Src.CenterX, n.Top));
                }
                else if (n == leftNode)
                {
                    points.Add(n.CustomPoint(1, 0));
                    if (n.Left > e.Src.CenterX)
                        points.Add(new Point2D(e.Src.CenterX, e.Dst.Top));
                }
                else
                {
                    points.Add(n.CustomPoint(1, 0));
                    points.Add(n.CustomPoint(0, 0));
                }
            }
        }

        private void calcSrcLeftNode(IEdge e, ICollection<Point2D> points)
        {
            var rank = _rankMap[e.Src.Rank];
            var index = rank.IndexOf(e.Src);
            IList<IVertex> nodes = new List<IVertex>();

            for (var i = index; i >= 0; i--)
            {
                var node = rank[i];
                if (i == index && node.IsTmp && i > 0)
                {
                    nodes.Add(rank[i - 1]);
                    break;
                }
                var btw = between(node.CenterX, e.Src.CenterX, e.Dst.CenterX);
                if (!btw)
                    break;
                nodes.Add(node);
            }

            var leftNode = nodes.Last();
            foreach (var n in nodes.Reverse())
            {
                if (n.IsTmp)
                    points.Add(new Point2D(n.Left, e.Src.Bottom));
                else if (n == e.Src)
                {
                    if (n == leftNode && n.Left > e.Dst.CenterX)
                        points.Add(new Point2D(e.Dst.CenterX, n.Bottom));
                    if (e.Src.CenterX > e.Dst.CenterX)
                        points.Add(n.CustomPoint(0, 0.5));
                    else
                        points.Add(n.CustomPoint(0, 1));
                    points.Add(n.CenterPoint());
                }
                else if (n == leftNode)
                {
                    if (n.Left > e.Dst.CenterX)
                        points.Add(new Point2D(e.Dst.CenterX, n.Bottom));
                    points.Add(n.CustomPoint(1, 1));
                }
                else
                {
                    points.Add(n.CustomPoint(0, 1));
                    points.Add(n.CustomPoint(1, 1));
                }
            }
        }

        #endregion

        private static bool between(double x, double a, double b)
        {
            return Math.Min(a, b) <= x && x <= Math.Max(a, b);
        }

        #endregion

        #region Edge routing

        private void route()
        {
            var g = (IGraph)Graph;

            foreach (var edge in g.Edges)
            {
                List<Point2D> points = null;
                try
                {
                    points = edge.Src != edge.Dst ? piecewiseLinearCurve(edge) : loopCurve(edge);
                }
                catch (Exception ex)
                {
                    dump(edge);
                    points = new List<Point2D>
                    {
                        edge.Src.CenterPoint(),
                        edge.Dst.CenterPoint(),
                    };
                }
                finally
                {
                    if (points.Count < 2)
                    {
                        dump(edge);
                    }
                    // Replace center points of source and destination nodes by port points.
                    var srcPort = edge.Src.GetShapePort(points[1]);
                    var dstPort = edge.Dst.GetShapePort(points[points.Count - 2]);
                    points[0] = srcPort;
                    points[points.Count - 1] = dstPort;

                    if (edge.IsRevert)
                        points.Reverse();

                    edge.Points.Clear();
                    foreach (var p in points)
                        edge.Points.Add(p);
                }
            }
        }

        #region Построение кусочнолинейной кривой

        private static List<Point2D> piecewiseLinearCurve(IEdge edge)
        {
            return PiecewiseLinearCurve(edge.PolygonPoints, edge.DstPointIndex);
        }

        private static void dump(IEdge edge)
        {
            var strPoints = edge.PolygonPoints
                .Select(x => $"new Point2D({x.X}, {x.Y})")
                .ToArray();
            Debug.WriteLine("var points = new List<Point2D>{");
            Debug.WriteLine(string.Join(",", strPoints));
            Debug.WriteLine("};");
            Debug.WriteLine("var result = EdgeRouteJob<VertexAttrs, EdgeAttrs>.PiecewiseLinearCurve(points, {0});", edge.DstPointIndex);
            Debug.WriteLine(@"Assert.IsTrue(result.Count > 1, ""Кривая не может состоять из одной точки."");");
        }

        public static List<Point2D> PiecewiseLinearCurve(IEnumerable<Point2D> polygonPoints, int iEnd)
        {
            var edgePolygon = new Polygon2D(polygonPoints);

            var start = edgePolygon.Points[0];
            var end = edgePolygon.Points[iEnd];
            var startPols = new List<Polygon2D>();
            var endPols = new List<Polygon2D>();

            // Разбиваем на выпуклые многоугольники.
            var convexPolygons = edgePolygon.SplitConvex();

            // Словарь: ребро -> список многоугольников, его содержащих.  
            var linePolMap = new Dictionary<Line2D, List<Polygon2D>>();
            foreach (var polygon in convexPolygons)
                foreach (var l in polygon.Edges)
                {
                    if (!linePolMap.TryGetValue(l, out var pols))
                    {
                        pols = new List<Polygon2D>();
                        linePolMap[l] = pols;
                    }
                    pols.Add(polygon);
                    if (l.P1 == start)
                        startPols.Add(polygon);
                    if (l.P1 == end)
                        endPols.Add(polygon);
                }

            // Создаем граф полигонов.
            var polygonGraph = new GraphModel<Polygon2D, Line2D>();
            foreach (var item in linePolMap)
                switch (item.Value.Count)
                {
                    case 1: // Внешнее ребро
                        break;
                    case 2: // Смежное ребро.
                        var e = polygonGraph.AddEdge(item.Value[0], item.Value[1], item.Key);
                        break;
                    default: // Этого быть не должно.
                        throw new Exception("Ошибка в алгоритме.");
                }

            // Создаем граф точек.
            var pointGraph = new GraphModel<Point2D, object>();
            foreach (var node in polygonGraph.Vertices)
            {
                var midPoints = node.Edges
                    .Select(x => x.Data.GetPoint(0.5))
                    .ToList();
                if (node.Data.Points.Contains(start))
                    midPoints.Add(start);
                if (node.Data.Points.Contains(end))
                    midPoints.Add(end);
                var midLines = combinate(midPoints);
                midLines.Iter(x => pointGraph.AddEdge(x.P1, x.P2, new object()));
            }

            // Добавляем в граф начальную точку.
            // Причем если в некотором многоугольнике содержится и конечная точка,
            // то добавляем ребро start -> end
            foreach (var startPol in startPols)
                startPol.Points.CircleIter((a, b) =>
                    pointGraph.AddEdge(start, b == end ? end : a + (b - a) / 2, null));

            // Добавляем в граф конечную точку.
            foreach (var endPol in endPols)
                endPol.Points.CircleIter((a, b) =>
                    pointGraph.AddEdge(end, a + (b - a) / 2, new object()));

            // Инициализируем вес ребер
            pointGraph.Edges.Iter(x => x.Weight = (x.Dst.Data - x.Src.Data).Len);

            var shortestPath = new List<Point2D>();
            var dijkstra = pointGraph.UndirectedDijkstra();
            dijkstra.EnterNode += x => shortestPath.Add(x.Data);
            dijkstra.Find(start, end);

            for (var i = shortestPath.Count - 2; i > 0; i--)
                if (edgePolygon.IsInner(shortestPath[i - 1], shortestPath[i + 1]))
                    shortestPath.RemoveAt(i);

            return shortestPath;
        }

        private static IEnumerable<Line2D> combinate(IList<Point2D> points)
        {
            for (var i = 0; i < points.Count; i++)
                for (var j = i + 1; j < points.Count; j++)
                    yield return new Line2D(points[i], points[j]);
        }

        #endregion

        #region Построение петли.

        private static List<Point2D> loopCurve(IEdge edge)
        {
            return new List<Point2D>
            {
                edge.Src.CenterPoint(),
                edge.Src.CustomPoint(1,0.5) + new Vector2D(10,10),
                edge.Src.CustomPoint(1,0.5) + new Vector2D(30,0),
                edge.Src.CustomPoint(1,0.5) + new Vector2D(10,-10),
                edge.Src.CenterPoint(),
            };
        }

        #endregion

        #endregion
    }
}
