using System.Collections.Generic;
using GraphLight.Algorithm;

namespace GraphLight.Graph
{
    public static class GraphExtensions
    {
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
                e.Revert();
        }
    }
}