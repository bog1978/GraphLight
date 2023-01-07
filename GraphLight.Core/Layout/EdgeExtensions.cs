using System;
using System.Linq;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    public static class EdgeExtensions
    {
        public static bool Cross<V, E>(this IEdge<V, E> edge, IEdge<V, E> otherEdge)
            where V : IVertexDataLayered
        {
            var a1 = edge.Src;
            var b1 = edge.Dst;
            var a2 = otherEdge.Src;
            var b2 = otherEdge.Dst;
            return a1.Data.Position < a2.Data.Position && b1.Data.Position > b2.Data.Position
                || a1.Data.Position > a2.Data.Position && b1.Data.Position < b2.Data.Position
                || a1.Data.Position == a2.Data.Position && b1.Data.Position == b2.Data.Position;
        }

        public static double PositionSpan<V, E>(this IEdge<V, E> edge)
            where V : IVertexDataLayered
            where E : IEdgeDataWeight
        {
            var delta = edge.Src.Data.Position - edge.Dst.Data.Position;
            return Math.Abs(delta) * edge.Data.Weight;
        }
    }
}