using GraphLight.Algorithm;
using System.Collections.Generic;

namespace GraphLight.Model
{
    public static class Graph
    {
        public static IGraph<G, V, E> CreateInstance<G, V, E>(G data)
        {
            return new GenericGraph<G, V, E>(data);
        }

        /// <summary>
        /// Makes graph acyclic by reversing some edges.
        /// </summary>
        /// <param name="graph"></param>
        public static void Acyclic<G, V, E>(this IGraph<G, V, E> graph)
        {
            var backEdges = new List<IEdge<V, E>>();
            var dfs = graph.DepthFirstSearch(TraverseRule.PreOrder);
            dfs.OnEdge = ei =>
            {
                if (ei.EdgeType == DfsEdgeType.Back)
                    backEdges.Add(ei.Edge);
            };
            dfs.Execute();
            foreach (var e in backEdges)
                e.Revert();
        }
    }
}