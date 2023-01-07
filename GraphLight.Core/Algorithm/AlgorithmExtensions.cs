using System;
using GraphLight.Graph;
using GraphLight.Layout;

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

        public static IAlgorithm RankNetworkSimplex<V, E>(this IGraph<V, E> graph)
            where V : IVertexDataLayered
            where E : IEdgeDataWeight
            => new RankNetworkSimplex<V, E>(graph);

        public static IAlgorithm PositionNetworkSimplex<V, E>(this IGraph<V, E> graph)
            where V : IVertexDataLayered, IVertexDataLocation
            where E : IEdgeDataWeight
            => new PositionNetworkSimplex<V, E>(graph);
    }
}