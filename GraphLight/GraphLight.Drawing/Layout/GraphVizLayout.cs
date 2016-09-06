using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Collections;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    public partial class GraphVizLayout<TVertex, TEdge> : GraphLayout<TVertex, TEdge>
        where TVertex : IVertexAttrs, new()
        where TEdge : IEdgeAttrs, new()
    {
        private const double V_SPACE = 50;
        private const double H_SPACE = 30;

        public override void Layout()
        {
            var weights = Graph.Edges.Backup(x => x.Weight);

            foreach (var vertex in Graph.Verteces)
                NodeMeasure.Measure(vertex);

            Acyclic();
            RankVerteces();
            addTmpNodes();
            OrderVerteces();
            ArrangeVerteces();
            RouteEdges();

            weights.Restore((e, x) => e.Weight = x);
        }

        protected virtual void Acyclic()
        {
            var backEdges = new List<IEdge<TVertex, TEdge>>();
            var dfs = new DepthFirstSearch<TVertex, TEdge>(Graph);
            dfs.OnBackEdge += backEdges.Add;
            dfs.Find();
            foreach (var e in backEdges)
                e.Revert();
        }

        protected virtual void RankVerteces()
        {
            var alg = new RankNetworkSimplex<TVertex, TEdge>(Graph);
            alg.Execute();
        }

        protected virtual void ArrangeVerteces()
        {
            setTopPositions();
            setLeftPositions();
            Graph.Width = Graph.Verteces.Min(x => x.Data.Left) + Graph.Verteces.Max(x => x.Data.Right);
            Graph.Height = Graph.Verteces.Min(x => x.Data.Top) + Graph.Verteces.Max(x => x.Data.Bottom);
        }

        private void setTopPositions()
        {
            var rows =
                (from node in Graph.Verteces
                 group node by node.Data.Rank
                     into row
                     let r = row.Key
                     let h = row.Max(x => x.Data.Height) + V_SPACE
                     select new { r, h })
                    .ToDictionary(x => x.r, x => x.h);

            foreach (var node in Graph.Verteces)
            {
                var rank = node.Data.Rank;
                var y = (rows[rank] - node.Data.Height) / 2
                    + rows.Where(z => z.Key < rank).Sum(z => z.Value);
                node.Data.Top = y;
            }
        }

        /// <summary>
        ///   Вычисление оптимальных горизонтальных координат.
        ///   Применяется симплекс-метод.
        /// </summary>
        private void setLeftPositions()
        {
            var alg = new PositionNetworkSimplex<TVertex, TEdge>(Graph);
            alg.Execute();
        }

        private void addTmpNodes()
        {
            var edgesToSplit =
                from e in Graph.Edges
                where Math.Abs(e.Dst.Data.Rank - e.Src.Data.Rank) > 1
                select e;

            foreach (var edge in edgesToSplit.ToList())
            {
                var edge1 = edge;
                var distance = (edge.Dst.Data.Rank - edge.Src.Data.Rank);
                var increment = Math.Sign(distance);
                for (var rankShift = increment; rankShift != distance; rankShift += increment)
                {
                    var newNode = Graph.InsertControlPoint(edge1);
                    newNode.Data.Id = "mid_" + newNode.Data.Id;
                    newNode.Data.IsTmp = true;
                    newNode.Data.Rank = edge.Src.Data.Rank + rankShift;
                    edge1 = newNode.OutEdges.First();
                }
            }

            foreach (var e in Graph.Edges)
                e.Weight = e.Src.Data.IsTmp
                    ? (e.Dst.Data.IsTmp ? 8 : 2)
                    : (e.Dst.Data.IsTmp ? 2 : 1);
        }

        private void removeTmpNodes()
        {
            var tmpNodes = Graph.Verteces
                .Where(x => x.Data.IsTmp)
                .ToList();
            tmpNodes.Iter(x => Graph.RemoveControlPoint(x));
        }
    }
}