using System.Collections.Generic;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    internal static class GraphExtensions
    {
        public static IEnumerable<List<IVertex>> GetRankList(this IGraph graph)
        {
            return
                from node in graph.Vertices
                orderby node.Rank, node.Position
                group node by node.Rank
                    into rank
                    select rank.ToList();
        }

        public static IDictionary<int, List<IVertex>> GetRankMap(this IGraph graph)
        {
            var ranks =
                from node in graph.Vertices
                orderby node.Rank, node.Position
                group node by node.Rank
                    into rank
                    select rank;
            return ranks.ToDictionary(rank => rank.Key, rank => rank.ToList());
        }

        /// <summary>
        /// Makes graph acyclic by reversing some edges.
        /// </summary>
        /// <param name="graph"></param>
        public static void Acyclic(this IGraph graph)
        {
            var backEdges = new List<IEdge>();
            var dfs = new DepthFirstSearch(graph);
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
    }
}