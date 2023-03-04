using System;
using System.Collections.Generic;
using GraphLight.Algorithm;

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

        public static void AddEdgeRange<G, V, E>(this IGraph<G, V, E> graph, params (V src, V dst, E data)[] edgeList)
            where V : IEquatable<V>
        {
            foreach (var edge in edgeList)
                graph.AddEdge(edge.src, edge.dst, edge.data);
        }

        public static IReadOnlyList<V> GetRoots<G, V, E>(this IGraph<G, V, E> graph)
            where V : IEquatable<V>
        {
            var roots = new List<V>();
            var dfs = graph.DepthFirstSearch(TraverseRule.PreOrder);
            dfs.OnNode += x =>
            {
                if (x.VertexType == DfsVertexType.Root)
                    roots.Add(x.Vertex);
            };
            dfs.Execute();
            return roots;
        }
    }
}