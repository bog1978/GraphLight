using System;
using System.Linq;
using GraphLight.Collections;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    public class GraphVizLayout : GraphLayout
    {
        private const double V_SPACE = 50;
        private const double H_SPACE = 30;

        private int _tmpId;

        public override void Layout()
        {
            var weights = Graph.Edges.Backup(x => x.Data.Weight);

            foreach (var vertex in Graph.Vertices)
                NodeMeasure.Measure(vertex);

            Graph.Acyclic();
            RankVertices();
            AddTmpNodes();
            OrderVertices();
            ArrangeVertices();
            RouteEdges();

            weights.Restore((e, x) => e.Data.Weight = x);
        }

        protected virtual void RouteEdges() => new SplineEdgeRouter(Graph).Execute();

        protected virtual void OrderVertices() => new VertextOrderer(Graph).Execute();

        protected virtual void RankVertices()
        {
            var alg = Graph.RankNetworkSimplex();
            alg.Execute();
        }

        protected virtual void ArrangeVertices()
        {
            var g = Graph;
            SetTopPositions();
            SetLeftPositions();
            g.Data.Width = g.Vertices.Min(x => x.Rect.Left) + g.Vertices.Max(x => x.Rect.Right);
            g.Data.Height = g.Vertices.Min(x => x.Rect.Top) + g.Vertices.Max(x => x.Rect.Bottom);
        }

        private void SetTopPositions()
        {
            var rows =
                (from node in Graph.Vertices
                 group node by node.Rank
                     into row
                 let r = row.Key
                 let h = row.Max(x => x.Rect.Height) + V_SPACE
                 select new { r, h })
                    .ToDictionary(x => x.r, x => x.h);

            foreach (var node in Graph.Vertices)
            {
                var rank = node.Rank;
                var y = (rows[rank] - node.Rect.Height) / 2
                    + rows.Where(z => z.Key < rank).Sum(z => z.Value);
                node.Rect.Top = y;
            }
        }

        /// <summary>
        ///   Вычисление оптимальных горизонтальных координат.
        ///   Применяется симплекс-метод.
        /// </summary>
        private void SetLeftPositions()
        {
            var alg = Graph.PositionNetworkSimplex();
            alg.Execute();
        }

        private void AddTmpNodes()
        {
            var edgesToSplit =
                from e in Graph.Edges
                where Math.Abs(e.Dst.Rank - e.Src.Rank) > 1
                select e;

            foreach (var edge in edgesToSplit.ToList())
            {
                var edge1 = edge;
                var distance = edge.Dst.Rank - edge.Src.Rank;
                var increment = Math.Sign(distance);
                for (var rankShift = increment; rankShift != distance; rankShift += increment)
                {
                    var newNode = Graph.InsertControlPoint(edge1, new VertexData($"mid_{++_tmpId}"), new EdgeData());
                    newNode.IsTmp = true;
                    newNode.Rank = edge.Src.Rank + rankShift;
                    edge1 = Graph.GetOutEdges(newNode).First();
                }
            }

            foreach (var e in Graph.Edges)
                e.Data.Weight = e.Src.IsTmp
                    ? e.Dst.IsTmp ? 8 : 2
                    : e.Dst.IsTmp ? 2 : 1;
        }
    }
}