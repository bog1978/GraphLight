using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Collections;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    partial class GraphVizLayout<TVertex, TEdge>
    {
        protected virtual void OrderVerteces()
        {
            nodeOrderDfs();
            nodeOrderMinCross();
            nodeOrderSort();
        }

        private Dictionary<IVertex<TVertex, TEdge>, int> _nodeColors;
        private int _count;

        private void nodeOrderDfs()
        {
            // Все узлы белые
            _nodeColors = Graph.Verteces.ToDictionary(x => x, x => -1);

            foreach (var node in Graph.Verteces.Where(node => _nodeColors[node] == -1))
            {
                dfs(node);
                _nodeColors[node] = 1;
            }

            var ranks = Graph.GetRankList();
            foreach (var rank in ranks)
                for (var i = 0; i < rank.Count; i++)
                    rank[i].Data.Position = i;
        }

        private void dfs(IVertex<TVertex, TEdge> node)
        {
            node.Data.Position = _count++;
            _nodeColors[node] = 0;
            foreach (var dst in node.OutEdges.Select(e => e.Dst).Where(dst => _nodeColors[dst] == -1))
            {
                dfs(dst);
                _nodeColors[dst] = 1;
            }
        }

        private void nodeOrderSort()
        {
            var ranks = Graph.GetRankList();

            var L = 0;
            for (var cnt = 0; cnt < 25; cnt++)
            {
                var oldQuality = ranks.Sum(x => (double)quality(x));

                foreach (var rank in ranks)
                {
                    for (var i = 0; i < rank.Count - 1; i++)
                    {
                        var n1 = rank[i];
                        var n2 = rank[i + 1];
                        var cross1 = crossCount(n1, n2);
                        var len1 = totalLength(n1, n2);

                        swapPositions(n1, n2);
                        var cross2 = crossCount(n1, n2);
                        var len2 = totalLength(n1, n2);

                        if (cross2 < cross1)
                            i = 0;
                        else if (cross2 == cross1 && len2 < len1)
                        {
                            i = 0;
                            L++;
                        }
                        else
                            swapPositions(n1, n2);
                    }

                    rank.Sort((x, y) => x.Data.Position.CompareTo(y.Data.Position));
                }

                var newQuality = ranks.Sum(x => (double)quality(x));

                if (newQuality == oldQuality)
                    break;
            }
        }

        private static void swapPositions(IVertex<TVertex, TEdge> n1, IVertex<TVertex, TEdge> n2)
        {
            var tmp = n1.Data.Position;
            n1.Data.Position = n2.Data.Position;
            n2.Data.Position = tmp;
        }

        private static int crossCount(IVertex<TVertex, TEdge> n1, IVertex<TVertex, TEdge> n2)
        {
            var cross1 =
                from e1 in n1.InEdges
                from e2 in n2.InEdges
                where e1.Cross(e2)
                select e1;

            var cross2 =
                from e1 in n1.OutEdges
                from e2 in n2.OutEdges
                where e1.Cross(e2)
                select e1;

            return cross1.Count() + cross2.Count();
        }

        private static double totalLength(IVertex<TVertex, TEdge> n1, IVertex<TVertex, TEdge> n2)
        {
            var l1 = n1.Edges.Sum(e => Math.Abs(e.Src.Data.Position - e.Dst.Data.Position) * e.Weight);
            var l2 = n2.Edges.Sum(e => Math.Abs(e.Src.Data.Position - e.Dst.Data.Position) * e.Weight);
            return l1 + l2;
        }

        private static int quality(IEnumerable<IVertex<TVertex, TEdge>> rank)
        {
            var cnt = 0;
            var edges = rank.SelectMany(x => x.OutEdges).ToArray();
            for (var i = 0; i < edges.Length; i++)
                for (var j = i + 1; j < edges.Length; j++)
                {
                    var e1 = edges[i];
                    var e2 = edges[j];
                    if (e1.Cross(e2))
                        cnt++;
                }
            return cnt;
        }

        private void nodeOrderMinCross()
        {
            var ranks =
                from node in Graph.Verteces
                orderby node.Data.Rank, node.Data.Position
                group node by node.Data.Rank
                    into rank
                    select rank.ToList();

            var bestPositions = Graph.Verteces.ToDictionary(x => x, x => x.Data.Position);
            var bestCrossing = double.MaxValue;
            var bestLenght = double.MaxValue;

            var isReverse = false;
            for (var i = 0; i < 10; i++)
            {
                var tmpRanks = isReverse ? ranks.Reverse() : ranks;

                tmpRanks.Iter((adjacentRank, rank) =>
                {
                    var order = new BaricenterOrderManager(Math.Max(adjacentRank.Count, rank.Count), isReverse);
                    rank.Iter(order.Insert);
                    order.UpdatePositions();
                    rank.Sort((a, b) => a.Data.Position.CompareTo(b.Data.Position));
                });

                var currCrossing = ranks.Sum(x => rankCross(x));
                var currLength = Graph.Edges.Sum(e => e.PositionSpan() * e.PositionSpan());

                if (currCrossing < bestCrossing || currCrossing == bestCrossing && currLength < bestLenght)
                {
                    bestPositions = Graph.Verteces.ToDictionary(x => x, x => x.Data.Position);
                    bestCrossing = currCrossing;
                    bestLenght = currLength;
                    i = 0;
                }
                isReverse = !isReverse;
            }
            foreach (var pair in bestPositions)
                pair.Key.Data.Position = pair.Value;
        }

        private static int rankCross(IEnumerable<IVertex<TVertex, TEdge>> rank)
        {
            var cnt = 0;
            var edges = rank.SelectMany(x => x.OutEdges).ToArray();
            for (var i = 0; i < edges.Length; i++)
                for (var j = i + 1; j < edges.Length; j++)
                {
                    var e1 = edges[i];
                    var e2 = edges[j];
                    if (cross(e1, e2))
                        cnt++;
                }
            return cnt;
        }

        private static bool cross(IEdge<TVertex, TEdge> e1, IEdge<TVertex, TEdge> e2)
        {
            if (e1 == e2)
                return false;
            return e1.Src.Data.Position < e2.Src.Data.Position && e1.Dst.Data.Position > e2.Dst.Data.Position
                || e1.Src.Data.Position > e2.Src.Data.Position && e1.Dst.Data.Position < e2.Dst.Data.Position;
        }

        public abstract class OrderManager
        {
            private readonly bool _isRevertPath;

            private readonly IList<IList<IVertex<TVertex, TEdge>>>
                _nodeGroups = new List<IList<IVertex<TVertex, TEdge>>>();

            protected OrderManager(int count, bool isRevertPath)
            {
                for (var i = 0; i < count; i++)
                    _nodeGroups.Add(new List<IVertex<TVertex, TEdge>>());
                _isRevertPath = isRevertPath;
            }

            public void Insert(IVertex<TVertex, TEdge> node)
            {
                var index = _isRevertPath
                    ? GetReverseIndex(node)
                    : GetDirectIndex(node);
                _nodeGroups[index].Add(node);
            }

            public void UpdatePositions()
            {
                _nodeGroups
                    .SelectMany(x => x.Reverse())
                    .Iter((n, i) => { n.Data.Position = i; });
            }

            protected abstract int GetDirectIndex(IVertex<TVertex, TEdge> node);
            protected abstract int GetReverseIndex(IVertex<TVertex, TEdge> node);
        }

        public class BaricenterOrderManager : OrderManager
        {
            public BaricenterOrderManager(int count, bool isRevertPath)
                : base(count, isRevertPath) { }

            protected override int GetDirectIndex(IVertex<TVertex, TEdge> node)
            {
                return node.InEdges.Any()
                    ? (int)Math.Round(
                        node.InEdges.Select(x => x.Src.Data.Position).Average())
                    : node.Data.Position;
            }

            protected override int GetReverseIndex(IVertex<TVertex, TEdge> node)
            {
                return node.OutEdges.Any()
                    ? (int)Math.Round(
                        node.OutEdges.Select(x => x.Dst.Data.Position).Average())
                    : node.Data.Position;
            }
        }

        public class MedianOrderManager : OrderManager
        {
            public MedianOrderManager(int count, bool isRevertPath)
                : base(count, isRevertPath) { }

            protected override int GetDirectIndex(IVertex<TVertex, TEdge> node)
            {
                var positions = node.InEdges
                    .Select(x => x.Src.Data.Position)
                    .OrderBy(x => x)
                    .ToList();
                if (node.InEdges.Any())
                {
                    var index = positions.Count % 2 == 0
                        ? positions.Count / 2 - 1
                        : positions.Count / 2;
                    return positions[index];
                }
                return node.Data.Position;
            }

            protected override int GetReverseIndex(IVertex<TVertex, TEdge> node)
            {
                var positions = node.OutEdges
                    .Select(x => x.Dst.Data.Position)
                    .OrderBy(x => x)
                    .ToList();
                if (node.OutEdges.Any())
                {
                    var index = positions.Count % 2 == 0
                        ? positions.Count / 2 - 1
                        : positions.Count / 2;
                    return positions[index];
                }
                return node.Data.Position;
            }
        }
    }
}
