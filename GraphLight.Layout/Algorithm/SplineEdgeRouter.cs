using System.Collections.Generic;
using System.Linq;
using GraphLight.Collections;
using GraphLight.Geometry;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    internal class SplineEdgeRouter : IAlgorithm
    {
        private readonly IGraph<IGraphData, IVertexData, IEdgeData> _graph;

        public SplineEdgeRouter(IGraph<IGraphData, IVertexData, IEdgeData> graph)
        {
            _graph = graph;
        }

        public void Execute()
        {
            var dfs = _graph.DepthFirstSearch(TraverseRule.PreOrder);

            IEdge<IVertexData, IEdgeData>? chainEdge = null;

            dfs.OnEdge += (e, t) =>
            {
                if (!e.Src.Data.IsTmp && !e.Dst.Data.IsTmp)
                {
                    // Обычное ребро.
                    e.Data.Points.Add(e.Src.CenterPoint());
                    e.Data.Points.Add(e.Dst.CenterPoint());
                }
                else if (!e.Src.Data.IsTmp && e.Dst.Data.IsTmp)
                {
                    // Начало цепочки.
                    chainEdge = e;
                    chainEdge.Data.Points.Add(e.Src.CenterPoint());
                }
                else if (e.Src.Data.IsTmp && e.Dst.Data.IsTmp)
                {
                    // Середина цепочки.
                    chainEdge?.Data.Points.Add(e.Src.CenterPoint());
                }
                else if (e.Src.Data.IsTmp && !e.Dst.Data.IsTmp)
                {
                    // Конец цепочки.
                    chainEdge?.Data.Points.Add(e.Src.CenterPoint());
                    chainEdge?.Data.Points.Add(e.Dst.CenterPoint());
                    chainEdge = null;
                }
            };
            dfs.Execute();

            RemoveTmpNodes();

            foreach (var edge in _graph.Edges)
            {
                if (Equals(edge.Src, edge.Dst))
                {
                    edge.Data.Points.Clear();
                    var p = LoopCurve(edge);
                    p.Iter(edge.Data.Points.Add);
                }

                var points = edge.Data.Points;
                var srcPort = edge.Src.GetShapePort(points[1]);
                var dstPort = edge.Dst.GetShapePort(points[points.Count - 2]);
                points[0] = srcPort;
                points[points.Count - 1] = dstPort;

                // Если ребро было инвертировано, нужно инвертировать и точки.
                if (edge.IsRevert)
                    ReversePoints(points);
            }
        }

        /// <summary>
        /// Переворачивает список точек наоборот.
        /// </summary>
        /// <param name="points">Список точек.</param>
        private static void ReversePoints(IList<Point2D> points)
        {
            for (var i = 0; i < points.Count / 2; i++)
            {
                var j = points.Count - 1 - i;
                var a = points[i];
                var b = points[j];
                points[i] = b;
                points[j] = a;
            }
        }

        private void RemoveTmpNodes()
        {
            var tmpNodes = _graph.Vertices
                .Where(x => x.Data.IsTmp)
                .ToList();
            tmpNodes.Iter(x => _graph.RemoveControlPoint(x));
        }

        private static IEnumerable<Point2D> LoopCurve(IEdge<IVertexData, IEdgeData> edge)
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
    }
}