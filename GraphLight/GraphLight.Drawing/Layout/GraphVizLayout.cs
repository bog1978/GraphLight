using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Collections;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    public partial class GraphVizLayout : GraphLayout
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
            var backEdges = new List<IEdge>();
            var dfs = new DepthFirstSearch(Graph);
            dfs.OnBackEdge += backEdges.Add;
            dfs.Find();
            foreach (var e in backEdges)
                e.Revert();
        }

        protected virtual void RankVerteces()
        {
            var alg = new RankNetworkSimplex(Graph);
            alg.Execute();
        }

        protected virtual void ArrangeVerteces()
        {
            setTopPositions();
            setLeftPositions();
            Graph.Width = Graph.Verteces.Min(x => x.Left) + Graph.Verteces.Max(x => x.Right);
            Graph.Height = Graph.Verteces.Min(x => x.Top) + Graph.Verteces.Max(x => x.Bottom);
        }

        private void setTopPositions()
        {
            var rows =
                (from node in Graph.Verteces
                 group node by node.Rank
                     into row
                     let r = row.Key
                     let h = row.Max(x => x.Height) + V_SPACE
                     select new { r, h })
                    .ToDictionary(x => x.r, x => x.h);

            foreach (var node in Graph.Verteces)
            {
                var rank = node.Rank;
                var y = (rows[rank] - node.Height) / 2
                    + rows.Where(z => z.Key < rank).Sum(z => z.Value);
                node.Top = y;
            }
        }

        /// <summary>
        ///   Вычисление оптимальных горизонтальных координат.
        ///   Применяется симплекс-метод.
        /// </summary>
        private void setLeftPositions()
        {
            var alg = new PositionNetworkSimplex(Graph);
            alg.Execute();
        }

        private void addTmpNodes()
        {
            var edgesToSplit =
                from e in Graph.Edges
                where Math.Abs(e.Dst.Rank - e.Src.Rank) > 1
                select e;

            foreach (var edge in edgesToSplit.ToList())
            {
                var edge1 = edge;
                var distance = (edge.Dst.Rank - edge.Src.Rank);
                var increment = Math.Sign(distance);
                for (var rankShift = increment; rankShift != distance; rankShift += increment)
                {
                    var newNode = Graph.InsertControlPoint(edge1);
                    //newNode.Id = "mid_" + newNode.Id;
                    newNode.IsTmp = true;
                    newNode.Rank = edge.Src.Rank + rankShift;
                    edge1 = newNode.OutEdges.First();
                }
            }

            foreach (var e in Graph.Edges)
                e.Weight = e.Src.IsTmp
                    ? (e.Dst.IsTmp ? 8 : 2)
                    : (e.Dst.IsTmp ? 2 : 1);
        }

        private void removeTmpNodes()
        {
            var tmpNodes = Graph.Verteces
                .Where(x => x.IsTmp)
                .ToList();
            tmpNodes.Iter(x => Graph.RemoveControlPoint(x));
        }
    }
}