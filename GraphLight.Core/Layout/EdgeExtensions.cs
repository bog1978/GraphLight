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
        {
            var delta = edge.Src.Data.Position - edge.Dst.Data.Position;
            return Math.Abs(delta) * edge.Weight;
        }

        public static IVertex<V, E> InsertControlPoint<V, E>(this IGraph<V, E> graph, IEdge<V, E> edge, V data)
        {
            return graph.InsertVertex(edge, data);
        }

        public static void RemoveControlPoint<V, E>(this IGraph<V, E> graph, IVertex<V, E> vertex)
        {
            var inCnt = vertex.InEdges.Count();
            var outCnt = vertex.OutEdges.Count();

            if (inCnt > 1 || outCnt > 1)
                throw new Exception("Can't remove node");

            if (inCnt == 0 && outCnt == 1)
                graph.RemoveEdge(vertex.OutEdges.First());
            else if (inCnt == 1 && outCnt == 0)
                graph.RemoveEdge(vertex.InEdges.First());
            else if (inCnt == 1 && outCnt == 1)
            {
                var inEdge = vertex.InEdges.First();
                var outEdge = vertex.OutEdges.First();

                inEdge.Dst = outEdge.Dst;
                graph.RemoveEdge(outEdge);
            }
            graph.RemoveVertex(vertex);
        }
    }
}