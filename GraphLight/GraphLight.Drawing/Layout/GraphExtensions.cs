using System.Collections.Generic;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    internal static class GraphExtensions
    {
        public static IEnumerable<List<Vertex<TVertex, TEdge>>> GetRankList<TVertex, TEdge>(
            this Graph<TVertex, TEdge> graph)
            where TVertex : VertexAttrs where TEdge : new()
            //where TEdge : IEdgeAttrs
        {
            return
                from node in graph.Verteces
                orderby node.Rank, node.Position
                group node by node.Rank
                    into rank
                    select rank.ToList();
        }

        public static IDictionary<int, List<Vertex<TVertex, TEdge>>> GetRankMap<TVertex, TEdge>(
            this Graph<TVertex, TEdge> graph)
            where TVertex : VertexAttrs where TEdge : new()
        {
            var ranks =
                from node in graph.Verteces
                orderby node.Rank, node.Position
                group node by node.Rank
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
        public static void Acyclic<TVertex, TEdge>(this Graph<TVertex, TEdge> graph)
            where TVertex : VertexAttrs where TEdge : new()
            //where TEdge : IEdgeAttrs
        {
            var backEdges = new List<Edge<TVertex, TEdge>>();
            var dfs = new DepthFirstSearch<TVertex, TEdge>(graph);
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