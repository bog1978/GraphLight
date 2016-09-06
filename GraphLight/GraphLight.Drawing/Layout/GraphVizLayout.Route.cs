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
    partial class GraphVizLayout<TVertex, TEdge>
    {
        protected virtual void RouteEdges()
        {
            calculatePolygons();
            removeTmpNodes();
            route();
        }

        #region Calculating polygons

        private IDictionary<int, List<IVertex<TVertex, TEdge>>> _rankMap;

        private void calculatePolygons()
        {
            _rankMap = Graph.GetRankMap();
            Graph.Edges.Iter(calculate);
        }

        private void calculate(IEdge<TVertex, TEdge> edge)
        {
            IList<IEdge<TVertex, TEdge>> edges = new List<IEdge<TVertex, TEdge>>();
            for (var currEdge = edge; currEdge != null;
                currEdge = currEdge.Dst.OutEdges.FirstOrDefault(x => x.Src.Data.IsTmp))
                edges.Add(currEdge);

            var points = new List<Point2D>();

            foreach (var e in edges)
            {
                calcSrcRightNode(e, points);
                calcDstRightNode(e, points);
            }

            //edge.SrcPointIndex = 0;
            edge.Data.DstPointIndex = points.Count - 1;

            foreach (var e in edges.Reverse())
            {
                calcDstLeftNode(e, points);
                calcSrcLeftNode(e, points);
            }

            var edgePolygon = new Polygon2D(points);
            edge.Data.DstPointIndex = edgePolygon.Simplify(edge.Data.DstPointIndex)[0];
            edge.Data.PolygonPoints = edgePolygon.Points;
        }

        #region Вычисление полигона

        private void calcSrcRightNode(IEdge<TVertex, TEdge> e, ICollection<Point2D> points)
        {
            var rank = _rankMap[e.Src.Data.Rank];
            var index = rank.IndexOf(e.Src);
            IList<IVertex<TVertex, TEdge>> nodes = new List<IVertex<TVertex, TEdge>>();

            for (var i = index; i < rank.Count; i++)
            {
                var node = rank[i];
                if (i == index && node.Data.IsTmp && i < rank.Count - 1)
                {
                    nodes.Add(rank[i + 1]);
                    break;
                }
                var btw = between(node.Data.CenterX, e.Src.Data.CenterX, e.Dst.Data.CenterX);
                if (!btw)
                    break;
                nodes.Add(node);
            }

            var rightNode = nodes.Last();
            foreach (var n in nodes)
            {
                if (n.Data.IsTmp)
                    points.Add(new Point2D(n.Data.Right, e.Src.Data.Bottom));
                else if (n == e.Src)
                {
                    points.Add(n.Data.CenterPoint());
                    if (e.Src.Data.CenterX < e.Dst.Data.CenterX)
                        points.Add(n.Data.CustomPoint(1, 0.5));
                    else
                        points.Add(n.Data.CustomPoint(1, 1));
                    if (n == rightNode && n.Data.Right < e.Dst.Data.CenterX)
                        points.Add(new Point2D(e.Dst.Data.CenterX, n.Data.Bottom));
                }
                else if (n == rightNode)
                {
                    points.Add(n.Data.CustomPoint(0, 1));
                    if (n.Data.Right < e.Dst.Data.CenterX)
                        points.Add(new Point2D(e.Dst.Data.CenterX, n.Data.Bottom));
                }
                else
                {
                    points.Add(n.Data.CustomPoint(0, 1));
                    points.Add(n.Data.CustomPoint(1, 1));
                }
            }
        }

        private void calcDstRightNode(IEdge<TVertex, TEdge> e, ICollection<Point2D> points)
        {
            var rank = _rankMap[e.Dst.Data.Rank];
            var index = rank.IndexOf(e.Dst);
            IList<IVertex<TVertex, TEdge>> nodes = new List<IVertex<TVertex, TEdge>>();

            for (var i = index; i < rank.Count; i++)
            {
                var node = rank[i];
                if (i == index && node.Data.IsTmp && i < rank.Count - 1)
                {
                    nodes.Add(rank[i + 1]);
                    break;
                }
                var btw = between(node.Data.CenterX, e.Src.Data.CenterX, e.Dst.Data.CenterX);
                if (!btw)
                    break;
                nodes.Add(node);
            }

            var rightNode = nodes.Last();
            foreach (var n in nodes.Reverse())
            {
                if (n.Data.IsTmp)
                    points.Add(new Point2D(n.Data.Right, e.Dst.Data.Top));
                else if (n == e.Dst)
                {
                    if (n == rightNode && n.Data.Right < e.Src.Data.CenterX)
                        points.Add(new Point2D(e.Src.Data.CenterX, n.Data.Top));
                    if (e.Src.Data.CenterX > e.Dst.Data.CenterX)
                        points.Add(n.Data.CustomPoint(1, 0.5));
                    else
                        points.Add(n.Data.CustomPoint(1, 0));
                    points.Add(n.Data.CenterPoint());
                }
                else if (n == rightNode)
                {
                    if (n.Data.Right < e.Src.Data.CenterX)
                        points.Add(new Point2D(e.Src.Data.CenterX, n.Data.Top));
                    points.Add(n.Data.CustomPoint(0, 0));
                }
                else
                {
                    points.Add(n.Data.CustomPoint(1, 0));
                    points.Add(n.Data.CustomPoint(0, 0));
                }
            }
        }

        private void calcDstLeftNode(IEdge<TVertex, TEdge> e, ICollection<Point2D> points)
        {
            var rank = _rankMap[e.Dst.Data.Rank];
            var index = rank.IndexOf(e.Dst);
            IList<IVertex<TVertex, TEdge>> nodes = new List<IVertex<TVertex, TEdge>>();

            for (var i = index; i >= 0; i--)
            {
                var node = rank[i];
                if (i == index && node.Data.IsTmp && i > 0)
                {
                    nodes.Add(rank[i - 1]);
                    break;
                }
                var btw = between(node.Data.CenterX, e.Src.Data.CenterX, e.Dst.Data.CenterX);
                if (!btw)
                    break;
                nodes.Add(node);
            }

            var leftNode = nodes.Last();
            foreach (var n in nodes)
            {
                if (n.Data.IsTmp)
                    points.Add(new Point2D(n.Data.Left, e.Dst.Data.Top));
                else if (n == e.Dst)
                {
                    points.Add(n.Data.CenterPoint());
                    if (e.Src.Data.CenterX < e.Dst.Data.CenterX)
                        points.Add(n.Data.CustomPoint(0, 0.5));
                    else
                        points.Add(n.Data.CustomPoint(0, 0));
                    if (n == leftNode && n.Data.Left > e.Src.Data.CenterX)
                        points.Add(new Point2D(e.Src.Data.CenterX, n.Data.Top));
                }
                else if (n == leftNode)
                {
                    points.Add(n.Data.CustomPoint(1, 0));
                    if (n.Data.Left > e.Src.Data.CenterX)
                        points.Add(new Point2D(e.Src.Data.CenterX, e.Dst.Data.Top));
                }
                else
                {
                    points.Add(n.Data.CustomPoint(1, 0));
                    points.Add(n.Data.CustomPoint(0, 0));
                }
            }
        }

        private void calcSrcLeftNode(IEdge<TVertex, TEdge> e, ICollection<Point2D> points)
        {
            var rank = _rankMap[e.Src.Data.Rank];
            var index = rank.IndexOf(e.Src);
            IList<IVertex<TVertex, TEdge>> nodes = new List<IVertex<TVertex, TEdge>>();

            for (var i = index; i >= 0; i--)
            {
                var node = rank[i];
                if (i == index && node.Data.IsTmp && i > 0)
                {
                    nodes.Add(rank[i - 1]);
                    break;
                }
                var btw = between(node.Data.CenterX, e.Src.Data.CenterX, e.Dst.Data.CenterX);
                if (!btw)
                    break;
                nodes.Add(node);
            }

            var leftNode = nodes.Last();
            foreach (var n in nodes.Reverse())
            {
                if (n.Data.IsTmp)
                    points.Add(new Point2D(n.Data.Left, e.Src.Data.Bottom));
                else if (n == e.Src)
                {
                    if (n == leftNode && n.Data.Left > e.Dst.Data.CenterX)
                        points.Add(new Point2D(e.Dst.Data.CenterX, n.Data.Bottom));
                    if (e.Src.Data.CenterX > e.Dst.Data.CenterX)
                        points.Add(n.Data.CustomPoint(0, 0.5));
                    else
                        points.Add(n.Data.CustomPoint(0, 1));
                    points.Add(n.Data.CenterPoint());
                }
                else if (n == leftNode)
                {
                    if (n.Data.Left > e.Dst.Data.CenterX)
                        points.Add(new Point2D(e.Dst.Data.CenterX, n.Data.Bottom));
                    points.Add(n.Data.CustomPoint(1, 1));
                }
                else
                {
                    points.Add(n.Data.CustomPoint(0, 1));
                    points.Add(n.Data.CustomPoint(1, 1));
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
            foreach (var edge in Graph.Edges)
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
                        edge.Src.Data.CenterPoint(),
                        edge.Dst.Data.CenterPoint(),
                    };
                }
                finally
                {
                    if (points.Count < 2)
                    {
                        dump(edge);
                    }
                    // Replace center points of source and destination nodes by port points.
                    var srcPort = edge.Src.Data.GetShapePort(points[1]);
                    var dstPort = edge.Dst.Data.GetShapePort(points[points.Count - 2]);
                    points[0] = srcPort;
                    points[points.Count - 1] = dstPort;

                    if (edge.Data.IsRevert)
                        points.Reverse();

                    edge.Data.Points.Clear();
                    foreach (var p in points)
                        edge.Data.Points.Add(p);
                }
            }
        }

        #region Построение кусочнолинейной кривой

        private static List<Point2D> piecewiseLinearCurve(IEdge<TVertex, TEdge> edge)
        {
            return PiecewiseLinearCurve(edge.Data.PolygonPoints, edge.Data.DstPointIndex);
        }

        private static void dump(IEdge<TVertex, TEdge> edge)
        {
            var strPoints = edge.Data.PolygonPoints
                .Select(x => string.Format("new Point2D({0}, {1})", x.X, x.Y))
                .ToArray();
            Debug.WriteLine("var points = new List<Point2D>{");
            Debug.WriteLine(string.Join(",", strPoints));
            Debug.WriteLine("};");
            Debug.WriteLine("var result = EdgeRouteJob<VertexAttrs, EdgeAttrs>.PiecewiseLinearCurve(points, {0});", edge.Data.DstPointIndex);
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
                    List<Polygon2D> pols;
                    if (!linePolMap.TryGetValue(l, out pols))
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
            var polygonGraph = new Graph<Polygon2D, Line2D>();
            foreach (var item in linePolMap)
                switch (item.Value.Count)
                {
                    case 1: // Внешнее ребро
                        break;
                    case 2: // Смежное ребро.
                        var e = polygonGraph.AddEdge(item.Value[0], item.Value[1]);
                        e.Data = item.Key;
                        break;
                    default: // Этого быть не должно.
                        throw new Exception("Ошибка в алгоритме.");
                }

            // Создаем граф точек.
            var pointGraph = new Graph<Point2D, object>();
            foreach (var node in polygonGraph.Verteces)
            {
                var midPoints = node.Edges
                    .Select(x => x.Data.GetPoint(0.5))
                    .ToList();
                if (node.Data.Points.Contains(start))
                    midPoints.Add(start);
                if (node.Data.Points.Contains(end))
                    midPoints.Add(end);
                var midLines = combinate(midPoints);
                midLines.Iter(x => pointGraph.AddEdge(x.P1, x.P2));
            }

            // Добавляем в граф начальную точку.
            // Причем если в некотором многоугольнике содержится и конечная точка,
            // то добавляем ребро start -> end
            foreach (var startPol in startPols)
                startPol.Points.CircleIter((a, b) =>
                    pointGraph.AddEdge(start, b == end ? end : a + (b - a) / 2));

            // Добавляем в граф конечную точку.
            foreach (var endPol in endPols)
                endPol.Points.CircleIter((a, b) =>
                    pointGraph.AddEdge(end, a + (b - a) / 2));

            // Инициализируем вес ребер
            pointGraph.Edges.Iter(x => x.Weight = (x.Dst.Data - x.Src.Data).Len);

            var shortestPath = new List<Point2D>();
            var dijkstra = new UndirectedDijkstra<Point2D, object>(pointGraph);
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

        private static List<Point2D> loopCurve(IEdge<TVertex, TEdge> edge)
        {
            return new List<Point2D>
            {
                edge.Src.Data.CenterPoint(),
                edge.Src.Data.CustomPoint(1,0.5) + new Vector2D(10,10),
                edge.Src.Data.CustomPoint(1,0.5) + new Vector2D(30,0),
                edge.Src.Data.CustomPoint(1,0.5) + new Vector2D(10,-10),
                edge.Src.Data.CenterPoint(),
            };
        }

        #endregion

        #endregion
    }
}
