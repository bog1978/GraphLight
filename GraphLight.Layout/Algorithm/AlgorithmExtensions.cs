using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    internal static class AlgorithmExtensions
    {
        internal static IAlgorithm RankNetworkSimplex<V, E>(this IGraph<V, E> graph)
            where V : IVertexDataLayered
            where E : IEdgeDataWeight
            => new RankNetworkSimplex<V, E>(graph);

        internal static IAlgorithm PositionNetworkSimplex<V, E>(this IGraph<V, E> graph)
            where V : IVertexDataLayered, IVertexDataLocation
            where E : IEdgeDataWeight
            => new PositionNetworkSimplex<V, E>(graph);
    }
}