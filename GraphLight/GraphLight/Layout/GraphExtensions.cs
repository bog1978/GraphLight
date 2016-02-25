using System.Collections.Generic;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    internal static class GraphExtensions
    {
        public static IEnumerable<List<IVertex<TVertex, TEdge>>> GetRankList<TVertex, TEdge>(
            this IGraph<TVertex, TEdge> graph)
            where TVertex : IVertexAttrs
            where TEdge : IEdgeAttrs
        {
            return
                from node in graph.Verteces
                orderby node.Data.Rank, node.Data.Position
                group node by node.Data.Rank
                    into rank
                    select rank.ToList();
        }

        public static IDictionary<int, List<IVertex<TVertex, TEdge>>> GetRankMap<TVertex, TEdge>(
            this IGraph<TVertex, TEdge> graph)
            where TVertex : IVertexAttrs
        {
            var ranks =
                from node in graph.Verteces
                orderby node.Data.Rank, node.Data.Position
                group node by node.Data.Rank
                    into rank
                    select rank;
            return ranks.ToDictionary(rank => rank.Key, rank => rank.ToList());
        }

        /// <summary>
        /// Makes graph acyclic by reversing some edges.
        /// </summary>
        /// <typeparam name="TVertex"></typeparam>
        /// <typeparam name="TEdge"></typeparam>
        /// <param name="graph"></param>
        public static void Acyclic<TVertex, TEdge>(this IGraph<TVertex, TEdge> graph)
            where TVertex : IVertexAttrs
            where TEdge : IEdgeAttrs
        {
            var backEdges = new List<IEdge<TVertex, TEdge>>();
            var dfs = new DepthFirstSearch<TVertex, TEdge>(graph);
            dfs.OnBackEdge = backEdges.Add;
            dfs.Find();
            foreach (var e in backEdges)
            {
                var tmp = e.Src;
                e.Src = e.Dst;
                e.Dst = tmp;
                e.Data.IsRevert = true;
            }
        }
    }
}