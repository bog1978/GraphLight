﻿using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    public static class AlgorithmExtensions
    {
        public static IShortestPath<V, E> UndirectedDijkstra<V, E>(this IGraph<V, E> graph) => new UndirectedDijkstra<V, E>(graph);
    }
}