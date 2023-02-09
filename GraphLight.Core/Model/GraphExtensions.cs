using System;

namespace GraphLight.Model
{
    public static class GraphExtensions
    {
        public static void AddVertexRange<G, V, E>(this IGraph<G, V, E> graph, params V[] vertexList)
            where V : IEquatable<V>
        {
            foreach (var vertex in vertexList)
                graph.AddVertex(vertex);
        }

        public static void AddEdgeRange<G, V, E>(this IGraph<G, V, E> graph, E edgeData, params (V src, V dst)[] edgeList)
            where V : IEquatable<V>
        {
            foreach (var edge in edgeList)
                graph.AddEdge(edge.src, edge.dst, edgeData);
        }
    }
}