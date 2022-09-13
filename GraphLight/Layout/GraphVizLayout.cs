using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Collections;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    public partial class GraphVizLayout<V, E> : GraphLayout<V, E>
        where V : IVertexDataLayered, IVertexDataLocation
    {
        private const double V_SPACE = 50;
        private const double H_SPACE = 30;

        private int _tmpId;

        public override void Layout()
        {
            var weights = Graph.Edges.Backup(x => x.Weight);

            foreach (var vertex in Graph.Vertices)
                NodeMeasure.Measure(vertex);

            Acyclic();
            RankVertices();
            addTmpNodes();
            OrderVertices();
            ArrangeVertices();
            RouteEdges();

            weights.Restore((e, x) => e.Weight = x);
        }

        protected virtual void Acyclic()
        {
            var backEdges = new List<IEdge<V, E>>();
            var dfs = Graph.DepthFirstSearch();
            dfs.OnBackEdge += backEdges.Add;
            dfs.Execute();
            foreach (var e in backEdges.Cast<IEdge>())
                e.Revert();
        }

        protected virtual void RankVertices()
        {
            var alg = Graph.RankNetworkSimplex();
            alg.Execute();
        }

        protected virtual void ArrangeVertices()
        {
            var g = (IGraph)Graph;
            setTopPositions();
            setLeftPositions();
            g.Width = g.Vertices.Min(x => x.Data.Left) + g.Vertices.Max(x => x.Data.Right);
            g.Height = g.Vertices.Min(x => x.Data.Top) + g.Vertices.Max(x => x.Data.Bottom);
        }

        private void setTopPositions()
        {
            var rows =
                (from node in Graph.Vertices
                 group node by node.Data.Rank
                     into row
                     let r = row.Key
                     let h = row.Max(x => x.Data.Height) + V_SPACE
                     select new { r, h })
                    .ToDictionary(x => x.r, x => x.h);

            foreach (var node in Graph.Vertices)
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
            var alg = Graph.PositionNetworkSimplex();
            alg.Execute();
        }

        private void addTmpNodes()
        {
            var g = (IGraph)Graph;

            var edgesToSplit =
                from e in g.Edges
                where Math.Abs(e.Dst.Data.Rank - e.Src.Data.Rank) > 1
                select e;

            foreach (var edge in edgesToSplit.ToList())
            {
                var edge1 = edge;
                var distance = (edge.Dst.Data.Rank - edge.Src.Data.Rank);
                var increment = Math.Sign(distance);
                for (var rankShift = increment; rankShift != distance; rankShift += increment)
                {
                    var newNode = g.InsertControlPoint(edge1, new VertexData($"mid_{++_tmpId}"));
                    newNode.Data.IsTmp = true;
                    newNode.Data.Rank = edge.Src.Data.Rank + rankShift;
                    edge1 = (IEdge)newNode.OutEdges.First();
                }
            }

            foreach (var e in g.Edges)
                e.Weight = e.Src.Data.IsTmp
                    ? (e.Dst.Data.IsTmp ? 8 : 2)
                    : (e.Dst.Data.IsTmp ? 2 : 1);
        }

        private void removeTmpNodes()
        {
            var tmpNodes = Graph.Vertices
                .Where(x => x.Data.IsTmp)
                .ToList();
            tmpNodes.Iter(x => Graph.RemoveControlPoint(x));
        }
    }
}