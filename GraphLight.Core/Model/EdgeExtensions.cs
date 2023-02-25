using System;

namespace GraphLight.Model
{
    public static class EdgeExtensions
    {
        public static bool IsLoop<V, E>(this IEdge<V, E> edge)
            where V : IEquatable<V> =>
            edge.Src.Equals(edge.Dst);
    }
}