using System;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    public static class AlgorithmExtensions
    {
        public static IShortestPath<V, E> UndirectedDijkstra<G, V, E>(this IGraph<G, V, E> graph)
        where E : IEdgeDataWeight
            => new UndirectedDijkstra<G, V, E>(graph);

        public static IDepthFirstSearch<V, E> DepthFirstSearch<G, V, E>(this IGraph<G, V, E> graph, TraverseRule rule)
            => new DepthFirstSearch<G, V, E>(graph, rule);

        public static ISpanningTree<V, E> PrimSpanningTree<G, V, E>(this IGraph<G, V, E> graph, Func<IEdge<V, E>, double> weightFunc)
            => new PrimSpanningTree<G, V, E>(graph, weightFunc);

        public static ISpanningTree<V, E> KruskalSpanningTree<G, V, E>(this IGraph<G, V, E> graph, Func<IEdge<V, E>, double> weightFunc)
            where V : IEquatable<V>
            where E : IEdgeDataWeight
            => new KruskalSpanningTree<G, V, E>(graph, weightFunc);
    }
}