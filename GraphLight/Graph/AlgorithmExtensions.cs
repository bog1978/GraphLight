using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphLight.Graph
{
    public static class AlgorithmExtensions
    {
        public static void TraverseOutEdgesBfs<TVertex, TEdge>(this BaseGraph<TVertex, TEdge>.Vertex vertex, Action<BaseGraph<TVertex, TEdge>.Vertex> callback)
        {
            var map = new Dictionary<BaseGraph<TVertex, TEdge>.Vertex, VertexColor>();
            var q = new Queue<BaseGraph<TVertex, TEdge>.Vertex>(new[] { vertex });
            while (q.Any())
            {
                var v = q.Dequeue();
                callback(v);
                map[v] = VertexColor.Black;
                foreach (var edge in v.OutEdges.Where(edge => !map.ContainsKey(edge.Dst)))
                    q.Enqueue(edge.Dst);
            }
        }
    }
}
