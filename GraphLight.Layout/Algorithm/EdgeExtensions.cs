using System;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    internal static class EdgeExtensions
    {
        internal static bool Cross<V, E>(this IEdge<V, E> edge, IEdge<V, E> otherEdge)
            where V : IEquatable<V>, IVertexDataLayered
        {
            var a1 = edge.Src;
            var b1 = edge.Dst;
            var a2 = otherEdge.Src;
            var b2 = otherEdge.Dst;
            return a1.Position < a2.Position && b1.Position > b2.Position
                || a1.Position > a2.Position && b1.Position < b2.Position
                || a1.Position == a2.Position && b1.Position == b2.Position;
        }

        internal static double PositionSpan<V, E>(this IEdge<V, E> edge)
            where V : IEquatable<V>, IVertexDataLayered
            where E : IEdgeDataWeight
        {
            var delta = edge.Src.Position - edge.Dst.Position;
            return Math.Abs(delta) * edge.Data.Weight;
        }
    }
}