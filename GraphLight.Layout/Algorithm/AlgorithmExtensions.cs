using GraphLight.Model;

namespace GraphLight.Algorithm
{
    internal static class AlgorithmExtensions
    {
        internal static IAlgorithm RankNetworkSimplex<G, V, E>(this IGraph<G, V, E> graph)
            where V : IVertexDataLayered
            where E : IEdgeDataWeight
            => new RankNetworkSimplex<G, V, E>(graph);

        internal static IAlgorithm PositionNetworkSimplex<G, V, E>(this IGraph<G, V, E> graph)
            where V : IVertexDataLayered, IVertexDataLocation
            where E : IEdgeDataWeight
            => new PositionNetworkSimplex<G, V, E>(graph);
    }
}