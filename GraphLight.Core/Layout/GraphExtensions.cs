using System.Collections.Generic;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    public static class GraphExtensions
    {
        public static IEnumerable<List<IVertex<V, E>>> GetRankList<V, E>(this IGraph<V, E> graph)
            where V : IVertexDataLayered
        {
            return
                from node in graph.Vertices
                orderby node.Data.Rank, node.Data.Position
                group node by node.Data.Rank
                    into rank
                    select rank.ToList();
        }

        public static IDictionary<int, List<IVertex<V, E>>> GetRankMap<V, E>(this IGraph<V, E> graph)
            where V : IVertexDataLayered
        {
            var ranks =
                from node in graph.Vertices
                orderby node.Data.Rank, node.Data.Position
                group node by node.Data.Rank
                    into rank
                    select rank;
            return ranks.ToDictionary(rank => rank.Key, rank => rank.ToList());
        }

        /// <summary>
        /// Makes graph acyclic by reversing some edges.
        /// </summary>
        /// <param name="graph"></param>
        public static void Acyclic<V, E>(this IGraph<V, E> graph)
        {
            var backEdges = new List<IEdge<V, E>>();
            var dfs = graph.DepthFirstSearch();
            dfs.OnBackEdge = backEdges.Add;
            dfs.Execute();
            foreach (var e in backEdges)
            {
                var tmp = e.Src;
                e.Src = e.Dst;
                e.Dst = tmp;
                e.IsRevert = true;
            }
        }
    }
}