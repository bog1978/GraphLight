using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Collections;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    public partial class GraphVizLayout<V, E> : GraphLayout<V, E>
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
            dfs.Find();
            foreach (var e in backEdges.Cast<IEdge>())
                e.Revert();
        }

        protected virtual void RankVertices()
        {
            var alg = new RankNetworkSimplex((IGraph)Graph);
            alg.Execute();
        }

        protected virtual void ArrangeVertices()
        {
            var g = (IGraph)Graph;
            setTopPositions();
            setLeftPositions();
            g.Width = g.Vertices.Min(x => x.Left) + g.Vertices.Max(x => x.Right);
            g.Height = g.Vertices.Min(x => x.Top) + g.Vertices.Max(x => x.Bottom);
        }

        private void setTopPositions()
        {
            var g = (IGraph)Graph;
            var rows =
                (from node in g.Vertices
                 group node by node.Rank
                     into row
                     let r = row.Key
                     let h = row.Max(x => x.Height) + V_SPACE
                     select new { r, h })
                    .ToDictionary(x => x.r, x => x.h);

            foreach (var node in g.Vertices)
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
            var g = (IGraph)Graph;
            var alg = new PositionNetworkSimplex(g);
            alg.Execute();
        }

        private void addTmpNodes()
        {
            var g = (IGraph)Graph;

            var edgesToSplit =
                from e in g.Edges
                where Math.Abs(e.Dst.Rank - e.Src.Rank) > 1
                select e;

            foreach (var edge in edgesToSplit.ToList())
            {
                var edge1 = edge;
                var distance = (edge.Dst.Rank - edge.Src.Rank);
                var increment = Math.Sign(distance);
                for (var rankShift = increment; rankShift != distance; rankShift += increment)
                {
                    var newNode = (IVertex)g.InsertControlPoint(edge1, new VertexData($"mid_{++_tmpId}"));
                    newNode.IsTmp = true;
                    newNode.Rank = edge.Src.Rank + rankShift;
                    edge1 = newNode.OutEdges.First();
                }
            }

            foreach (var e in g.Edges)
                e.Weight = e.Src.IsTmp
                    ? (e.Dst.IsTmp ? 8 : 2)
                    : (e.Dst.IsTmp ? 2 : 1);
        }

        private void removeTmpNodes()
        {
            var g = (IGraph)Graph;

            var tmpNodes = g.Vertices
                .Where(x => x.IsTmp)
                .ToList();
            tmpNodes.Iter(x => g.RemoveControlPoint(x));
        }
    }
}