#nullable enable
using System.Collections.Generic;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Collections;
using GraphLight.Geometry;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    internal class SplineEdgeRouter<V, E> : IAlgorithm
        where V : IVertexDataLayered, IVertexDataLocation
        where E : IEdgeData
    {
        private readonly IGraph<V, E> _graph;

        public SplineEdgeRouter(IGraph<V, E> graph)
        {
            _graph = graph;
        }

        public void Execute()
        {
            var dfs = _graph.DepthFirstSearch();

            IEdge<V, E>? chainEdge = null;

            dfs.OnEdge += (e, t) =>
            {
                switch (e.Src.Data.IsTmp, e.Dst.Data.IsTmp)
                {
                    case (false, false): // Обычное ребро.
                        e.Data.Points.Add(e.Src.CenterPoint());
                        e.Data.Points.Add(e.Dst.CenterPoint());
                        break;
                    case (false, true): // Начало цепочки.
                        chainEdge = e;
                        chainEdge.Data.Points.Add(e.Src.CenterPoint());
                        break;
                    case (true, true): // Середина цепочки.
                        chainEdge?.Data.Points.Add(e.Src.CenterPoint());
                        break;
                    case (true, false): // Конец цепочки.
                        chainEdge?.Data.Points.Add(e.Src.CenterPoint());
                        chainEdge?.Data.Points.Add(e.Dst.CenterPoint());
                        chainEdge = null;
                        break;
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
            }
        }

        private void RemoveTmpNodes()
        {
            var tmpNodes = _graph.Vertices
                .Where(x => x.Data.IsTmp)
                .ToList();
            tmpNodes.Iter(x => _graph.RemoveControlPoint(x));
        }

        private static IEnumerable<Point2D> LoopCurve(IEdge<V, E> edge)
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