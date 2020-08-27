using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphLight.Graph
{
    public static class AlgorithmExtensions
    {
        public static void TraverseOutEdgesBfs<TVertex, TEdge>(this Vertex<TVertex, TEdge> vertex, Action<Vertex<TVertex, TEdge>> callback)
        {
            var map = new Dictionary<Vertex<TVertex, TEdge>, VertexColor>();
            var q = new Queue<Vertex<TVertex, TEdge>>(new[] { vertex });
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
