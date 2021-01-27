using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphLight.Graph
{
    public static class AlgorithmExtensions
    {
        public static void TraverseOutEdgesBfs<V, E>(this IVertex<V, E> vertex, Action<IVertex<V, E>> callback)
        {
            var map = new Dictionary<IVertex<V, E>, VertexColor>();
            var q = new Queue<IVertex<V, E>>(new[] { vertex });
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
