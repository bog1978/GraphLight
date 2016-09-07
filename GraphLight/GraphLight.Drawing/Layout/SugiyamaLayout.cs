using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    public class SugiyamaLayout<TVertex, TEdge>
        where TVertex : VertexAttrs
        where TEdge : /*IEdgeAttrs,*/ new()
    {
        private Graph<TVertex, TEdge> _graph;
        private ICollection<Edge<TVertex, TEdge>> _loops;
        private IDictionary<Edge<TVertex, TEdge>, List<Edge<TVertex, TEdge>>> _merged;

        public SugiyamaLayout(Graph<TVertex, TEdge> graph)
        {
            if (graph == null)
                throw new ArgumentNullException("graph");
            _graph = graph;
        }

        public void Layout()
        {
            removeLoops();
            acyclic();
            mergeEdges();

            // Layout

            restoreMergedEdges();
            restoreLoops();
        }

        /// <summary>
        /// If verteces A and B are connected with multiple edges these edges must be
        /// temporary replaced with one edge with weight equal to sum of weights.
        /// </summary>
        private void mergeEdges()
        {
            // Save edges to restore them later.
            _merged = _graph.Edges
                .GroupBy(x => x, new EdgeComparer())
                .Where(x => x.Count() > 1)
                .ToDictionary(
                    x =>
                    {
                        var e = _graph.AddEdge(
                            x.Key.Src.Data,
                            x.Key.Dst.Data,
                            new TEdge());
                        e.Weight = x.Sum(y => y.Weight);
                        return e;
                    },
                    x => x.ToList());

            // Remove merged edges from graph.
            foreach (var grp in _merged.Values)
                foreach (var e in grp)
                    _graph.RemoveEdge(e);
        }

        /// <summary>
        /// Restores previously merged edges before edge routing.
        /// </summary>
        private void restoreMergedEdges()
        {
        }

        /// <summary>
        /// Temporary removes loops to reduce total amount of edges.
        /// </summary>
        private void removeLoops()
        {
            _loops = _graph.Edges.Where(x => x.Src == x.Dst).ToList();
            foreach (var e in _loops)
                _graph.RemoveEdge(e);
        }

        /// <summary>
        /// Restores temporary removed loops before edge routing.
        /// </summary>
        private void restoreLoops()
        {
            foreach (var e in _loops)
                _graph.AddEdge(e.Src.Data, e.Dst.Data, e.Data);
        }

        /// <summary>
        /// Makes graph acyclic by reverting some edges. This is a requirement of simplex method.
        /// </summary>
        private void acyclic()
        {
            var backEdges = new List<Edge<TVertex, TEdge>>();
            var dfs = new DepthFirstSearch<TVertex, TEdge>(_graph);
            dfs.OnBackEdge = backEdges.Add;
            dfs.Find();
            foreach (var e in backEdges)
            {
                var tmp = e.Src;
                e.Src = e.Dst;
                e.Dst = tmp;
                e.IsRevert = true;
            }
        }

        private class EdgeComparer : IEqualityComparer<Edge<TVertex, TEdge>>
        {
            public bool Equals(Edge<TVertex, TEdge> x, Edge<TVertex, TEdge> y)
            {
                return x.Src == y.Src && x.Dst == y.Dst
                    || x.Src == y.Dst && x.Dst == y.Src;
            }

            public int GetHashCode(Edge<TVertex, TEdge> obj)
            {
                return obj.Src.GetHashCode() & obj.Dst.GetHashCode();
            }
        }
    }
}
