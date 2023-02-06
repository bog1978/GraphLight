using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Collections;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    internal class VertextOrderer : IAlgorithm
    {
        private readonly IGraph<IGraphData, IVertexData, IEdgeData> _graph;
        private readonly IDictionary<IVertexData, int> _nodeColors;
        private int _count;

        public VertextOrderer(IGraph<IGraphData, IVertexData, IEdgeData> graph)
        {
            _graph = graph;
            // Все узлы белые
            _nodeColors = _graph.Vertices.ToDictionary(x => x, x => -1);
        }

        public void Execute()
        {
            nodeOrderDfs();
            nodeOrderMinCross();
            nodeOrderSort();
        }

        private void nodeOrderDfs()
        {
            foreach (var node in _graph.Vertices.Where(node => _nodeColors[node] == -1))
            {
                dfs(node);
                _nodeColors[node] = 1;
            }

            var ranks = _graph.GetRankList();
            foreach (var rank in ranks)
                for (var i = 0; i < rank.Count; i++)
                    rank[i].Position = i;
        }

        private void dfs(IVertexData node)
        {
            node.Position = _count++;
            _nodeColors[node] = 0;
            foreach (var dst in _graph.GetOutEdges(node).Select(e => e.Dst).Where(dst => _nodeColors[dst] == -1))
            {
                dfs(dst);
                _nodeColors[dst] = 1;
            }
        }

        private void nodeOrderSort()
        {
            var ranks = _graph.GetRankList();

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

                    rank.Sort((x, y) => x.Position.CompareTo(y.Position));
                }

                var newQuality = ranks.Sum(x => (double)quality(x));

                if (newQuality == oldQuality)
                    break;
            }
        }

        private static void swapPositions(IVertexData n1, IVertexData n2)
        {
            var tmp = n1.Position;
            n1.Position = n2.Position;
            n2.Position = tmp;
        }

        private int crossCount(IVertexData n1, IVertexData n2)
        {
            var cross1 =
                from e1 in _graph.GetInEdges(n1)
                from e2 in _graph.GetInEdges(n2)
                where e1.Cross(e2)
                select e1;

            var cross2 =
                from e1 in _graph.GetOutEdges(n1)
                from e2 in _graph.GetOutEdges(n2)
                where e1.Cross(e2)
                select e1;

            return cross1.Count() + cross2.Count();
        }

        private double totalLength(IVertexData n1, IVertexData n2)
        {
            var l1 = _graph.GetEdges(n1).Sum(e => Math.Abs(e.Src.Position - e.Dst.Position) * e.Data.Weight);
            var l2 = _graph.GetEdges(n2).Sum(e => Math.Abs(e.Src.Position - e.Dst.Position) * e.Data.Weight);
            return l1 + l2;
        }

        private int quality(IEnumerable<IVertexData> rank)
        {
            var cnt = 0;
            var edges = rank.SelectMany(x => _graph.GetOutEdges(x)).ToArray();
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
                from node in _graph.Vertices
                orderby node.Rank, node.Position
                group node by node.Rank
                    into rank
                select rank.ToList();

            var bestPositions = _graph.Vertices.ToDictionary(x => x, x => x.Position);
            var bestCrossing = double.MaxValue;
            var bestLenght = double.MaxValue;

            var isReverse = false;
            for (var i = 0; i < 10; i++)
            {
                var tmpRanks = isReverse ? ranks.Reverse() : ranks;

                tmpRanks.Iter((adjacentRank, rank) =>
                {
                    var order = new BaricenterOrderManager(_graph, Math.Max(adjacentRank.Count, rank.Count), isReverse);
                    rank.Iter(order.Insert);
                    order.UpdatePositions();
                    rank.Sort((a, b) => a.Position.CompareTo(b.Position));
                });

                var currCrossing = ranks.Sum(x => rankCross(x));
                var currLength = _graph.Edges.Sum(e => e.PositionSpan() * e.PositionSpan());

                if (currCrossing < bestCrossing || currCrossing == bestCrossing && currLength < bestLenght)
                {
                    bestPositions = _graph.Vertices.ToDictionary(x => x, x => x.Position);
                    bestCrossing = currCrossing;
                    bestLenght = currLength;
                    i = 0;
                }
                isReverse = !isReverse;
            }
            foreach (var pair in bestPositions)
                pair.Key.Position = pair.Value;
        }

        private int rankCross(IEnumerable<IVertexData> rank)
        {
            var cnt = 0;
            var edges = rank.SelectMany(x => _graph.GetOutEdges(x)).ToArray();
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

        private static bool cross(IEdge<IVertexData, IEdgeData> e1, IEdge<IVertexData, IEdgeData> e2)
        {
            if (e1 == e2)
                return false;
            return e1.Src.Position < e2.Src.Position && e1.Dst.Position > e2.Dst.Position
                || e1.Src.Position > e2.Src.Position && e1.Dst.Position < e2.Dst.Position;
        }

        private abstract class OrderManager
        {
            private readonly bool _isRevertPath;

            private readonly IList<IList<IVertexData>> _nodeGroups = new List<IList<IVertexData>>();

            protected OrderManager(int count, bool isRevertPath)
            {
                for (var i = 0; i < count; i++)
                    _nodeGroups.Add(new List<IVertexData>());
                _isRevertPath = isRevertPath;
            }

            public void Insert(IVertexData node)
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
                    .Iter((n, i) => { n.Position = i; });
            }

            protected abstract int GetDirectIndex(IVertexData node);
            protected abstract int GetReverseIndex(IVertexData node);
        }

        private class BaricenterOrderManager : OrderManager
        {
            private readonly IGraph<IGraphData, IVertexData, IEdgeData> _graph;

            public BaricenterOrderManager(IGraph<IGraphData, IVertexData, IEdgeData> graph, int count, bool isRevertPath)
                : base(count, isRevertPath) => _graph = graph;

            protected override int GetDirectIndex(IVertexData node)
            {
                return _graph.GetInEdges(node).Any()
                    ? (int)Math.Round(
                        _graph.GetInEdges(node).Select(x => x.Src.Position).Average())
                    : node.Position;
            }

            protected override int GetReverseIndex(IVertexData node)
            {
                return _graph.GetOutEdges(node).Any()
                    ? (int)Math.Round(
                        _graph.GetOutEdges(node).Select(x => x.Dst.Position).Average())
                    : node.Position;
            }
        }

        private class MedianOrderManager : OrderManager
        {
            private readonly IGraph<IGraphData, IVertexData, IEdgeData> _graph;

            public MedianOrderManager(IGraph<IGraphData, IVertexData, IEdgeData> graph, int count, bool isRevertPath)
                : base(count, isRevertPath) => _graph = graph;

            protected override int GetDirectIndex(IVertexData node)
            {
                var positions = _graph.GetInEdges(node)
                    .Select(x => x.Src.Position)
                    .OrderBy(x => x)
                    .ToList();
                if (_graph.GetInEdges(node).Any())
                {
                    var index = positions.Count % 2 == 0
                        ? positions.Count / 2 - 1
                        : positions.Count / 2;
                    return positions[index];
                }
                return node.Position;
            }

            protected override int GetReverseIndex(IVertexData node)
            {
                var positions = _graph.GetOutEdges(node)
                    .Select(x => x.Dst.Position)
                    .OrderBy(x => x)
                    .ToList();
                if (_graph.GetOutEdges(node).Any())
                {
                    var index = positions.Count % 2 == 0
                        ? positions.Count / 2 - 1
                        : positions.Count / 2;
                    return positions[index];
                }
                return node.Position;
            }
        }
    }
}
