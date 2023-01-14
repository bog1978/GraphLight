﻿using System.Collections.Generic;
using GraphLight.Algorithm;

namespace GraphLight.Model
{
    public static class GraphExtensions
    {
        /// <summary>
        /// Makes graph acyclic by reversing some edges.
        /// </summary>
        /// <param name="graph"></param>
        public static void Acyclic<G, V, E>(this IGraph<G, V, E> graph)
        {
            var backEdges = new List<IEdge<V, E>>();
            var dfs = graph.DepthFirstSearch();
            dfs.OnBackEdge = backEdges.Add;
            dfs.Execute();
            foreach (var e in backEdges)
                e.Revert();
        }
    }
}