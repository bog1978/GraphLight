using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    public static class AlgorithmExtensions
    {
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