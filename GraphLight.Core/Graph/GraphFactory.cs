namespace GraphLight.Graph
{
    public static class GraphFactory
    {
        public static IGraph<G, V, E> CreateInstance<G, V, E>(G data)
        {
            return new GenericGraph<G, V, E>(data);
        }
    }
}