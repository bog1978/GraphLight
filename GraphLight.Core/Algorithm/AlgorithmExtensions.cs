using System;
using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    public static class AlgorithmExtensions
    {
        public static IShortestPath<V, E> UndirectedDijkstra<V, E>(this IGraph<V, E> graph)
        where E : IEdgeDataWeight
            => new UndirectedDijkstra<V, E>(graph);

        public static IDepthFirstSearch<V, E> DepthFirstSearch<V, E>(this IGraph<V, E> graph)
            => new DepthFirstSearch<V, E>(graph);

        public static ISpanningTree<V, E> PrimSpanningTree<V, E>(this IGraph<V, E> graph, Func<IEdge<V, E>, double> weightFunc)
            => new PrimSpanningTree<V, E>(graph, weightFunc);

        public static ISpanningTree<V, E> KruskalSpanningTree<V, E>(this IGraph<V, E> graph, Func<IEdge<V, E>, double> weightFunc)
            where V : IEquatable<V>
            where E : IEdgeDataWeight
            => new KruskalSpanningTree<V, E>(graph, weightFunc);
    }
}